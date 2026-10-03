--!strict

local ContextActionService = game:GetService("ContextActionService")
local GuiService = game:GetService("GuiService")
local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local RunService = game:GetService("RunService")
local Workspace = game:GetService("Workspace")

local player = Players.LocalPlayer
local playerGui = player:WaitForChild("PlayerGui")
local remotes = ReplicatedStorage:WaitForChild("OneMoreFloorRemotes")
local floorAction = remotes:WaitForChild("FloorAction") :: RemoteEvent
local floorState = remotes:WaitForChild("FloorState") :: RemoteEvent

local HudController = {}

local COLORS = {
	Foundation = Color3.fromRGB(19, 26, 36),
	Surface = Color3.fromRGB(31, 41, 54),
	Raised = Color3.fromRGB(42, 53, 68),
	Text = Color3.fromRGB(244, 247, 251),
	Muted = Color3.fromRGB(164, 178, 194),
	Warm = Color3.fromRGB(244, 166, 80),
	Cyan = Color3.fromRGB(91, 220, 239),
	Green = Color3.fromRGB(105, 214, 151),
	Danger = Color3.fromRGB(239, 92, 105),
}

local screen: ScreenGui
local status: Frame
local metricGrid: UIGridLayout
local objective: Frame
local objectiveTitle: TextLabel
local objectiveHint: TextLabel
local timerFill: Frame
local feedback: TextLabel
local resultPanel: Frame
local resultTitle: TextLabel
local resultSummary: TextLabel
local retryButton: TextButton
local metricValues: { [string]: TextLabel } = {}
local deadline = 0
local timeLimit = 0
local currentPhase = "Preparing"

local function corner(parent: Instance, radius: number)
	local item = Instance.new("UICorner")
	item.CornerRadius = UDim.new(0, radius)
	item.Parent = parent
end

local function stroke(parent: Instance, color: Color3, transparency: number, thickness: number?)
	local item = Instance.new("UIStroke")
	item.Color = color
	item.Transparency = transparency
	item.Thickness = thickness or 1
	item.Parent = parent
end

local function textLimit(parent: TextLabel | TextButton, minimum: number, maximum: number)
	local item = Instance.new("UITextSizeConstraint")
	item.MinTextSize = minimum
	item.MaxTextSize = maximum
	item.Parent = parent
end

local function label(
	parent: Instance,
	name: string,
	text: string,
	size: UDim2,
	position: UDim2,
	textSize: number,
	color: Color3,
	font: Enum.Font?
): TextLabel
	local item = Instance.new("TextLabel")
	item.Name = name
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

local function metric(parent: Instance, name: string, title: string): TextLabel
	local card = Instance.new("Frame")
	card.Name = name
	card.BackgroundColor3 = COLORS.Surface
	card.BackgroundTransparency = 0.02
	card.BorderSizePixel = 0
	card.Parent = parent
	corner(card, 12)
	stroke(card, COLORS.Cyan, 0.78)

	label(
		card,
		"Title",
		title,
		UDim2.new(1, -20, 0, 14),
		UDim2.fromOffset(10, 5),
		9,
		COLORS.Muted,
		Enum.Font.GothamBold
	)
	local value = label(
		card,
		"Value",
		"—",
		UDim2.new(1, -20, 1, -21),
		UDim2.fromOffset(10, 17),
		17,
		COLORS.Text,
		Enum.Font.GothamBlack
	)
	value.TextScaled = true
	textLimit(value, 11, 18)
	metricValues[name] = value
	return value
end

local function actionButton(
	parent: Instance,
	name: string,
	text: string,
	size: UDim2,
	position: UDim2,
	background: Color3
): TextButton
	local item = Instance.new("TextButton")
	item.Name = name
	item.Size = size
	item.Position = position
	item.BackgroundColor3 = background
	item.BorderSizePixel = 0
	item.Font = Enum.Font.GothamBlack
	item.Text = text
	item.TextSize = 16
	item.TextColor3 = COLORS.Foundation
	item.Selectable = true
	item.AutoButtonColor = true
	item.Parent = parent
	corner(item, 13)
	return item
end

local function updateResponsive()
	local camera = Workspace.CurrentCamera
	if not camera then
		return
	end
	local width = camera.ViewportSize.X
	if width < 620 then
		status.Size = UDim2.new(1, -24, 0, 112)
		metricGrid.FillDirectionMaxCells = 2
		metricGrid.CellSize = UDim2.new(0.5, -4, 0, 44)
		metricGrid.CellPadding = UDim2.fromOffset(6, 6)
		objective.Position = UDim2.new(0.5, 0, 0, 138)
		objective.Size = UDim2.new(1, -28, 0, 72)
		feedback.Position = UDim2.new(0.5, -150, 0, 220)
	elseif width < 1050 then
		status.Size = UDim2.new(1, -34, 0, 66)
		metricGrid.FillDirectionMaxCells = 4
		metricGrid.CellSize = UDim2.new(0.25, -5, 1, 0)
		metricGrid.CellPadding = UDim2.fromOffset(6, 0)
		objective.Position = UDim2.new(0.5, 0, 0, 94)
		objective.Size = UDim2.fromOffset(500, 72)
		feedback.Position = UDim2.new(0.5, -150, 0, 176)
	else
		status.Size = UDim2.fromOffset(720, 66)
		metricGrid.FillDirectionMaxCells = 4
		metricGrid.CellSize = UDim2.new(0.25, -5, 1, 0)
		metricGrid.CellPadding = UDim2.fromOffset(6, 0)
		objective.Position = UDim2.new(0.5, 0, 0, 94)
		objective.Size = UDim2.fromOffset(540, 72)
		feedback.Position = UDim2.new(0.5, -150, 0, 176)
	end
end

local function refreshMetrics(payload: any?)
	local floor = if payload and payload.Floor then payload.Floor else player:GetAttribute("OMFFloor") or 1
	local best = if payload and payload.BestFloor then payload.BestFloor else player:GetAttribute("OMFBest") or 0
	local streak = if payload and payload.Streak then payload.Streak else player:GetAttribute("OMFStreak") or 0
	local coins = if payload and payload.Coins then payload.Coins else player:GetAttribute("OMFCoins") or 0
	metricValues.FloorMetric.Text = tostring(floor)
	metricValues.BestMetric.Text = tostring(best)
	metricValues.StreakMetric.Text = ("x%d"):format(streak)
	metricValues.CoinsMetric.Text = ("◈ %d"):format(coins)
end

local function showFeedback(text: string, color: Color3)
	feedback.Text = text
	feedback.TextColor3 = color
	feedback.TextTransparency = 0
	feedback.Visible = true
	feedback.Size = UDim2.fromOffset(340, 60)
	task.delay(0.65, function()
		if feedback.Parent then
			feedback.TextTransparency = 1
			feedback.Visible = false
		end
	end)
end

local function applyState(payload: any)
	if type(payload) ~= "table" then
		return
	end
	currentPhase = tostring(payload.Phase or currentPhase)
	deadline = tonumber(payload.Deadline) or 0
	timeLimit = tonumber(payload.TimeLimit) or 0
	refreshMetrics(payload)

	objectiveTitle.Text = string.upper(tostring(payload.FamilyName or "NEXT FLOOR"))
	objectiveHint.Text = tostring(payload.Hint or "")
	objectiveHint.Visible = objectiveHint.Text ~= ""
	objectiveTitle.Position = if objectiveHint.Visible then UDim2.fromOffset(12, 8) else UDim2.fromOffset(12, 18)
	resultPanel.Visible = currentPhase == "Failed"
	objective.Visible = currentPhase ~= "Failed"

	if currentPhase == "Failed" then
		local reason = tostring(payload.Reason or "Miss")
		resultTitle.Text = "FLOOR FAILED"
		resultSummary.Text = ("Floor %d  •  %s\nRetry instantly — same challenge, same rules."):format(
			tonumber(payload.Floor) or 1,
			reason
		)
		GuiService.SelectedObject = retryButton
	elseif payload.Event == "FloorComplete" then
		local achievements = if type(payload.NewAchievements) == "table" then #payload.NewAchievements else 0
		if payload.NewBest == true then
			showFeedback(("NEW BEST  •  FLOOR %02d"):format(tonumber(payload.Floor) or 1), COLORS.Warm)
		elseif payload.DailyCompleted == true then
			showFeedback("DAILY GOAL  •  +25 ◈", COLORS.Green)
		elseif achievements > 0 then
			showFeedback("MILESTONE UNLOCKED", COLORS.Warm)
		else
			showFeedback("FLOOR CLEARED", COLORS.Green)
		end
	elseif payload.Event == "FloorStart" or payload.Event == "Retry" then
		local loginReward = tonumber(payload.DailyLoginReward) or 0
		if payload.Event == "FloorStart" and loginReward > 0 then
			showFeedback(
				("DAILY x%d  •  +%d ◈"):format(tonumber(payload.DailyLoginStreak) or 1, loginReward),
				COLORS.Warm
			)
		else
			showFeedback(("FLOOR %02d"):format(tonumber(payload.Floor) or 1), COLORS.Cyan)
		end
		if GuiService.SelectedObject == retryButton then
			GuiService.SelectedObject = nil
		end
	end
end

local function buildGui()
	local existing = playerGui:FindFirstChild("OneMoreFloorHUD")
	if existing then
		existing:Destroy()
	end

	screen = Instance.new("ScreenGui")
	screen.Name = "OneMoreFloorHUD"
	screen.ResetOnSpawn = false
	screen.IgnoreGuiInset = false
	screen.DisplayOrder = 20
	screen.ZIndexBehavior = Enum.ZIndexBehavior.Sibling
	screen.Parent = playerGui

	status = Instance.new("Frame")
	status.Name = "Status"
	status.AnchorPoint = Vector2.new(0.5, 0)
	status.Position = UDim2.new(0.5, 0, 0, 16)
	status.Size = UDim2.fromOffset(720, 66)
	status.BackgroundColor3 = COLORS.Foundation
	status.BackgroundTransparency = 0.03
	status.BorderSizePixel = 0
	status.Parent = screen
	corner(status, 16)
	stroke(status, COLORS.Cyan, 0.62)

	local metrics = Instance.new("Frame")
	metrics.Name = "Metrics"
	metrics.Position = UDim2.fromOffset(8, 8)
	metrics.Size = UDim2.new(1, -16, 1, -16)
	metrics.BackgroundTransparency = 1
	metrics.Parent = status

	metricGrid = Instance.new("UIGridLayout")
	metricGrid.FillDirection = Enum.FillDirection.Horizontal
	metricGrid.FillDirectionMaxCells = 4
	metricGrid.SortOrder = Enum.SortOrder.LayoutOrder
	metricGrid.CellPadding = UDim2.fromOffset(6, 0)
	metricGrid.CellSize = UDim2.new(0.25, -5, 1, 0)
	metricGrid.Parent = metrics

	metric(metrics, "FloorMetric", "FLOOR")
	metric(metrics, "BestMetric", "BEST")
	metric(metrics, "StreakMetric", "STREAK")
	metric(metrics, "CoinsMetric", "COINS")
	metrics.FloorMetric.LayoutOrder = 1
	metrics.BestMetric.LayoutOrder = 2
	metrics.StreakMetric.LayoutOrder = 3
	metrics.CoinsMetric.LayoutOrder = 4

	objective = Instance.new("Frame")
	objective.Name = "Objective"
	objective.AnchorPoint = Vector2.new(0.5, 0)
	objective.Position = UDim2.new(0.5, 0, 0, 94)
	objective.Size = UDim2.fromOffset(540, 72)
	objective.BackgroundColor3 = COLORS.Foundation
	objective.BackgroundTransparency = 0.08
	objective.BorderSizePixel = 0
	objective.Parent = screen
	corner(objective, 14)
	stroke(objective, COLORS.Warm, 0.68)

	objectiveTitle = label(
		objective,
		"Title",
		"NEXT FLOOR",
		UDim2.new(1, -24, 0, 24),
		UDim2.fromOffset(12, 8),
		13,
		COLORS.Warm,
		Enum.Font.GothamBlack
	)
	objectiveHint = label(objective, "Hint", "", UDim2.new(1, -24, 0, 24), UDim2.fromOffset(12, 31), 11, COLORS.Text)
	objectiveHint.TextWrapped = true

	local timerTrack = Instance.new("Frame")
	timerTrack.Name = "Timer"
	timerTrack.Position = UDim2.new(0, 12, 1, -9)
	timerTrack.Size = UDim2.new(1, -24, 0, 4)
	timerTrack.BackgroundColor3 = COLORS.Raised
	timerTrack.BorderSizePixel = 0
	timerTrack.ClipsDescendants = true
	timerTrack.Parent = objective
	corner(timerTrack, 3)

	timerFill = Instance.new("Frame")
	timerFill.Name = "Fill"
	timerFill.Size = UDim2.fromScale(1, 1)
	timerFill.BackgroundColor3 = COLORS.Cyan
	timerFill.BorderSizePixel = 0
	timerFill.Parent = timerTrack
	corner(timerFill, 3)

	feedback = label(
		screen,
		"Feedback",
		"",
		UDim2.fromOffset(340, 60),
		UDim2.new(0.5, -170, 0.36, 0),
		28,
		COLORS.Cyan,
		Enum.Font.GothamBlack
	)
	feedback.TextXAlignment = Enum.TextXAlignment.Center
	feedback.TextTransparency = 1
	feedback.Visible = false

	resultPanel = Instance.new("Frame")
	resultPanel.Name = "Result"
	resultPanel.AnchorPoint = Vector2.new(0.5, 0.5)
	resultPanel.Position = UDim2.fromScale(0.5, 0.52)
	resultPanel.Size = UDim2.fromOffset(390, 230)
	resultPanel.BackgroundColor3 = COLORS.Foundation
	resultPanel.BorderSizePixel = 0
	resultPanel.Visible = false
	resultPanel.Parent = screen
	corner(resultPanel, 20)
	stroke(resultPanel, COLORS.Danger, 0.24, 1.5)

	resultTitle = label(
		resultPanel,
		"Title",
		"FLOOR FAILED",
		UDim2.new(1, -30, 0, 44),
		UDim2.fromOffset(15, 18),
		23,
		COLORS.Text,
		Enum.Font.GothamBlack
	)
	resultTitle.TextXAlignment = Enum.TextXAlignment.Center
	resultSummary = label(
		resultPanel,
		"Summary",
		"",
		UDim2.new(1, -36, 0, 64),
		UDim2.fromOffset(18, 68),
		12,
		COLORS.Muted,
		Enum.Font.GothamMedium
	)
	resultSummary.TextWrapped = true
	resultSummary.TextXAlignment = Enum.TextXAlignment.Center

	retryButton = actionButton(
		resultPanel,
		"Retry",
		"RETRY  •  R / X",
		UDim2.fromOffset(230, 54),
		UDim2.new(0.5, -115, 1, -76),
		COLORS.Warm
	)
	retryButton.Activated:Connect(function()
		floorAction:FireServer("Retry")
	end)

	updateResponsive()
	Workspace:GetPropertyChangedSignal("CurrentCamera"):Connect(function()
		task.defer(updateResponsive)
	end)
	local camera = Workspace.CurrentCamera
	if camera then
		camera:GetPropertyChangedSignal("ViewportSize"):Connect(updateResponsive)
	end
end

function HudController.start()
	buildGui()

	floorState.OnClientEvent:Connect(applyState)
	for _, attribute in { "OMFFloor", "OMFBest", "OMFStreak", "OMFCoins" } do
		player:GetAttributeChangedSignal(attribute):Connect(function()
			refreshMetrics(nil)
		end)
	end

	ContextActionService:BindActionAtPriority("OneMoreFloorRetry", function(_name, inputState)
		if inputState == Enum.UserInputState.Begin and currentPhase == "Failed" then
			floorAction:FireServer("Retry")
		end
		return Enum.ContextActionResult.Sink
	end, false, Enum.ContextActionPriority.High.Value + 50, Enum.KeyCode.R, Enum.KeyCode.ButtonX)

	RunService.RenderStepped:Connect(function()
		if currentPhase ~= "Active" or deadline <= 0 or timeLimit <= 0 then
			timerFill.Size = UDim2.fromScale(1, 1)
			return
		end
		local remaining = math.max(0, deadline - Workspace:GetServerTimeNow())
		local alpha = math.clamp(remaining / timeLimit, 0, 1)
		timerFill.Size = UDim2.fromScale(alpha, 1)
		timerFill.BackgroundColor3 = if alpha < 0.28 then COLORS.Danger else COLORS.Cyan
	end)

	refreshMetrics(nil)
end

return HudController
