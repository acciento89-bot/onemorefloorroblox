local tests = {
	require("../tests/challenge-rules.spec"),
	require("../tests/floor-state-rules.spec"),
	require("../tests/selector-rules.spec"),
}

local passed = 0
for _, test in tests do
	test()
	passed += 1
end

print(("One More Floor: %d pure Luau tests passed"):format(passed))
