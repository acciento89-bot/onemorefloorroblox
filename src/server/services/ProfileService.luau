--!strict

local DataStoreService = game:GetService("DataStoreService")
local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local RunService = game:GetService("RunService")

local shared = ReplicatedStorage:WaitForChild("OneMoreFloor"):WaitForChild("Shared")
local ProfileRules = require(shared.profile.ProfileRules)

local ProfileService = {}

local STORE_NAME = "OneMoreFloor_Profile_v1"
local LOCK_SECONDS = 150
local AUTOSAVE_SECONDS = 60
local LOAD_ATTEMPTS = 3
local RETRY_DELAY = 1.5

type Session = {
	Profile: ProfileRules.Profile,
	Dirty: boolean,
	Persistent: boolean,
	Released: boolean,
}

local sessions: { [Player]: Session } = {}
local store: any = nil
local autosaveStarted = false

local function getStore(): any
	if store then
		return store
	end
	local ok, result = pcall(function()
		return DataStoreService:GetDataStore(STORE_NAME)
	end)
	if not ok then
		return nil
	end
	store = result
	return store
end

local function keyFor(player: Player): string
	return ("u_%d"):format(player.UserId)
end

local function lockRecord(profile: ProfileRules.Profile): any
	return {
		Profile = ProfileRules.snapshot(profile),
		Lock = {
			JobId = game.JobId,
			Timestamp = os.time(),
		},
		UpdatedAt = os.time(),
	}
end

local function acquire(player: Player): (ProfileRules.Profile, boolean)
	local activeStore = getStore()
	if not activeStore then
		warn("[One More Floor] DataStore unavailable; using ephemeral profile.")
		return ProfileRules.default(), false
	end

	if RunService:IsStudio() then
		local ok = pcall(function()
			activeStore:GetAsync("__omf_api_probe__")
		end)
		if not ok then
			warn("[One More Floor] DataStore unavailable in Studio; using ephemeral profile.")
			return ProfileRules.default(), false
		end
	end

	for attempt = 1, LOAD_ATTEMPTS do
		local acquired = false
		local loaded: ProfileRules.Profile? = nil
		local ok, err = pcall(function()
			activeStore:UpdateAsync(keyFor(player), function(existing)
				local now = os.time()
				if type(existing) == "table" and type(existing.Lock) == "table" then
					local owner = existing.Lock.JobId
					local timestamp = tonumber(existing.Lock.Timestamp) or 0
					if owner ~= nil and owner ~= game.JobId and now - timestamp < LOCK_SECONDS then
						return existing
					end
				end

				local rawProfile = if type(existing) == "table" then existing.Profile else existing
				loaded = ProfileRules.sanitize(rawProfile)
				acquired = true
				return lockRecord(loaded :: ProfileRules.Profile)
			end)
		end)

		if ok and acquired and loaded then
			return loaded, true
		end
		if not ok then
			warn(
				("[One More Floor] Profile load attempt %d failed for %d: %s"):format(
					attempt,
					player.UserId,
					tostring(err)
				)
			)
		end
		if attempt < LOAD_ATTEMPTS then
			task.wait(RETRY_DELAY * attempt)
		end
	end

	warn(
		("[One More Floor] Could not acquire persistent profile for %d; using protected ephemeral session."):format(
			player.UserId
		)
	)
	return ProfileRules.default(), false
end
local function writeSession(player: Player, session: Session, releaseLock: boolean): boolean
	if session.Released or not session.Persistent then
		return true
	end

	local activeStore = getStore()
	if not activeStore then
		return false
	end

	local snapshot = ProfileRules.snapshot(session.Profile)
	local ok, err = pcall(function()
		activeStore:UpdateAsync(keyFor(player), function(existing)
			if type(existing) == "table" and type(existing.Lock) == "table" then
				local owner = existing.Lock.JobId
				if owner ~= nil and owner ~= game.JobId then
					return existing
				end
			end

			local record = {
				Profile = snapshot,
				UpdatedAt = os.time(),
			}
			if not releaseLock then
				record.Lock = {
					JobId = game.JobId,
					Timestamp = os.time(),
				}
			end
			return record
		end)
	end)

	if not ok then
		warn(("[One More Floor] Profile save failed for %d: %s"):format(player.UserId, tostring(err)))
		return false
	end

	session.Dirty = false
	if releaseLock then
		session.Released = true
	end
	return true
end

function ProfileService.load(player: Player): ProfileRules.Profile
	local existing = sessions[player]
	if existing then
		return existing.Profile
	end

	local profile, persistent = acquire(player)
	sessions[player] = {
		Profile = profile,
		Dirty = false,
		Persistent = persistent,
		Released = false,
	}
	player:SetAttribute("OMFProfilePersistent", persistent)
	return profile
end

function ProfileService.get(player: Player): ProfileRules.Profile?
	local session = sessions[player]
	return if session then session.Profile else nil
end

function ProfileService.markDirty(player: Player)
	local session = sessions[player]
	if session then
		session.Dirty = true
	end
end

function ProfileService.save(player: Player): boolean
	local session = sessions[player]
	if not session then
		return true
	end
	return writeSession(player, session, false)
end

function ProfileService.release(player: Player): boolean
	local session = sessions[player]
	if not session then
		return true
	end

	local ok = writeSession(player, session, true)
	sessions[player] = nil
	return ok
end
local function autosaveLoop()
	while true do
		task.wait(AUTOSAVE_SECONDS)
		for player, session in sessions do
			if session.Dirty and player.Parent == Players then
				task.spawn(ProfileService.save, player)
			end
		end
	end
end

function ProfileService.start()
	if autosaveStarted then
		return
	end
	autosaveStarted = true
	task.spawn(autosaveLoop)

	game:BindToClose(function()
		local pending = 0
		local done = Instance.new("BindableEvent")
		for player in sessions do
			pending += 1
			task.spawn(function()
				ProfileService.release(player)
				pending -= 1
				if pending == 0 then
					done:Fire()
				end
			end)
		end
		if pending > 0 then
			task.delay(8, function()
				done:Fire()
			end)
			done.Event:Wait()
		end
		done:Destroy()
	end)
end

return ProfileService
