--!strict

return table.freeze({
	Products = table.freeze({
		Revive = table.freeze({
			Key = "revive_1",
			ProductId = 3716235872,
			ExpectedRobux = 15,
			DisplayName = "Revive",
			Description = "Continue the current floor. The run becomes assisted.",
		}),
		Skip = table.freeze({
			Key = "skip_1",
			ProductId = 3716236216,
			ExpectedRobux = 20,
			DisplayName = "Skip Token",
			Description = "Skip one floor. The run becomes assisted.",
		}),
		CoinBoost = table.freeze({
			Key = "coin_2x_30m",
			ProductId = 3716236586,
			ExpectedRobux = 49,
			DisplayName = "2× Coins · 30 min",
			Description = "Doubles normal floor and streak coin earnings only.",
			DurationSeconds = 30 * 60,
		}),
	}),
})
