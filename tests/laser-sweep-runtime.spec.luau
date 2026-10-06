local fs = require("@lune/fs")
local ChallengeConfig = require("../src/shared/config/ChallengeConfig")
local ChallengeRules = require("../src/shared/game/ChallengeRules")

return function()
	local family = ChallengeConfig.Families.laser_sweep
	assert(family ~= nil, "laser_sweep family must be registered")
	assert(family.VariantCount >= 4, "laser_sweep needs multiple variants")

	for variant = 1, family.VariantCount do
		for _, difficulty in { 0, 0.25, 0.5, 0.75, 1 } do
			local selection = {
				FamilyId = "laser_sweep",
				Variant = variant,
				Difficulty = difficulty,
				TimeLimitSeconds = 8,
			}
			local params = ChallengeRules.parameters(selection)
			assert(ChallengeRules.isSafe(selection, params), "laser_sweep parameters left the safe envelope")
		end
	end

	local factory = fs.readFile("src/server/services/ChallengeFactory.luau")
	local floorService = fs.readFile("src/server/services/FloorService.luau")
	assert(
		string.find(factory, 'elseif selection.FamilyId == "laser_sweep"', 1, true),
		"laser_sweep must build a real runtime challenge"
	)
	assert(string.find(factory, "Spinners = {}", 1, true), "challenge runtime must expose spinner collection")
	assert(
		string.find(floorService, "for _, spinner in course.Runtime.Spinners do", 1, true),
		"laser sweep spinner must update during active floors"
	)
	assert(
		string.find(floorService, "spinner.BaseCFrame * CFrame.Angles(0, angle, 0)", 1, true),
		"laser sweep must rotate around its authored hub"
	)
end
