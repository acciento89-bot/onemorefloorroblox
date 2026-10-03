--!strict

local MarketplaceService = game:GetService("MarketplaceService")
local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")

local shared = ReplicatedStorage:WaitForChild("OneMoreFloor"):WaitForChild("Shared")
local MonetizationConfig = require(shared.config.MonetizationConfig)
local ReceiptRules = require(shared.profile.ReceiptRules)

local FloorService = require(script.Parent.FloorService)
local ProfileService = require(script.Parent.ProfileService)

local MonetizationService = {}

local monetizationAction: RemoteEvent
local monetizationState: RemoteEvent
local monetizationQuery: RemoteFunction

local productsById: { [number]: any } = {}
for _, product in MonetizationConfig.Products do
	if product.ProductId > 0 then
		productsById[product.ProductId] = product
	end
end

local function stateFor(player: Player): any
	local profile = ProfileService.get(player)
	local products = {}
	for name, product in MonetizationConfig.Products do
		products[name] = {
			Key = product.Key,
			ProductId = product.ProductId,
			ExpectedRobux = product.ExpectedRobux,
			DisplayName = product.DisplayName,
			Description = product.Description,
			Configured = product.ProductId > 0,
		}
	end

	return {
		Products = products,
		ReviveCredits = if profile then profile.ReviveCredits else 0,
		SkipTokens = if profile then profile.SkipTokens else 0,
		CoinMultiplierUntil = if profile then profile.CoinMultiplierUntil else 0,
		Phase = player:GetAttribute("OMFPhase") or "Preparing",
	}
end

local function pushState(player: Player)
	if player.Parent ~= Players then
		return
	end
	local state = stateFor(player)
	player:SetAttribute("OMFReviveCredits", state.ReviveCredits)
	player:SetAttribute("OMFSkipTokens", state.SkipTokens)
	player:SetAttribute("OMFCoinMultiplierUntil", state.CoinMultiplierUntil)
	monetizationState:FireClient(player, state)
end

local function onAction(player: Player, action: any)
	if type(action) ~= "string" then
		return
	end

	local ok = false
	local reason = "UnknownAction"
	if action == "UseRevive" then
		ok, reason = FloorService.useRevive(player)
	elseif action == "UseSkip" then
		ok, reason = FloorService.useSkip(player)
	end

	pushState(player)
	monetizationState:FireClient(player, {
		Event = "ActionResult",
		Action = action,
		Success = ok,
		Reason = reason,
	})
end

local function processReceipt(receiptInfo: any): Enum.ProductPurchaseDecision
	local player = Players:GetPlayerByUserId(receiptInfo.PlayerId)
	if not player then
		return Enum.ProductPurchaseDecision.NotProcessedYet
	end

	local profile = ProfileService.get(player)
	if not profile then
		return Enum.ProductPurchaseDecision.NotProcessedYet
	end

	local product = productsById[receiptInfo.ProductId]
	if not product then
		warn(("[One More Floor] Unknown developer product %s"):format(tostring(receiptInfo.ProductId)))
		return Enum.ProductPurchaseDecision.NotProcessedYet
	end

	local purchaseId = tostring(receiptInfo.PurchaseId or "")
	if profile.ProcessedReceipts[purchaseId] ~= true then
		local granted, reason = ReceiptRules.apply(profile, purchaseId, product.Key, os.time(), product.DurationSeconds)
		if not granted and reason ~= "AlreadyProcessed" then
			warn(("[One More Floor] Receipt grant failed: %s"):format(reason))
			return Enum.ProductPurchaseDecision.NotProcessedYet
		end
		ProfileService.markDirty(player)
	end

	if not ProfileService.save(player) then
		return Enum.ProductPurchaseDecision.NotProcessedYet
	end

	FloorService.refreshProfile(player)
	pushState(player)
	return Enum.ProductPurchaseDecision.PurchaseGranted
end

function MonetizationService.start()
	local remotes = ReplicatedStorage:WaitForChild("OneMoreFloorRemotes")
	monetizationAction = remotes:WaitForChild("MonetizationAction") :: RemoteEvent
	monetizationState = remotes:WaitForChild("MonetizationState") :: RemoteEvent
	monetizationQuery = remotes:WaitForChild("MonetizationQuery") :: RemoteFunction

	monetizationAction.OnServerEvent:Connect(onAction)
	monetizationQuery.OnServerInvoke = function(player: Player)
		return stateFor(player)
	end
	MarketplaceService.ProcessReceipt = processReceipt

	Players.PlayerAdded:Connect(function(player)
		task.delay(1, function()
			if player.Parent == Players then
				pushState(player)
			end
		end)
	end)
	for _, player in Players:GetPlayers() do
		task.defer(pushState, player)
	end
end

return MonetizationService
