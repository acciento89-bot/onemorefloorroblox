--!strict

local ReceiptRules = {}

function ReceiptRules.apply(
	profile: any,
	purchaseId: string,
	productKey: string,
	now: number,
	boostDuration: number?
): (boolean, string)
	if purchaseId == "" or profile.ProcessedReceipts[purchaseId] == true then
		return false, "AlreadyProcessed"
	end

	if productKey == "revive_1" then
		profile.ReviveCredits += 1
	elseif productKey == "skip_1" then
		profile.SkipTokens += 1
	elseif productKey == "coin_2x_30m" then
		local duration = math.max(1, math.floor(boostDuration or 1800))
		profile.CoinMultiplierUntil = math.max(profile.CoinMultiplierUntil, math.floor(now)) + duration
	else
		return false, "UnknownProduct"
	end

	profile.ProcessedReceipts[purchaseId] = true
	return true, "Granted"
end

return table.freeze(ReceiptRules)
