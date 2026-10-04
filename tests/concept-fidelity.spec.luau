local fs = require("@lune/fs")

return function()
	local hud = fs.readFile("src/client/controllers/HudController.luau")
	local world = fs.readFile("src/server/services/WorldService.luau")
	local cosmetics = fs.readFile("src/client/controllers/CosmeticController.luau")
	local monetization = fs.readFile("src/client/controllers/MonetizationController.luau")

	assert(string.find(hud, 'logo.Name = "OneMoreFloorLogo"', 1, true), "HUD needs a branded One More Floor logo")
	assert(
		string.find(hud, 'floorProgress.Name = "FloorProgress"', 1, true),
		"HUD needs a concept-style floor progress strip"
	)
	assert(
		string.find(hud, 'quickRail.Name = "ConceptQuickRail"', 1, true),
		"HUD needs compact boost/style concept actions"
	)
	assert(string.find(world, 'crown.Name = "HeroTowerCrown"', 1, true), "tower needs a hero crown landmark")
	assert(string.find(world, '"SkySunDisc"', 1, true), "world needs a warm sky focal point")
	assert(string.find(world, 'cloudRing.Name = "CloudRing"', 1, true), "tower needs layered cloud depth")
	assert(
		string.find(cosmetics, "concept launcher owns desktop/tablet", 1, true),
		"style legacy launcher must yield to concept rail"
	)
	assert(
		string.find(monetization, "concept launcher owns desktop/tablet", 1, true),
		"boost legacy launcher must yield to concept rail"
	)
end
