local fs = require("@lune/fs")

return function()
	local source = fs.readFile("src/client/controllers/VisualPolishController.luau")

	assert(
		string.find(source, "local base = instance.BackgroundColor3", 1, true),
		"visual polish must preserve authored semantic button colors"
	)
	assert(
		not string.find(source, "local function buttonColor", 1, true),
		"global name-based button recoloring must stay removed"
	)
	assert(
		not string.find(source, "instance.BackgroundColor3 = accent", 1, true),
		"visual polish must not repaint every button with one accent"
	)
end
