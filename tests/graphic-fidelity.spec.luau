local fs = require("@lune/fs")

return function()
	local world = fs.readFile("src/server/services/WorldService.luau")
	local art = fs.readFile("docs/ART-DIRECTION.md")

	assert(string.find(world, 'cityRing.Name = "TowerDepthRing"', 1, true), "city depth layer missing")
	assert(string.find(world, '"FacadeGlassColumn"', 1, true), "glass facade detail missing")
	assert(string.find(world, '"FacadeLightBand"', 1, true), "facade light bands missing")
	assert(string.find(world, '"SunGlowHalo"', 1, true), "sun glow layer missing")
	assert(string.find(world, 'cloudRing.Name = "CloudRing"', 1, true), "cloud layers missing")
	assert(string.find(art, "tower-city identity", 1, true), "tower-city identity missing")
end
