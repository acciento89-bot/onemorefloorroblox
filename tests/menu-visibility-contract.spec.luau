local fs = require("@lune/fs")

return function()
	local hud = fs.readFile("src/client/controllers/HudController.luau")
	local art = fs.readFile("docs/ART-DIRECTION.md")

	assert(string.find(hud, "conceptShowcase.Visible = false", 1, true), "concept montage must stay hidden in gameplay")
	assert(
		not string.find(hud, "conceptShowcase.Visible = width >= 620", 1, true),
		"desktop must not force the concept montage visible"
	)
	assert(string.find(hud, 'dailyPanel.Name = "DailyRewardsPanel"', 1, true), "Daily needs a dedicated modal panel")
	assert(string.find(hud, "dailyPanel.Visible = false", 1, true), "Daily modal must be hidden by default")
	assert(string.find(hud, 'dailyQuick.Name = "DailyQuick"', 1, true), "HUD needs a compact Daily launcher")
	assert(string.find(hud, "setDailyVisible(true)", 1, true), "Daily launcher must open only the Daily panel")
	assert(string.find(hud, "setDailyVisible(false)", 1, true), "Daily modal needs explicit close paths")
	assert(string.find(hud, "resultPanel.Visible = false", 1, true), "failure result must remain event-driven")
	assert(
		string.find(art, "feature montage", 1, true),
		"art direction must define the concept image as a feature montage"
	)
end
