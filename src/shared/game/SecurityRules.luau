--!strict

local SecurityRules = {}

SecurityRules.MIN_COMPLETION_SECONDS = 0.55
SecurityRules.MAX_EXIT_DISTANCE = 7.5
SecurityRules.MAX_VERTICAL_DEVIATION = 12
SecurityRules.MAX_REASONABLE_SPEED = 85
SecurityRules.MAX_SAMPLE_STEP = 14

local function finite(value: number): boolean
	return value == value and value ~= math.huge and value ~= -math.huge
end

function SecurityRules.isFiniteVector(value: any): boolean
	return typeof(value) == "Vector3" and finite(value.X) and finite(value.Y) and finite(value.Z)
end

function SecurityRules.validateMovementSample(
	previousPosition: Vector3,
	currentPosition: Vector3,
	deltaSeconds: number
): boolean
	if not SecurityRules.isFiniteVector(previousPosition) or not SecurityRules.isFiniteVector(currentPosition) then
		return false
	end
	if not finite(deltaSeconds) or deltaSeconds <= 0 then
		return false
	end
	if deltaSeconds > 2 then
		-- A long server hitch is not reliable evidence of a client teleport.
		return true
	end
	local distance = (currentPosition - previousPosition).Magnitude
	return distance / deltaSeconds <= SecurityRules.MAX_REASONABLE_SPEED
end

function SecurityRules.validateCompletion(
	startPosition: Vector3,
	currentPosition: Vector3,
	exitPosition: Vector3,
	floorY: number,
	elapsedSeconds: number
): (boolean, string)
	if
		not SecurityRules.isFiniteVector(startPosition)
		or not SecurityRules.isFiniteVector(currentPosition)
		or not SecurityRules.isFiniteVector(exitPosition)
	then
		return false, "NonFinite"
	end

	if not finite(elapsedSeconds) or elapsedSeconds < SecurityRules.MIN_COMPLETION_SECONDS then
		return false, "TooFast"
	end

	if (currentPosition - exitPosition).Magnitude > SecurityRules.MAX_EXIT_DISTANCE then
		return false, "ExitDistance"
	end

	if math.abs(currentPosition.Y - floorY) > SecurityRules.MAX_VERTICAL_DEVIATION then
		return false, "VerticalBounds"
	end

	local travelled = (currentPosition - startPosition).Magnitude
	if travelled / math.max(elapsedSeconds, 0.001) > SecurityRules.MAX_REASONABLE_SPEED then
		return false, "ImpossibleSpeed"
	end

	return true, "Valid"
end

return table.freeze(SecurityRules)
