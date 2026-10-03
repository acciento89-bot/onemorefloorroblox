return function()
	local ProfileRules = require("../src/shared/profile/ProfileRules")
	local RetentionRules = require("../src/shared/profile/RetentionRules")

	local profile = ProfileRules.default()
	local first = RetentionRules.applyLogin(profile, 100)
	assert(first.Claimed == true and first.Reward == 10 and first.Streak == 1, "first daily reward")
	assert(profile.Coins == 10, "daily reward credited")

	local duplicate = RetentionRules.applyLogin(profile, 100)
	assert(duplicate.Claimed == false and duplicate.Reward == 0, "same-day login idempotent")
	assert(profile.Coins == 10, "same-day login no duplicate coins")

	local second = RetentionRules.applyLogin(profile, 101)
	assert(second.Claimed == true and second.Reward == 12 and second.Streak == 2, "daily streak increments")
	local reset = RetentionRules.applyLogin(profile, 104)
	assert(reset.Streak == 1 and reset.Reward == 10, "missed day resets streak")

	assert(RetentionRules.markFamilySeen(profile, "precision_jump") == true, "first family seen")
	assert(RetentionRules.markFamilySeen(profile, "precision_jump") == false, "seen family idempotent")

	local dailyCompleted = false
	for floor = 1, 5 do
		local result = RetentionRules.recordFloor(profile, 104, "precision_jump", floor)
		dailyCompleted = dailyCompleted or result.DailyCompleted
	end
	assert(dailyCompleted == true, "daily goal completes at target")
	assert(profile.DailyGoalClaimed == true, "daily goal claimed")
	local coinsAfterDaily = profile.Coins

	local repeatDaily = RetentionRules.recordFloor(profile, 104, "precision_jump", 6)
	assert(repeatDaily.DailyCompleted == false, "daily reward not duplicated")
	assert(profile.Coins == coinsAfterDaily, "no duplicate daily coins")

	local milestone = RetentionRules.recordFloor(profile, 104, "moving_platform", 10)
	assert(profile.Achievements.floor_10 == true, "floor milestone awarded")
	assert(#milestone.NewAchievements >= 1, "milestone reported")

	local beforeRepeat = profile.Coins
	RetentionRules.recordFloor(profile, 104, "moving_platform", 10)
	assert(profile.Coins == beforeRepeat, "milestone reward idempotent")
	for _ = 1, 3 do
		RetentionRules.recordFloor(profile, 104, "moving_platform", 11)
	end
	assert(profile.Achievements.master_moving_platform == true, "family mastery awarded")
end
