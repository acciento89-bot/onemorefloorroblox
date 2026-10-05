local fs = require("@lune/fs")

local hud = fs.readFile("src/client/controllers/HudController.luau")

assert(
	not string.find(hud, "status.Visible = width < 620", 1, true),
	"desktop/tablet status must not be disabled by the compact-width predicate"
)
assert(
	string.find(hud, "status.Visible = true", 1, true),
	"responsive branches must explicitly keep the live status visible"
)
assert(
	string.find(hud, "quickRail.Position = UDim2.new(1, -18, 0, 18)", 1, true),
	"desktop/tablet quick actions must stay in the top-right safe zone"
)
assert(
	not string.find(hud, "quickRail.Position = UDim2.new(1, -266", 1, true),
	"quick actions must not float hundreds of pixels away from the right edge"
)
assert(
	string.find(hud, "quickRail.AnchorPoint = Vector2.new(1, 1)", 1, true),
	"compact quick actions need an explicit bottom-right safe-zone anchor"
)
assert(
	string.find(hud, "objective.Position = UDim2.new(0.5, 0, 1, -88)", 1, true),
	"desktop/tablet objective must remain centered and out of the character view"
)

return true
