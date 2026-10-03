--!strict

local familyOrder = {
	"precision_jump",
	"moving_platform",
	"dodge_gates",
	"narrow_path",
	"switch_dash",
	"disappearing_tiles",
	"moving_wall",
}

local families = {
	precision_jump = table.freeze({
		DisplayName = "Precision Jump",
		Hint = "Jump the gap. Land inside the marked bay.",
		VariantCount = 4,
		AccentRGB = { 255, 181, 76 },
	}),
	moving_platform = table.freeze({
		DisplayName = "Moving Platform",
		Hint = "Time the moving lift and cross.",
		VariantCount = 4,
		AccentRGB = { 91, 220, 239 },
	}),
	dodge_gates = table.freeze({
		DisplayName = "Dodge Gates",
		Hint = "Read the open lane and pass the gates.",
		VariantCount = 4,
		AccentRGB = { 255, 108, 116 },
	}),
	narrow_path = table.freeze({
		DisplayName = "Balance Run",
		Hint = "Stay on the lit path to the exit.",
		VariantCount = 4,
		AccentRGB = { 126, 226, 162 },
	}),
	switch_dash = table.freeze({
		DisplayName = "Switch Dash",
		Hint = "Hit the switch, then reach the exit.",
		VariantCount = 4,
		AccentRGB = { 198, 145, 255 },
	}),
	disappearing_tiles = table.freeze({
		DisplayName = "Fading Tiles",
		Hint = "Keep moving. Used tiles retract behind you.",
		VariantCount = 4,
		AccentRGB = { 255, 207, 92 },
	}),
	moving_wall = table.freeze({
		DisplayName = "Moving Wall",
		Hint = "Find the opening and get through.",
		VariantCount = 4,
		AccentRGB = { 99, 179, 255 },
	}),
}

return table.freeze({
	FamilyOrder = table.freeze(familyOrder),
	Families = table.freeze(families),
	TutorialOrder = table.freeze({ "precision_jump", "narrow_path", "switch_dash" }),
})
