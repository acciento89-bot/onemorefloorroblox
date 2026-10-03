--!strict

local ProfileRules = {}

ProfileRules.CURRENT_VERSION = 1

export type Profile = {
	Version: number,
	BestFloor: number,
	Coins: number,
	OwnedCosmetics: { [string]: boolean },
	Equipped: {
		Trail: string,
		WinEffect: string,
		Theme: string,
	},
	Settings: {
		ReducedMotion: boolean,
		MasterVolume: number,
	},
	LastDailyDay: number,
}

local function copyMap(source: { [string]: boolean }): { [string]: boolean }
	local result = {}
	for key, value in source do
		if type(key) == "string" and value == true then
			result[key] = true
		end
	end
	return result
end

function ProfileRules.default(): Profile
	return {
		Version = ProfileRules.CURRENT_VERSION,
		BestFloor = 0,
		Coins = 0,
		OwnedCosmetics = {
			trail_none = true,
			win_none = true,
			theme_foundry = true,
		},
		Equipped = {
			Trail = "trail_none",
			WinEffect = "win_none",
			Theme = "theme_foundry",
		},
		Settings = {
			ReducedMotion = false,
			MasterVolume = 1,
		},
		LastDailyDay = 0,
	}
end

local function integer(value: any, minimum: number, maximum: number, fallback: number): number
	if type(value) ~= "number" or value ~= value or value == math.huge or value == -math.huge then
		return fallback
	end
	return math.clamp(math.floor(value), minimum, maximum)
end

local function validEquipped(owned: { [string]: boolean }, value: any, fallback: string): string
	if type(value) == "string" and owned[value] == true then
		return value
	end
	return fallback
end

function ProfileRules.sanitize(raw: any): Profile
	local defaults = ProfileRules.default()
	if type(raw) ~= "table" then
		return defaults
	end

	local owned = if type(raw.OwnedCosmetics) == "table" then copyMap(raw.OwnedCosmetics) else {}
	for id in defaults.OwnedCosmetics do
		owned[id] = true
	end

	local equipped = if type(raw.Equipped) == "table" then raw.Equipped else {}
	local settings = if type(raw.Settings) == "table" then raw.Settings else {}
	return {
		Version = ProfileRules.CURRENT_VERSION,
		BestFloor = integer(raw.BestFloor, 0, 1_000_000, 0),
		Coins = integer(raw.Coins, 0, 2_000_000_000, 0),
		OwnedCosmetics = owned,
		Equipped = {
			Trail = validEquipped(owned, equipped.Trail, defaults.Equipped.Trail),
			WinEffect = validEquipped(owned, equipped.WinEffect, defaults.Equipped.WinEffect),
			Theme = validEquipped(owned, equipped.Theme, defaults.Equipped.Theme),
		},
		Settings = {
			ReducedMotion = settings.ReducedMotion == true,
			MasterVolume = if type(settings.MasterVolume) == "number"
				then math.clamp(settings.MasterVolume, 0, 1)
				else defaults.Settings.MasterVolume,
		},
		LastDailyDay = integer(raw.LastDailyDay, 0, 100_000_000, 0),
	}
end

function ProfileRules.clone(profile: Profile): Profile
	return ProfileRules.sanitize(profile)
end

function ProfileRules.snapshot(profile: Profile): any
	local clean = ProfileRules.sanitize(profile)
	return {
		Version = clean.Version,
		BestFloor = clean.BestFloor,
		Coins = clean.Coins,
		OwnedCosmetics = clean.OwnedCosmetics,
		Equipped = clean.Equipped,
		Settings = clean.Settings,
		LastDailyDay = clean.LastDailyDay,
	}
end

return table.freeze(ProfileRules)
