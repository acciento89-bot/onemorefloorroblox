return function()
	local CosmeticConfig = require("../src/shared/config/CosmeticConfig")
	local ProfileRules = require("../src/shared/profile/ProfileRules")

	local seen = {}
	local categoryCount = {
		Trail = 0,
		WinEffect = 0,
		Theme = 0,
	}

	for _, id in CosmeticConfig.Order do
		assert(not seen[id], "duplicate cosmetic id: " .. id)
		seen[id] = true
		local item = CosmeticConfig.Items[id]
		assert(item ~= nil, "ordered cosmetic missing: " .. id)
		assert(item.Id == id, "cosmetic id mismatch")
		assert(categoryCount[item.Category] ~= nil, "unsupported cosmetic category")
		assert(type(item.Cost) == "number" and item.Cost >= 0, "invalid cosmetic cost")
		assert(type(item.DisplayName) == "string" and #item.DisplayName > 0, "missing cosmetic name")
		assert(type(item.ColorRGB) == "table" and #item.ColorRGB == 3, "invalid cosmetic color")
		categoryCount[item.Category] += 1
	end

	assert(categoryCount.Trail >= 3, "trail launch variety")
	assert(categoryCount.WinEffect >= 3, "win effect launch variety")
	assert(categoryCount.Theme >= 3, "theme launch variety")

	local profile = ProfileRules.default()
	for _, equippedId in
		{
			profile.Equipped.Trail,
			profile.Equipped.WinEffect,
			profile.Equipped.Theme,
		}
	do
		assert(CosmeticConfig.Items[equippedId] ~= nil, "default equipped cosmetic missing from catalog")
		assert(profile.OwnedCosmetics[equippedId] == true, "default equipped cosmetic must be owned")
	end
end
