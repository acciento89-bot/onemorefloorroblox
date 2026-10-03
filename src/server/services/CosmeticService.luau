--!strict

local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")

local shared = ReplicatedStorage:WaitForChild("OneMoreFloor"):WaitForChild("Shared")
local CosmeticConfig = require(shared.config.CosmeticConfig)

local FloorService = require(script.Parent.FloorService)
local ProfileService = require(script.Parent.ProfileService)

local CosmeticService = {}

local cosmeticAction: RemoteEvent
local cosmeticState: RemoteEvent
local cosmeticQuery: RemoteFunction

local lastActionAt: { [Player]: number } = {}
local characterConnections: { [Player]: RBXScriptConnection } = {}

local function color(values): Color3
	return Color3.fromRGB(values[1], values[2], values[3])
end

local function stateFor(player: Player): any
	local profile = ProfileService.get(player)
	if not profile then
		return nil
	end
	return {
		Coins = profile.Coins,
		OwnedCosmetics = profile.OwnedCosmetics,
		Equipped = profile.Equipped,
		Settings = profile.Settings,
	}
end

local function syncAttributes(player: Player)
	local profile = ProfileService.get(player)
	if not profile then
		return
	end
	player:SetAttribute("OMFTrail", profile.Equipped.Trail)
	player:SetAttribute("OMFWinEffect", profile.Equipped.WinEffect)
	player:SetAttribute("OMFTheme", profile.Equipped.Theme)
	player:SetAttribute("OMFReducedMotion", profile.Settings.ReducedMotion)
end

local function clearTrail(character: Model)
	local root = character:FindFirstChild("HumanoidRootPart")
	if not root or not root:IsA("BasePart") then
		return
	end
	for _, name in { "OMFTrail", "OMFTrailA0", "OMFTrailA1" } do
		local existing = root:FindFirstChild(name)
		if existing then
			existing:Destroy()
		end
	end
end
local function applyTrail(player: Player, character: Model)
	clearTrail(character)
	local profile = ProfileService.get(player)
	if not profile then
		return
	end
	local item = CosmeticConfig.Items[profile.Equipped.Trail]
	if not item or item.Id == "trail_none" then
		return
	end

	local root = character:FindFirstChild("HumanoidRootPart")
	if not root or not root:IsA("BasePart") then
		return
	end

	local a0 = Instance.new("Attachment")
	a0.Name = "OMFTrailA0"
	a0.Position = Vector3.new(-0.55, -0.8, 0.35)
	a0.Parent = root

	local a1 = Instance.new("Attachment")
	a1.Name = "OMFTrailA1"
	a1.Position = Vector3.new(0.55, -0.8, 0.35)
	a1.Parent = root

	local trail = Instance.new("Trail")
	trail.Name = "OMFTrail"
	trail.Attachment0 = a0
	trail.Attachment1 = a1
	trail.Color = ColorSequence.new(color(item.ColorRGB))
	trail.LightEmission = 0.45
	trail.Lifetime = 0.28
	trail.MinLength = 0.08
	trail.Transparency = NumberSequence.new({
		NumberSequenceKeypoint.new(0, 0.12),
		NumberSequenceKeypoint.new(1, 1),
	})
	trail.WidthScale = NumberSequence.new({
		NumberSequenceKeypoint.new(0, 0.75),
		NumberSequenceKeypoint.new(1, 0),
	})
	trail.Parent = root
end

local function applyCharacter(player: Player, character: Model)
	task.defer(function()
		local root = character:WaitForChild("HumanoidRootPart", 8)
		if not root then
			return
		end
		for _ = 1, 32 do
			if ProfileService.get(player) then
				applyTrail(player, character)
				return
			end
			task.wait(0.25)
		end
	end)
end

local function pushState(player: Player, eventName: string?)
	syncAttributes(player)
	local state = stateFor(player)
	if state then
		state.Event = eventName or "State"
		cosmeticState:FireClient(player, state)
	end
end
local function actionAllowed(player: Player): boolean
	local now = os.clock()
	local last = lastActionAt[player] or 0
	if now - last < 0.12 then
		return false
	end
	lastActionAt[player] = now
	return true
end

local function categoryField(category: string): string?
	if category == "Trail" then
		return "Trail"
	elseif category == "WinEffect" then
		return "WinEffect"
	elseif category == "Theme" then
		return "Theme"
	end
	return nil
end

local function onAction(player: Player, payload: any)
	if not actionAllowed(player) or type(payload) ~= "table" then
		return
	end
	local kind = payload.Kind
	if type(kind) ~= "string" then
		return
	end

	local profile = ProfileService.get(player)
	if not profile then
		return
	end

	if kind == "Setting" then
		if payload.Key ~= "ReducedMotion" or type(payload.Value) ~= "boolean" then
			return
		end
		profile.Settings.ReducedMotion = payload.Value
		ProfileService.markDirty(player)
		pushState(player, "SettingUpdated")
		return
	end

	local id = payload.Id
	if type(id) ~= "string" then
		return
	end
	local item = CosmeticConfig.Items[id]
	if not item then
		return
	end

	if kind == "Buy" then
		if profile.OwnedCosmetics[id] == true then
			pushState(player, "AlreadyOwned")
			return
		end
		if item.Cost <= 0 or profile.Coins < item.Cost then
			pushState(player, "InsufficientCoins")
			return
		end
		profile.Coins -= item.Cost
		profile.OwnedCosmetics[id] = true
		ProfileService.markDirty(player)
		FloorService.refreshProfile(player)
		pushState(player, "Purchased")
		return
	end

	if kind == "Equip" then
		if profile.OwnedCosmetics[id] ~= true then
			return
		end
		local field = categoryField(item.Category)
		if not field then
			return
		end
		profile.Equipped[field] = id
		ProfileService.markDirty(player)
		pushState(player, "Equipped")
		if item.Category == "Trail" and player.Character then
			applyTrail(player, player.Character)
		end
		return
	end
end
function CosmeticService.start()
	local remotes = ReplicatedStorage:WaitForChild("OneMoreFloorRemotes")
	cosmeticAction = remotes:WaitForChild("CosmeticAction") :: RemoteEvent
	cosmeticState = remotes:WaitForChild("CosmeticState") :: RemoteEvent
	cosmeticQuery = remotes:WaitForChild("CosmeticQuery") :: RemoteFunction

	cosmeticQuery.OnServerInvoke = function(player: Player)
		return stateFor(player)
	end
	cosmeticAction.OnServerEvent:Connect(onAction)

	local function bindPlayer(player: Player)
		if characterConnections[player] then
			characterConnections[player]:Disconnect()
		end
		characterConnections[player] = player.CharacterAdded:Connect(function(character)
			applyCharacter(player, character)
		end)
		if player.Character then
			applyCharacter(player, player.Character)
		end
		task.defer(function()
			for _ = 1, 32 do
				if ProfileService.get(player) then
					pushState(player, "Initial")
					return
				end
				task.wait(0.25)
			end
		end)
	end

	Players.PlayerAdded:Connect(bindPlayer)
	Players.PlayerRemoving:Connect(function(player)
		lastActionAt[player] = nil
		local connection = characterConnections[player]
		if connection then
			connection:Disconnect()
			characterConnections[player] = nil
		end
	end)

	for _, player in Players:GetPlayers() do
		task.defer(bindPlayer, player)
	end
end

return CosmeticService
