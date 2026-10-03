return function()
	local ChallengeConfig = require("../src/shared/config/ChallengeConfig")
	local GameConfig = require("../src/shared/config/GameConfig")

	assert(GameConfig.Camera.ProfileBlendSpeed >= 4, "camera profiles must blend quickly enough for short floors")
	for _, familyId in ChallengeConfig.FamilyOrder do
		local profile = GameConfig.Camera.Profiles[familyId]
		assert(profile ~= nil, ("missing camera profile for %s"):format(familyId))
		assert(profile.Distance >= 16 and profile.Distance <= 24, familyId .. " camera distance out of range")
		assert(profile.Height >= 8 and profile.Height <= 14, familyId .. " camera height out of range")
		assert(profile.FocusLead >= 3 and profile.FocusLead <= 7, familyId .. " focus lead out of range")
		assert(profile.FieldOfView >= 64 and profile.FieldOfView <= 74, familyId .. " FOV out of range")
	end
end
