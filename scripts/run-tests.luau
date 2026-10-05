local tests = {
	require("../tests/menu-visibility-contract.spec"),
	require("../tests/concept-showcase-ui.spec"),
	require("../tests/concept-fidelity.spec"),
	require("../tests/visual-polish.spec"),
	require("../tests/camera-config.spec"),
	require("../tests/challenge-rules.spec"),
	require("../tests/cosmetic-config.spec"),
	require("../tests/floor-state-rules.spec"),
	require("../tests/profile-rules.spec"),
	require("../tests/receipt-rules.spec"),
	require("../tests/retention-rules.spec"),
	require("../tests/security-rules.spec"),
	require("../tests/selector-rules.spec"),
}

local passed = 0
for _, test in tests do
	test()
	passed += 1
end

print(("One More Floor: %d pure Luau tests passed"):format(passed))
