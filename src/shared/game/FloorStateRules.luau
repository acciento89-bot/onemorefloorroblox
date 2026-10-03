--!strict

local FloorStateRules = {}

FloorStateRules.State = table.freeze({
	Preparing = "Preparing",
	Active = "Active",
	Success = "Success",
	Failed = "Failed",
	Transition = "Transition",
})

local transitions = {
	Preparing = { Active = true },
	Active = { Success = true, Failed = true },
	Success = { Transition = true },
	Failed = { Preparing = true },
	Transition = { Preparing = true },
}

function FloorStateRules.canTransition(fromState: string, toState: string): boolean
	local allowed = transitions[fromState]
	return allowed ~= nil and allowed[toState] == true
end

return table.freeze(FloorStateRules)
