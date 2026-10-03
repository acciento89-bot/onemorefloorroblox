--!strict

return table.freeze({
	DisplayName = "One More Floor",
	Floor = table.freeze({
		TargetSecondsMin = 5,
		TargetSecondsMax = 10,
		TransitionSeconds = 0.85,
		RoomWidth = 32,
		RoomDepth = 34,
		RoomHeight = 14,
		FloorRise = 16,
		KillPlaneOffset = -16,
	}),
	Difficulty = table.freeze({
		EarlyEnd = 10,
		MidEnd = 25,
		MaximumAlpha = 1,
	}),
	Movement = table.freeze({
		WalkSpeed = 16,
		JumpPower = 50,
	}),
	Camera = table.freeze({
		FieldOfView = 68,
		Distance = 18,
		Height = 10,
		FocusLead = 4,
		Smoothness = 9,
	}),
	Economy = table.freeze({
		FloorCoins = 3,
		StreakBonusEvery = 5,
		StreakBonusCoins = 8,
	}),
	Limits = table.freeze({
		MaxActiveRoomParts = 180,
		MaxActionRequestsPerSecond = 14,
	}),
})
