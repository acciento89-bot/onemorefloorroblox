--!strict

local items = {
	trail_none = table.freeze({
		Id = "trail_none",
		Category = "Trail",
		DisplayName = "Clean",
		Cost = 0,
		ColorRGB = { 180, 192, 205 },
	}),
	trail_cyan = table.freeze({
		Id = "trail_cyan",
		Category = "Trail",
		DisplayName = "Skyline",
		Cost = 120,
		ColorRGB = { 91, 220, 239 },
	}),
	trail_ember = table.freeze({
		Id = "trail_ember",
		Category = "Trail",
		DisplayName = "Ember",
		Cost = 180,
		ColorRGB = { 244, 166, 80 },
	}),
	win_none = table.freeze({
		Id = "win_none",
		Category = "WinEffect",
		DisplayName = "Clean",
		Cost = 0,
		ColorRGB = { 180, 192, 205 },
	}),
	win_burst = table.freeze({
		Id = "win_burst",
		Category = "WinEffect",
		DisplayName = "Pulse Burst",
		Cost = 160,
		ColorRGB = { 105, 214, 151 },
	}),
	win_ring = table.freeze({
		Id = "win_ring",
		Category = "WinEffect",
		DisplayName = "Sky Ring",
		Cost = 220,
		ColorRGB = { 126, 183, 255 },
	}),
	theme_foundry = table.freeze({
		Id = "theme_foundry",
		Category = "Theme",
		DisplayName = "Foundry",
		Cost = 0,
		ColorRGB = { 244, 166, 80 },
	}),
	theme_skyline = table.freeze({
		Id = "theme_skyline",
		Category = "Theme",
		DisplayName = "Skyline",
		Cost = 260,
		ColorRGB = { 91, 220, 239 },
	}),
	theme_neon = table.freeze({
		Id = "theme_neon",
		Category = "Theme",
		DisplayName = "Night Shift",
		Cost = 320,
		ColorRGB = { 198, 145, 255 },
	}),
}

return table.freeze({
	Items = table.freeze(items),
	Order = table.freeze({
		"trail_none",
		"trail_cyan",
		"trail_ember",
		"win_none",
		"win_burst",
		"win_ring",
		"theme_foundry",
		"theme_skyline",
		"theme_neon",
	}),
})
