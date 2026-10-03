--!strict

local ProfileRules = {}

ProfileRules.CURRENT_VERSION = 3

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
	DailyStreak: number,
	DailyGoalDay: number,
	DailyGoalProgress: number,
	DailyGoalClaimed: boolean,
	Achievements: { [string]: boolean },
	SeenFamilies: { [string]: boolean },
	FamilyWins: { [string]: number },
	ReviveCredits: number,
	SkipTokens: number,
	CoinMultiplierUntil: number,
	ProcessedReceipts: { [string]: boolean },
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

local function copyNumberMap(source: { [string]: number }): { [string]: number }
	local result = {}
	for key, value in source do
		if type(key) == "string" and type(value) == "number" and value == value and value >= 0 then
			result[key] = math.clamp(math.floor(value), 0, 1_000_000)
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
		DailyStreak = 0,
		DailyGoalDay = 0,
		DailyGoalProgress = 0,
		DailyGoalClaimed = false,
		Achievements = {},
		SeenFamilies = {},
		FamilyWins = {},
		ReviveCredits = 0,
		SkipTokens = 0,
		CoinMultiplierUntil = 0,
		ProcessedReceipts = {},
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
	local achievements = if type(raw.Achievements) == "table" then copyMap(raw.Achievements) else {}
	local seenFamilies = if type(raw.SeenFamilies) == "table" then copyMap(raw.SeenFamilies) else {}
	local familyWins = if type(raw.FamilyWins) == "table" then copyNumberMap(raw.FamilyWins) else {}
	local processedReceipts = if type(raw.ProcessedReceipts) == "table" then copyMap(raw.ProcessedReceipts) else {}
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
		DailyStreak = integer(raw.DailyStreak, 0, 10_000, 0),
		DailyGoalDay = integer(raw.DailyGoalDay, 0, 100_000_000, 0),
		DailyGoalProgress = integer(raw.DailyGoalProgress, 0, 10_000, 0),
		DailyGoalClaimed = raw.DailyGoalClaimed == true,
		Achievements = achievements,
		SeenFamilies = seenFamilies,
		FamilyWins = familyWins,
		ReviveCredits = integer(raw.ReviveCredits, 0, 1_000_000, 0),
		SkipTokens = integer(raw.SkipTokens, 0, 1_000_000, 0),
		CoinMultiplierUntil = integer(raw.CoinMultiplierUntil, 0, 4_000_000_000, 0),
		ProcessedReceipts = processedReceipts,
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
		DailyStreak = clean.DailyStreak,
		DailyGoalDay = clean.DailyGoalDay,
		DailyGoalProgress = clean.DailyGoalProgress,
		DailyGoalClaimed = clean.DailyGoalClaimed,
		Achievements = clean.Achievements,
		SeenFamilies = clean.SeenFamilies,
		FamilyWins = clean.FamilyWins,
		ReviveCredits = clean.ReviveCredits,
		SkipTokens = clean.SkipTokens,
		CoinMultiplierUntil = clean.CoinMultiplierUntil,
		ProcessedReceipts = clean.ProcessedReceipts,
	}
end

return table.freeze(ProfileRules)
