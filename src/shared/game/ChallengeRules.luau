--!strict

local ChallengeRules = {}

function ChallengeRules.parameters(selection: any): any
	local id = selection.FamilyId
	local d = math.clamp(selection.Difficulty or 0, 0, 1)
	local variant = math.max(1, math.floor(selection.Variant or 1))

	if id == "precision_jump" then
		return {
			Gap = 5.2 + d * 1.8,
			LandingWidth = 8 - d * 1.8,
			Lateral = ((variant - 1) % 3 - 1) * (1.4 + d),
		}
	elseif id == "moving_platform" then
		return {
			Travel = 8 + d * 5,
			Speed = 5 + d * 2.5,
			PlatformWidth = 7 - d,
		}
	elseif id == "dodge_gates" then
		return {
			GateCount = 3 + math.floor(d * 2),
			OpeningWidth = 9 - d * 2,
			CycleSeconds = 2.2 - d * 0.55,
		}
	elseif id == "narrow_path" then
		return {
			PathWidth = 5.5 - d * 1.4,
			SegmentCount = 4 + math.floor(d * 2),
			Turn = ((variant % 2 == 0) and 1 or -1) * (2 + d * 1.5),
		}
	elseif id == "switch_dash" then
		return {
			SwitchSide = if variant % 2 == 0 then 1 else -1,
			SwitchOffset = 8 + d * 2,
			GateDelay = 0.25,
		}
	elseif id == "disappearing_tiles" then
		return {
			TileCount = 6 + math.floor(d * 2),
			TileHoldSeconds = 1.45 - d * 0.45,
			TileWidth = 5.5 - d * 0.8,
		}
	elseif id == "moving_wall" then
		return {
			OpeningWidth = 9 - d * 2,
			WallSpeed = 5 + d * 2.4,
			Travel = 11 + d * 4,
		}
	elseif id == "laser_sweep" then
		return {
			SweepSpeed = 0.82 + d * 0.52,
			BeamLength = 18 + d * 2,
			BeamHeight = 1.35 + (variant % 2) * 0.08,
			Phase = (variant - 1) * (math.pi / 4),
		}
	end
	error("unsupported challenge family: " .. tostring(id))
end

function ChallengeRules.isSafe(selection: any, params: any): boolean
	local timeLimit = tonumber(selection.TimeLimitSeconds)
	if not timeLimit or timeLimit < 5 or timeLimit > 10 then
		return false
	end
	local id = selection.FamilyId
	if id == "precision_jump" then
		return params.Gap >= 5 and params.Gap <= 7.2 and params.LandingWidth >= 5.5
	elseif id == "moving_platform" then
		return params.Speed <= 8 and params.PlatformWidth >= 5.5
	elseif id == "dodge_gates" then
		return params.OpeningWidth >= 6.5 and params.CycleSeconds >= 1.5
	elseif id == "narrow_path" then
		return params.PathWidth >= 4 and params.SegmentCount <= 6
	elseif id == "switch_dash" then
		return math.abs(params.SwitchSide) == 1 and params.SwitchOffset <= 11
	elseif id == "disappearing_tiles" then
		return params.TileHoldSeconds >= 0.9 and params.TileWidth >= 4.5
	elseif id == "moving_wall" then
		return params.OpeningWidth >= 6.5 and params.WallSpeed <= 8
	elseif id == "laser_sweep" then
		return params.SweepSpeed >= 0.8
			and params.SweepSpeed <= 1.4
			and params.BeamLength >= 18
			and params.BeamLength <= 20.5
			and params.BeamHeight >= 1.3
			and params.BeamHeight <= 1.55
	end
	return false
end

return table.freeze(ChallengeRules)
