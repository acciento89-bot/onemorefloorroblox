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
		FocusHeight = 2.4,
		Smoothness = 9,
		ProfileBlendSpeed = 6,
		Profiles = table.freeze({
			precision_jump = table.freeze({ Distance = 19.5, Height = 10.5, FocusLead = 5.0, FieldOfView = 70 }),
			moving_platform = table.freeze({ Distance = 20.0, Height = 11.0, FocusLead = 4.5, FieldOfView = 72 }),
			dodge_gates = table.freeze({ Distance = 19.0, Height = 11.5, FocusLead = 5.5, FieldOfView = 72 }),
			narrow_path = table.freeze({ Distance = 20.5, Height = 12.0, FocusLead = 6.0, FieldOfView = 69 }),
			switch_dash = table.freeze({ Distance = 19.5, Height = 11.0, FocusLead = 5.0, FieldOfView = 70 }),
			disappearing_tiles = table.freeze({ Distance = 20.5, Height = 12.0, FocusLead = 6.0, FieldOfView = 70 }),
			moving_wall = table.freeze({ Distance = 21.0, Height = 11.5, FocusLead = 5.5, FieldOfView = 72 }),
		}),
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
