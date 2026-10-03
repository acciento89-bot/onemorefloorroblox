return function()
	local ChallengeConfig = require("../src/shared/config/ChallengeConfig")
	local ChallengeRules = require("../src/shared/game/ChallengeRules")
	local SelectorRules = require("../src/shared/game/SelectorRules")

	for seedIndex = 1, 10 do
		local seed = seedIndex * 7919
		local history = {}
		for floor = 1, 100 do
			local selection
			selection, seed = SelectorRules.select(
				seed,
				floor,
				history,
				ChallengeConfig.FamilyOrder,
				ChallengeConfig.Families,
				ChallengeConfig.TutorialOrder
			)
			local params = ChallengeRules.parameters(selection)
			assert(ChallengeRules.isSafe(selection, params), "unsafe selected floor")
			if selection.FamilyId == "precision_jump" then
				assert(params.Gap <= 7.2, "precision jump gap exceeded safe launch envelope")
			end
			if floor <= 3 then
				assert(selection.FamilyId == ChallengeConfig.TutorialOrder[floor], "tutorial order changed")
			elseif #history > 0 then
				assert(selection.FamilyId ~= history[#history].FamilyId, "consecutive family repeat")
			end
			for index = math.max(1, #history - 3), #history do
				local previous = history[index]
				if previous and previous.FamilyId == selection.FamilyId then
					assert(
						previous.Variant ~= selection.Variant,
						"recent family variant repeated inside four-floor window"
					)
				end
			end
			table.insert(history, selection)
		end
	end
end
