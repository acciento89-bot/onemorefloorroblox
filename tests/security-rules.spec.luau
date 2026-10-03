return function()
	local roblox = require("@lune/roblox")
	local Vector3 = roblox.Vector3
	local SecurityRules = require("../src/shared/game/SecurityRules")

	local start = Vector3.new(0, 0, -12)
	local exit = Vector3.new(0, 0, 12)
	local current = Vector3.new(0, 0, 11.5)

	local valid, reason = SecurityRules.validateCompletion(start, current, exit, 0, 2)
	assert(valid == true and reason == "Valid", "valid completion accepted")

	local tooFast, tooFastReason = SecurityRules.validateCompletion(start, current, exit, 0, 0.1)
	assert(tooFast == false and tooFastReason == "TooFast", "too-fast completion rejected")

	local far, farReason = SecurityRules.validateCompletion(start, Vector3.new(20, 0, 0), exit, 0, 2)
	assert(far == false and farReason == "ExitDistance", "far completion rejected")

	local impossible, impossibleReason = SecurityRules.validateCompletion(Vector3.new(-100, 0, 12), current, exit, 0, 1)
	assert(impossible == false and impossibleReason == "ImpossibleSpeed", "impossible speed rejected")

	assert(
		SecurityRules.validateMovementSample(Vector3.zero, Vector3.new(10, 0, 0), 0.2) == true,
		"reasonable sample accepted"
	)
	assert(
		SecurityRules.validateMovementSample(Vector3.zero, Vector3.new(20, 0, 0), 0.1) == false,
		"teleport sample rejected"
	)

	local nanVector = Vector3.new(0 / 0, 0, 0)
	assert(SecurityRules.isFiniteVector(nanVector) == false, "NaN vector rejected")
end
