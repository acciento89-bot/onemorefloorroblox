--!strict

local RetentionRules = {}

RetentionRules.DAILY_GOAL_TARGET = 5
RetentionRules.DAILY_GOAL_REWARD = 25

function RetentionRules.dayNumber(timestamp: number): number
	return math.max(0, math.floor(timestamp / 86400))
end

function RetentionRules.applyLogin(profile: any, day: number): any
	local currentDay = math.max(0, math.floor(day))
	if profile.DailyGoalDay ~= currentDay then
		profile.DailyGoalDay = currentDay
		profile.DailyGoalProgress = 0
		profile.DailyGoalClaimed = false
	end

	if currentDay <= profile.LastDailyDay then
		return {
			Claimed = false,
			Reward = 0,
			Streak = profile.DailyStreak,
		}
	end

	if profile.LastDailyDay > 0 and currentDay == profile.LastDailyDay + 1 then
		profile.DailyStreak += 1
	else
		profile.DailyStreak = 1
	end
	profile.LastDailyDay = currentDay

	local reward = 8 + math.min(profile.DailyStreak, 7) * 2
	profile.Coins += reward
	return {
		Claimed = true,
		Reward = reward,
		Streak = profile.DailyStreak,
	}
end

local function awardAchievement(profile: any, id: string, reward: number, newAchievements: { string }): number
	if profile.Achievements[id] == true then
		return 0
	end
	profile.Achievements[id] = true
	table.insert(newAchievements, id)
	profile.Coins += reward
	return reward
end

function RetentionRules.markFamilySeen(profile: any, familyId: string): boolean
	if profile.SeenFamilies[familyId] == true then
		return false
	end
	profile.SeenFamilies[familyId] = true
	return true
end

function RetentionRules.recordFloor(profile: any, day: number, familyId: string, floorNumber: number): any
	local currentDay = math.max(0, math.floor(day))
	if profile.DailyGoalDay ~= currentDay then
		profile.DailyGoalDay = currentDay
		profile.DailyGoalProgress = 0
		profile.DailyGoalClaimed = false
	end

	profile.DailyGoalProgress += 1
	profile.FamilyWins[familyId] = (profile.FamilyWins[familyId] or 0) + 1

	local reward = 0
	local dailyCompleted = false
	if profile.DailyGoalProgress >= RetentionRules.DAILY_GOAL_TARGET and not profile.DailyGoalClaimed then
		profile.DailyGoalClaimed = true
		profile.Coins += RetentionRules.DAILY_GOAL_REWARD
		reward += RetentionRules.DAILY_GOAL_REWARD
		dailyCompleted = true
	end

	local achievements = {}
	if floorNumber >= 10 then
		reward += awardAchievement(profile, "floor_10", 15, achievements)
	end
	if floorNumber >= 25 then
		reward += awardAchievement(profile, "floor_25", 30, achievements)
	end
	if floorNumber >= 50 then
		reward += awardAchievement(profile, "floor_50", 60, achievements)
	end
	if profile.FamilyWins[familyId] >= 5 then
		reward += awardAchievement(profile, "master_" .. familyId, 12, achievements)
	end

	return {
		DailyCompleted = dailyCompleted,
		DailyProgress = profile.DailyGoalProgress,
		DailyTarget = RetentionRules.DAILY_GOAL_TARGET,
		Reward = reward,
		NewAchievements = achievements,
	}
end

return table.freeze(RetentionRules)
