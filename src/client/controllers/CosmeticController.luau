--!strict

local ContextActionService = game:GetService("ContextActionService")
local GuiService = game:GetService("GuiService")
local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local Workspace = game:GetService("Workspace")

local player = Players.LocalPlayer
local playerGui = player:WaitForChild("PlayerGui")
local shared = ReplicatedStorage:WaitForChild("OneMoreFloor"):WaitForChild("Shared")
local CosmeticConfig = require(shared.config.CosmeticConfig)
local remotes = ReplicatedStorage:WaitForChild("OneMoreFloorRemotes")
local cosmeticAction = remotes:WaitForChild("CosmeticAction") :: RemoteEvent
local cosmeticState = remotes:WaitForChild("CosmeticState") :: RemoteEvent
local cosmeticQuery = remotes:WaitForChild("CosmeticQuery") :: RemoteFunction

local CosmeticController = {}

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

local screen: ScreenGui
local openButton: TextButton
local panel: Frame
local coinsLabel: TextLabel
local list: ScrollingFrame
local reducedButton: TextButton
local state: any = nil
local itemButtons: { [string]: TextButton } = {}
local itemCards: { [string]: Frame } = {}

local function rgb(values): Color3
	return Color3.fromRGB(values[1], values[2], values[3])
end

local function round(parent: Instance, radius: number)
	local corner = Instance.new("UICorner")
	corner.CornerRadius = UDim.new(0, radius)
	corner.Parent = parent
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
	bold: boolean
): TextLabel
	local item = Instance.new("TextLabel")
	item.Size = size
	item.Position = position
	item.BackgroundTransparency = 1
	item.Font = if bold then Enum.Font.GothamBold else Enum.Font.GothamMedium
	item.Text = text
	item.TextSize = textSize
	item.TextColor3 = color
	item.TextXAlignment = Enum.TextXAlignment.Left
	item.TextYAlignment = Enum.TextYAlignment.Center
	item.Parent = parent
	return item
end

local function equippedId(category: string): string?
	if not state or type(state.Equipped) ~= "table" then
		return nil
	end
	if category == "Trail" then
		return state.Equipped.Trail
	elseif category == "WinEffect" then
		return state.Equipped.WinEffect
	elseif category == "Theme" then
		return state.Equipped.Theme
	end
	return nil
end

local function render()
	local coins = if state and type(state.Coins) == "number" then state.Coins else player:GetAttribute("OMFCoins") or 0
	coinsLabel.Text = ("◈ %d"):format(coins)

	for id, button in itemButtons do
		local item = CosmeticConfig.Items[id]
		local owned = state and type(state.OwnedCosmetics) == "table" and state.OwnedCosmetics[id] == true
		local equipped = equippedId(item.Category) == id
		if equipped then
			button.Text = "EQUIPPED"
			button.BackgroundColor3 = COLORS.Green
			button.TextColor3 = COLORS.Foundation
		elseif owned then
			button.Text = "EQUIP"
			button.BackgroundColor3 = COLORS.Cyan
			button.TextColor3 = COLORS.Foundation
		else
			button.Text = ("BUY  ◈ %d"):format(item.Cost)
			button.BackgroundColor3 = COLORS.Raised
			button.TextColor3 = if coins >= item.Cost then COLORS.Text else COLORS.Muted
		end

		local card = itemCards[id]
		local selected = equipped
		local line = card:FindFirstChildOfClass("UIStroke")
		if line then
			line.Color = if selected then rgb(item.ColorRGB) else Color3.fromRGB(73, 86, 102)
			line.Transparency = if selected then 0.08 else 0.55
		end
	end

	local reduced = state and type(state.Settings) == "table" and state.Settings.ReducedMotion == true
	reducedButton.Text = if reduced then "REDUCED MOTION  •  ON" else "REDUCED MOTION  •  OFF"
	reducedButton.BackgroundColor3 = if reduced then COLORS.Green else COLORS.Raised
	reducedButton.TextColor3 = if reduced then COLORS.Foundation else COLORS.Text
end

local function createCard(id: string, layoutOrder: number)
	local item = CosmeticConfig.Items[id]
	local card = Instance.new("Frame")
	card.Name = id
	card.LayoutOrder = layoutOrder
	card.Size = UDim2.new(1, -10, 0, 72)
	card.BackgroundColor3 = COLORS.Surface
	card.BorderSizePixel = 0
	card.Parent = list
	round(card, 12)
	stroke(card, Color3.fromRGB(73, 86, 102), 0.55)
	itemCards[id] = card

	local accent = Instance.new("Frame")
	accent.Size = UDim2.fromOffset(5, 48)
	accent.Position = UDim2.fromOffset(0, 12)
	accent.BackgroundColor3 = rgb(item.ColorRGB)
	accent.BorderSizePixel = 0
	accent.Parent = card
	round(accent, 3)

	local categoryName = if item.Category == "WinEffect" then "WIN EFFECT" else string.upper(item.Category)
	label(card, categoryName, UDim2.fromOffset(155, 16), UDim2.fromOffset(16, 9), 9, COLORS.Muted, true)
	label(card, item.DisplayName, UDim2.new(1, -180, 0, 28), UDim2.fromOffset(16, 27), 16, COLORS.Text, true)

	local action = Instance.new("TextButton")
	action.Name = "Action"
	action.AnchorPoint = Vector2.new(1, 0.5)
	action.Position = UDim2.new(1, -12, 0.5, 0)
	action.Size = UDim2.fromOffset(128, 42)
	action.BackgroundColor3 = COLORS.Raised
	action.BorderSizePixel = 0
	action.Font = Enum.Font.GothamBold
	action.TextSize = 12
	action.TextColor3 = COLORS.Text
	action.Selectable = true
	action.Parent = card
	round(action, 10)
	action.Activated:Connect(function()
		local owned = state and state.OwnedCosmetics and state.OwnedCosmetics[id] == true
		if equippedId(item.Category) == id then
			return
		end
		cosmeticAction:FireServer({
			Kind = if owned then "Equip" else "Buy",
			Id = id,
		})
	end)
	itemButtons[id] = action
end
local function updateResponsive()
	local camera = Workspace.CurrentCamera
	if not camera then
		return
	end
	local viewport = camera.ViewportSize
	if viewport.X < 620 then
		panel.Size = UDim2.new(1, -24, 0, math.min(560, viewport.Y - 48))
		openButton.Position = UDim2.new(1, -16, 1, -18)
	else
		panel.Size = UDim2.fromOffset(520, math.min(580, viewport.Y - 70))
		openButton.Position = UDim2.new(1, -22, 1, -22)
	end
end

local function setPanelVisible(visible: boolean)
	panel.Visible = visible
	if visible then
		render()
		GuiService.SelectedObject = itemButtons.trail_none or reducedButton
	elseif GuiService.SelectedObject and GuiService.SelectedObject:IsDescendantOf(panel) then
		GuiService.SelectedObject = openButton
	end
end

local function buildGui()
	local existing = playerGui:FindFirstChild("OneMoreFloorCosmetics")
	if existing then
		existing:Destroy()
	end

	screen = Instance.new("ScreenGui")
	screen.Name = "OneMoreFloorCosmetics"
	screen.ResetOnSpawn = false
	screen.DisplayOrder = 30
	screen.Parent = playerGui

	openButton = Instance.new("TextButton")
	openButton.Name = "StyleButton"
	openButton.AnchorPoint = Vector2.new(1, 1)
	openButton.Position = UDim2.new(1, -22, 1, -22)
	openButton.Size = UDim2.fromOffset(116, 46)
	openButton.BackgroundColor3 = COLORS.Foundation
	openButton.BorderSizePixel = 0
	openButton.Font = Enum.Font.GothamBold
	openButton.Text = "STYLE  ◈"
	openButton.TextSize = 13
	openButton.TextColor3 = COLORS.Text
	openButton.Selectable = true
	openButton.Parent = screen
	round(openButton, 12)
	stroke(openButton, COLORS.Warm, 0.35)

	panel = Instance.new("Frame")
	panel.Name = "StyleGarage"
	panel.AnchorPoint = Vector2.new(0.5, 0.5)
	panel.Position = UDim2.fromScale(0.5, 0.5)
	panel.Size = UDim2.fromOffset(520, 580)
	panel.BackgroundColor3 = COLORS.Foundation
	panel.BorderSizePixel = 0
	panel.Visible = false
	panel.Parent = screen
	round(panel, 18)
	stroke(panel, COLORS.Cyan, 0.42)

	label(panel, "STYLE GARAGE", UDim2.new(1, -160, 0, 34), UDim2.fromOffset(18, 14), 20, COLORS.Text, true)
	local sub = label(
		panel,
		"Cosmetic only — no leaderboard advantage.",
		UDim2.new(1, -36, 0, 22),
		UDim2.fromOffset(18, 45),
		10,
		COLORS.Muted,
		false
	)
	sub.TextWrapped = true

	coinsLabel = label(panel, "◈ 0", UDim2.fromOffset(100, 28), UDim2.new(1, -160, 0, 16), 14, COLORS.Warm, true)
	coinsLabel.TextXAlignment = Enum.TextXAlignment.Right

	local close = Instance.new("TextButton")
	close.AnchorPoint = Vector2.new(1, 0)
	close.Position = UDim2.new(1, -14, 0, 14)
	close.Size = UDim2.fromOffset(38, 34)
	close.BackgroundColor3 = COLORS.Raised
	close.BorderSizePixel = 0
	close.Font = Enum.Font.GothamBold
	close.Text = "×"
	close.TextSize = 20
	close.TextColor3 = COLORS.Text
	close.Parent = panel
	round(close, 10)

	list = Instance.new("ScrollingFrame")
	list.Name = "Items"
	list.Position = UDim2.fromOffset(14, 78)
	list.Size = UDim2.new(1, -28, 1, -158)
	list.BackgroundTransparency = 1
	list.BorderSizePixel = 0
	list.ScrollBarThickness = 5
	list.ScrollBarImageColor3 = COLORS.Cyan
	list.AutomaticCanvasSize = Enum.AutomaticSize.Y
	list.CanvasSize = UDim2.new()
	list.Parent = panel

	local layout = Instance.new("UIListLayout")
	layout.Padding = UDim.new(0, 8)
	layout.SortOrder = Enum.SortOrder.LayoutOrder
	layout.Parent = list

	for index, id in CosmeticConfig.Order do
		createCard(id, index)
	end

	reducedButton = Instance.new("TextButton")
	reducedButton.Name = "ReducedMotion"
	reducedButton.AnchorPoint = Vector2.new(0.5, 1)
	reducedButton.Position = UDim2.new(0.5, 0, 1, -34)
	reducedButton.Size = UDim2.new(1, -28, 0, 38)
	reducedButton.BackgroundColor3 = COLORS.Raised
	reducedButton.BorderSizePixel = 0
	reducedButton.Font = Enum.Font.GothamBold
	reducedButton.Text = "REDUCED MOTION  •  OFF"
	reducedButton.TextSize = 11
	reducedButton.TextColor3 = COLORS.Text
	reducedButton.Selectable = true
	reducedButton.Parent = panel
	round(reducedButton, 10)
	reducedButton.Activated:Connect(function()
		local current = state and type(state.Settings) == "table" and state.Settings.ReducedMotion == true
		cosmeticAction:FireServer({
			Kind = "Setting",
			Key = "ReducedMotion",
			Value = not current,
		})
	end)

	local note = label(
		panel,
		"Themes apply on the next floor.",
		UDim2.new(1, -28, 0, 20),
		UDim2.new(0, 14, 1, -22),
		10,
		COLORS.Muted,
		false
	)
	note.TextXAlignment = Enum.TextXAlignment.Center

	openButton.Activated:Connect(function()
		setPanelVisible(not panel.Visible)
	end)
	close.Activated:Connect(function()
		setPanelVisible(false)
	end)

	updateResponsive()
	local camera = Workspace.CurrentCamera
	if camera then
		camera:GetPropertyChangedSignal("ViewportSize"):Connect(updateResponsive)
	end
end
function CosmeticController.start()
	buildGui()

	cosmeticState.OnClientEvent:Connect(function(payload)
		if type(payload) == "table" then
			state = payload
			render()
		end
	end)

	ContextActionService:BindActionAtPriority("OneMoreFloorStyleToggle", function(_name, inputState)
		if inputState == Enum.UserInputState.Begin then
			setPanelVisible(not panel.Visible)
		end
		return Enum.ContextActionResult.Sink
	end, false, Enum.ContextActionPriority.High.Value + 20, Enum.KeyCode.C, Enum.KeyCode.ButtonY)
	ContextActionService:BindActionAtPriority("OneMoreFloorStyleClose", function(_name, inputState)
		if inputState == Enum.UserInputState.Begin and panel.Visible then
			setPanelVisible(false)
			return Enum.ContextActionResult.Sink
		end
		return Enum.ContextActionResult.Pass
	end, false, Enum.ContextActionPriority.High.Value + 21, Enum.KeyCode.ButtonB, Enum.KeyCode.Escape)

	task.spawn(function()
		for _ = 1, 4 do
			local ok, result = pcall(function()
				return cosmeticQuery:InvokeServer()
			end)
			if ok and type(result) == "table" then
				state = result
				render()
				return
			end
			task.wait(0.75)
		end
	end)
end

return CosmeticController
