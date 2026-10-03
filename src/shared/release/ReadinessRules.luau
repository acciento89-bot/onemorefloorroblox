--!strict

local ReadinessRules = {}

function ReadinessRules.evaluate(platform: any, monetization: any?)
	local blockers = {}
	if (platform.UniverseId or 0) <= 0 then
		table.insert(blockers, { Code = "UNIVERSE_ID", Detail = "Roblox Universe ID is not configured." })
	end
	if (platform.DevelopmentPlaceId or 0) <= 0 then
		table.insert(blockers, { Code = "DEV_PLACE_ID", Detail = "Development place ID is not configured." })
	end
	if (platform.ProductionPlaceId or 0) <= 0 then
		table.insert(blockers, { Code = "PROD_PLACE_ID", Detail = "Production place ID is not configured." })
	end

	if monetization and type(monetization.Products) == "table" then
		for name, product in monetization.Products do
			if (product.ProductId or 0) <= 0 then
				table.insert(blockers, {
					Code = "PRODUCT_" .. string.upper(name),
					Detail = ("%s developer product ID is not configured."):format(name),
				})
			end
		end
	end

	return {
		ReadyForSandbox = #blockers == 0,
		Blockers = blockers,
	}
end

return table.freeze(ReadinessRules)
