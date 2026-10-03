return function()
	local ChallengeConfig = require("../src/shared/config/ChallengeConfig")
	local ChallengeRules = require("../src/shared/game/ChallengeRules")
	for _, familyId in ChallengeConfig.FamilyOrder do
		for _, difficulty in { 0, 0.25, 0.5, 0.75, 1 } do
			for variant = 1, ChallengeConfig.Families[familyId].VariantCount do
				local selection = {
					FamilyId = familyId,
					Variant = variant,
					Difficulty = difficulty,
					TimeLimitSeconds = 10 - 3 * difficulty,
				}
				local params = ChallengeRules.parameters(selection)
				assert(ChallengeRules.isSafe(selection, params), familyId .. " generated unsafe params")
			end
		end
	end
end
