--!strict

local SelectorRules = {}

local MODULUS = 4294967296
local MULTIPLIER = 1664525
local INCREMENT = 1013904223

local function nextSeed(seed: number): number
	return (math.floor(seed) * MULTIPLIER + INCREMENT) % MODULUS
end

function SelectorRules.difficultyAlpha(floorNumber: number): number
	local floor = math.max(1, math.floor(floorNumber))
	if floor <= 3 then
		return 0
	elseif floor <= 10 then
		return (floor - 3) / 14
	elseif floor <= 25 then
		return 0.5 + (floor - 10) / 50
	end
	return math.min(1, 0.8 + (floor - 25) / 75)
end

local function recentlyUsed(history: { any }, familyId: string, variant: number): boolean
	for index = math.max(1, #history - 3), #history do
		local item = history[index]
		if item and item.FamilyId == familyId and item.Variant == variant then
			return true
		end
	end
	return false
end

function SelectorRules.select(
	seed: number,
	floorNumber: number,
	history: { any },
	familyOrder: { string },
	families: { [string]: any },
	tutorialOrder: { string }
)
	local floor = math.max(1, math.floor(floorNumber))
	local currentSeed = nextSeed(seed)
	local familyId: string

	if floor <= #tutorialOrder then
		familyId = tutorialOrder[floor]
	else
		local candidates = {}
		local last = history[#history]
		for _, id in familyOrder do
			if not last or id ~= last.FamilyId then
				table.insert(candidates, id)
			end
		end
		familyId = candidates[(currentSeed % #candidates) + 1]
	end

	local family = assert(families[familyId], "unknown challenge family")
	currentSeed = nextSeed(currentSeed)
	local variant = (currentSeed % family.VariantCount) + 1

	if floor > #tutorialOrder and recentlyUsed(history, familyId, variant) then
		for offset = 1, family.VariantCount - 1 do
			local candidate = ((variant - 1 + offset) % family.VariantCount) + 1
			if not recentlyUsed(history, familyId, candidate) then
				variant = candidate
				break
			end
		end
	end

	local difficulty = SelectorRules.difficultyAlpha(floor)
	return {
		Floor = floor,
		FamilyId = familyId,
		Variant = variant,
		Difficulty = difficulty,
		TimeLimitSeconds = 10 - 3 * difficulty,
	},
		currentSeed
end

return table.freeze(SelectorRules)
