--!strict

local GuiService = game:GetService("GuiService")
local MarketplaceService = game:GetService("MarketplaceService")
local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local Workspace = game:GetService("Workspace")

local player = Players.LocalPlayer
local playerGui = player:WaitForChild("PlayerGui")
local remotes = ReplicatedStorage:WaitForChild("OneMoreFloorRemotes")
local actionRemote = remotes:WaitForChild("MonetizationAction") :: RemoteEvent
local stateRemote = remotes:WaitForChild("MonetizationState") :: RemoteEvent
local queryRemote = remotes:WaitForChild("MonetizationQuery") :: RemoteFunction

local MonetizationController = {}

local COLORS = {
	Foundation = Color3.fromRGB(19, 26, 36),
	Surface = Color3.fromRGB(31, 41, 54),
	Raised = Color3.fromRGB(42, 53, 68),
	Text = Color3.fromRGB(244, 247, 251),
	Muted = Color3.fromRGB(164, 178, 194),
	Warm = Color3.fromRGB(244, 166, 80),
	Cyan = Color3.fromRGB(91, 220, 239),
	Green = Color3.fromRGB(105, 214, 151),
}

type ProductRow = {
	Action: TextButton,
	Status: TextLabel,
}

local currentState: any = nil
local panel: Frame
local shopButton: TextButton
local quickRevive: TextButton
local rows: { [string]: ProductRow } = {}
local pendingAutoRevive = false

local function corner(parent: Instance, radius: number)
	local item = Instance.new("UICorner")
	item.CornerRadius = UDim.new(0, radius)
	item.Parent = parent
end

local function stroke(parent: Instance, color: Color3, transparency: number)
	local item = Instance.new("UIStroke")
	item.Color = color
	item.Transparency = transparency
	item.Thickness = 1
	item.Parent = parent
end

local function label(
	parent: Instance,
	text: string,
	size: UDim2,
	position: UDim2,
	textSize: number,
	color: Color3,
	font: Enum.Font?
): TextLabel
	local item = Instance.new("TextLabel")
	item.Size = size
	item.Position = position
	item.BackgroundTransparency = 1
	item.Font = font or Enum.Font.GothamMedium
	item.Text = text
	item.TextSize = textSize
	item.TextColor3 = color
	item.TextXAlignment = Enum.TextXAlignment.Left
	item.TextYAlignment = Enum.TextYAlignment.Center
	item.Parent = parent
	return item
end

local function button(parent: Instance, text: string, size: UDim2, position: UDim2): TextButton
	local item = Instance.new("TextButton")
	item.Size = size
	item.Position = position
	item.BackgroundColor3 = COLORS.Cyan
	item.BorderSizePixel = 0
	item.Font = Enum.Font.GothamBlack
	item.Text = text
	item.TextSize = 13
	item.TextColor3 = COLORS.Foundation
	item.Selectable = true
	item.Parent = parent
	corner(item, 11)
	return item
end

local function timeText(untilTimestamp: number): string
	local remaining = math.max(0, untilTimestamp - os.time())
	if remaining <= 0 then
		return "INACTIVE"
	end
	return ("%02d:%02d LEFT"):format(math.floor(remaining / 60), remaining % 60)
end

local function phase(): string
	return tostring(player:GetAttribute("OMFPhase") or (currentState and currentState.Phase) or "Preparing")
end

local function product(name: string): any
	return currentState and currentState.Products and currentState.Products[name] or nil
end

local function prompt(name: string)
	local info = product(name)
	if not info or info.Configured ~= true or (info.ProductId or 0) <= 0 then
		return
	end
	MarketplaceService:PromptProductPurchase(player, info.ProductId)
end

local function updateRow(name: string, owned: number?, boostUntil: number?)
	local row = rows[name]
	local info = product(name)
	if not row then
		return
	end
	if not info or info.Configured ~= true then
		row.Action.Active = false
		row.Action.AutoButtonColor = false
		row.Action.Text = "NOT LIVE"
		row.Action.BackgroundColor3 = COLORS.Raised
		row.Action.TextColor3 = COLORS.Muted
		row.Status.Text = "Awaiting Roblox product configuration"
		return
	end

	row.Action.Active = true
	row.Action.AutoButtonColor = true
	row.Action.BackgroundColor3 = COLORS.Cyan
	row.Action.TextColor3 = COLORS.Foundation

	if name == "Revive" and (owned or 0) > 0 then
		row.Action.Text = if phase() == "Failed"
			then ("USE x%d"):format(owned :: number)
			else ("OWNED x%d"):format(owned :: number)
		row.Status.Text = "Continues the current floor · assisted run"
	elseif name == "Skip" and (owned or 0) > 0 then
		row.Action.Text = ("USE x%d"):format(owned :: number)
		row.Status.Text = "Skips exactly one floor · assisted run"
	elseif name == "CoinBoost" and (boostUntil or 0) > os.time() then
		row.Action.Text = ("EXTEND · %d R$"):format(info.ExpectedRobux)
		row.Status.Text = "2× floor + streak coins · " .. timeText(boostUntil or 0)
	else
		row.Action.Text = ("BUY · %d R$"):format(info.ExpectedRobux)
		row.Status.Text = tostring(info.Description or "")
	end
end

local function update()
	if not currentState then
		return
	end
	local reviveCredits = tonumber(currentState.ReviveCredits) or tonumber(player:GetAttribute("OMFReviveCredits")) or 0
	local skipTokens = tonumber(currentState.SkipTokens) or tonumber(player:GetAttribute("OMFSkipTokens")) or 0
	local boostUntil = tonumber(currentState.CoinMultiplierUntil)
		or tonumber(player:GetAttribute("OMFCoinMultiplierUntil"))
		or 0

	updateRow("Revive", reviveCredits, nil)
	updateRow("Skip", skipTokens, nil)
	updateRow("CoinBoost", nil, boostUntil)

	local reviveInfo = product("Revive")
	local failed = phase() == "Failed"
	quickRevive.Visible = failed
	if failed then
		if reviveCredits > 0 then
			quickRevive.Text = ("REVIVE  x%d"):format(reviveCredits)
			quickRevive.BackgroundColor3 = COLORS.Green
			quickRevive.Active = true
		elseif reviveInfo and reviveInfo.Configured == true then
			quickRevive.Text = ("REVIVE  ·  %d R$"):format(reviveInfo.ExpectedRobux)
			quickRevive.BackgroundColor3 = COLORS.Warm
			quickRevive.Active = true
		else
			quickRevive.Text = "REVIVE · NOT LIVE"
			quickRevive.BackgroundColor3 = COLORS.Raised
			quickRevive.Active = false
		end
	end

	if pendingAutoRevive and reviveCredits > 0 and failed then
		pendingAutoRevive = false
		actionRemote:FireServer("UseRevive")
	end
end

local function refresh()
	local ok, state = pcall(function()
		return queryRemote:InvokeServer()
	end)
	if ok and type(state) == "table" then
		currentState = state
		update()
	end
end

local function makeRow(parent: Instance, order: number, key: string, title: string, detail: string)
	local row = Instance.new("Frame")
	row.Name = key
	row.Size = UDim2.new(1, -20, 0, 72)
	row.Position = UDim2.fromOffset(10, 58 + (order - 1) * 80)
	row.BackgroundColor3 = COLORS.Surface
	row.BorderSizePixel = 0
	row.Parent = parent
	corner(row, 13)
	stroke(row, COLORS.Cyan, 0.78)

	label(row, title, UDim2.new(1, -150, 0, 22), UDim2.fromOffset(12, 8), 13, COLORS.Text, Enum.Font.GothamBold)
	local status = label(row, detail, UDim2.new(1, -150, 0, 32), UDim2.fromOffset(12, 31), 10, COLORS.Muted)
	status.TextWrapped = true
	local action = button(row, "NOT LIVE", UDim2.fromOffset(126, 44), UDim2.new(1, -138, 0.5, -22))
	rows[key] = { Action = action, Status = status }

	action.Activated:Connect(function()
		local credits = if key == "Revive"
			then tonumber(currentState and currentState.ReviveCredits) or 0
			else tonumber(currentState and currentState.SkipTokens) or 0
		if key == "Revive" and credits > 0 and phase() == "Failed" then
			actionRemote:FireServer("UseRevive")
		elseif key == "Skip" and credits > 0 then
			actionRemote:FireServer("UseSkip")
		else
			if key == "Revive" and phase() == "Failed" then
				pendingAutoRevive = true
			end
			prompt(key)
		end
	end)
end

local function build()
	local old = playerGui:FindFirstChild("OneMoreFloorMonetization")
	if old then
		old:Destroy()
	end

	local screen = Instance.new("ScreenGui")
	screen.Name = "OneMoreFloorMonetization"
	screen.ResetOnSpawn = false
	screen.DisplayOrder = 35
	screen.ZIndexBehavior = Enum.ZIndexBehavior.Sibling
	screen.Parent = playerGui

	shopButton = button(screen, "BOOSTS", UDim2.fromOffset(112, 48), UDim2.new(1, -128, 1, -66))
	shopButton.AnchorPoint = Vector2.new(0, 0)
	shopButton.BackgroundColor3 = COLORS.Foundation
	shopButton.TextColor3 = COLORS.Cyan
	stroke(shopButton, COLORS.Cyan, 0.38)

	panel = Instance.new("Frame")
	panel.Name = "Panel"
	panel.AnchorPoint = Vector2.new(1, 1)
	panel.Position = UDim2.new(1, -16, 1, -124)
	panel.Size = UDim2.fromOffset(410, 316)
	panel.BackgroundColor3 = COLORS.Foundation
	panel.BorderSizePixel = 0
	panel.Visible = false
	panel.Parent = screen
	corner(panel, 18)
	stroke(panel, COLORS.Cyan, 0.4)

	label(panel, "BOOSTS", UDim2.new(1, -70, 0, 34), UDim2.fromOffset(14, 10), 18, COLORS.Text, Enum.Font.GothamBlack)
	local close = button(panel, "×", UDim2.fromOffset(40, 36), UDim2.new(1, -50, 0, 8))
	close.BackgroundColor3 = COLORS.Raised
	close.TextColor3 = COLORS.Text
	close.TextSize = 20

	makeRow(panel, 1, "Revive", "REVIVE", "Continue current floor · assisted run")
	makeRow(panel, 2, "Skip", "SKIP TOKEN", "Advance one floor · assisted run")
	makeRow(panel, 3, "CoinBoost", "2× COINS · 30 MIN", "Doubles floor + streak coins only")

	quickRevive = button(screen, "REVIVE", UDim2.fromOffset(220, 50), UDim2.new(0.5, -110, 0.5, 138))
	quickRevive.Visible = false
	quickRevive.ZIndex = 50

	shopButton.Activated:Connect(function()
		panel.Visible = not panel.Visible
		if panel.Visible then
			refresh()
			GuiService.SelectedObject = rows.Revive.Action
		elseif GuiService.SelectedObject and GuiService.SelectedObject:IsDescendantOf(panel) then
			GuiService.SelectedObject = nil
		end
	end)
	close.Activated:Connect(function()
		panel.Visible = false
	end)
	quickRevive.Activated:Connect(function()
		local credits = tonumber(currentState and currentState.ReviveCredits)
			or tonumber(player:GetAttribute("OMFReviveCredits"))
			or 0
		if credits > 0 then
			actionRemote:FireServer("UseRevive")
		else
			pendingAutoRevive = true
			prompt("Revive")
		end
	end)

	local function responsive()
		local camera = Workspace.CurrentCamera
		if not camera then
			return
		end
		local width = camera.ViewportSize.X
		if width < 620 then
			panel.Size = UDim2.new(1, -24, 0, 316)
			panel.Position = UDim2.new(1, -12, 1, -118)
			shopButton.Position = UDim2.new(1, -124, 1, -60)
			quickRevive.Position = UDim2.new(0.5, -110, 0.5, 150)
		else
			panel.Size = UDim2.fromOffset(410, 316)
			panel.Position = UDim2.new(1, -16, 1, -124)
			shopButton.Position = UDim2.new(1, -128, 1, -66)
			quickRevive.Position = UDim2.new(0.5, -110, 0.5, 138)
		end
	end
	responsive()
	local camera = Workspace.CurrentCamera
	if camera then
		camera:GetPropertyChangedSignal("ViewportSize"):Connect(responsive)
	end
end

function MonetizationController.start()
	build()
	stateRemote.OnClientEvent:Connect(function(state)
		if type(state) == "table" and state.Products ~= nil then
			currentState = state
			update()
		else
			task.defer(refresh)
		end
	end)
	for _, attribute in { "OMFPhase", "OMFReviveCredits", "OMFSkipTokens", "OMFCoinMultiplierUntil" } do
		player:GetAttributeChangedSignal(attribute):Connect(function()
			if currentState then
				if attribute == "OMFReviveCredits" then
					currentState.ReviveCredits = player:GetAttribute(attribute) or 0
				elseif attribute == "OMFSkipTokens" then
					currentState.SkipTokens = player:GetAttribute(attribute) or 0
				elseif attribute == "OMFCoinMultiplierUntil" then
					currentState.CoinMultiplierUntil = player:GetAttribute(attribute) or 0
				end
			end
			update()
		end)
	end
	MarketplaceService.PromptProductPurchaseFinished:Connect(function(userId)
		if userId == player.UserId then
			task.delay(0.7, refresh)
		end
	end)
	task.defer(refresh)
end

return MonetizationController
