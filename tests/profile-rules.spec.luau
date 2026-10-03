return function()
	local ProfileRules = require("../src/shared/profile/ProfileRules")

	local fresh = ProfileRules.default()
	assert(fresh.Version == ProfileRules.CURRENT_VERSION, "default schema version")
	assert(fresh.BestFloor == 0 and fresh.Coins == 0, "default progression")
	assert(fresh.OwnedCosmetics.trail_none == true, "starter trail owned")
	assert(fresh.Equipped.Trail == "trail_none", "starter trail equipped")

	local dirty = {
		Version = -99,
		BestFloor = 12.9,
		Coins = -50,
		OwnedCosmetics = {
			trail_neon = true,
			bad = false,
		},
		Equipped = {
			Trail = "trail_neon",
			WinEffect = "not_owned",
			Theme = 123,
		},
		Settings = {
			ReducedMotion = true,
			MasterVolume = 7,
		},
		LastDailyDay = 123.8,
	}

	local clean = ProfileRules.sanitize(dirty)
	assert(clean.Version == ProfileRules.CURRENT_VERSION, "migration version")
	assert(clean.BestFloor == 12, "best floor integer sanitized")
	assert(clean.Coins == 0, "negative coins clamped")
	assert(clean.OwnedCosmetics.trail_neon == true, "owned cosmetic retained")
	assert(clean.OwnedCosmetics.trail_none == true, "starter entitlement restored")
	assert(clean.Equipped.Trail == "trail_neon", "owned equipped retained")
	assert(clean.Equipped.WinEffect == "win_none", "unowned win effect rejected")
	assert(clean.Equipped.Theme == "theme_foundry", "invalid theme rejected")
	assert(clean.Settings.ReducedMotion == true, "reduced motion retained")
	assert(clean.Settings.MasterVolume == 1, "volume clamped")
	assert(clean.LastDailyDay == 123, "daily day sanitized")

	local hostile = ProfileRules.sanitize({
		BestFloor = 0 / 0,
		Coins = math.huge,
		OwnedCosmetics = "bad",
	})
	assert(hostile.BestFloor == 0 and hostile.Coins == 0, "non-finite values rejected")
	assert(hostile.Equipped.Trail == "trail_none", "hostile profile falls back safely")
end
