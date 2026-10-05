--!strict

local ContextActionService = game:GetService("ContextActionService")
local GuiService = game:GetService("GuiService")
local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local RunService = game:GetService("RunService")
local Workspace = game:GetService("Workspace")

local player = Players.LocalPlayer
local playerGui = player:WaitForChild("PlayerGui")
local shared = ReplicatedStorage:WaitForChild("OneMoreFloor"):WaitForChild("Shared")
local CosmeticConfig = require(shared.config.CosmeticConfig)
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
local logo: Frame
local floorProgress: Frame
local floorProgressFill: Frame
local floorProgressText: TextLabel
local quickRail: Frame
local boostQuick: TextButton
local styleQuick: TextButton
local dailyQuick: TextButton
local dailyBackdrop: Frame
local dailyPanel: Frame
local dailyCloseButton: TextButton
local dailyStatus: TextLabel
local dailyChallenge: TextLabel
local dailyRewardTiles: { Frame } = {}
local conceptShowcase: Frame
local conceptProgressText: TextLabel
local conceptProgressFill: Frame
local conceptFloorText: TextLabel
local conceptBestText: TextLabel
local conceptStreakText: TextLabel
local conceptCoinsText: TextLabel
local conceptDailyCaption: TextLabel
local conceptDailyTiles: { Frame } = {}
local metricValues: { [string]: TextLabel } = {}
local deadline = 0
local timeLimit = 0
local currentPhase = "Preparing"
local currentDailyProgress = 0
local currentDailyTarget = 5
local currentDailyStreak = 0

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

local function closeExternalPanels()
	local monetization = playerGui:FindFirstChild("OneMoreFloorMonetization")
	local boostPanel = monetization and monetization:FindFirstChild("Panel")
	if boostPanel and boostPanel:IsA("Frame") then
		boostPanel.Visible = false
	end
	local cosmetics = playerGui:FindFirstChild("OneMoreFloorCosmetics")
	local stylePanel = cosmetics and cosmetics:FindFirstChild("StyleGarage")
	if stylePanel and stylePanel:IsA("Frame") then
		stylePanel.Visible = false
	end
end

local function refreshDailyPanel()
	if not dailyPanel then
		return
	end
	dailyStatus.Text = ("LOGIN STREAK  x%d"):format(currentDailyStreak)
	dailyChallenge.Text = ("DAILY FLOOR GOAL  %d / %d"):format(
		math.min(currentDailyProgress, currentDailyTarget),
		currentDailyTarget
	)
	local cycleDay = ((math.max(1, currentDailyStreak) - 1) % 5) + 1
	for index, tile in dailyRewardTiles do
		tile.BackgroundColor3 = if index < cycleDay
			then Color3.fromRGB(54, 96, 82)
			elseif index == cycleDay then Color3.fromRGB(89, 70, 104)
			else COLORS.Surface
	end
end

local function setDailyVisible(visible: boolean)
	dailyPanel.Visible = visible
	dailyBackdrop.Visible = visible
	if visible then
		closeExternalPanels()
		refreshDailyPanel()
		GuiService.SelectedObject = dailyCloseButton
	elseif GuiService.SelectedObject and GuiService.SelectedObject:IsDescendantOf(dailyPanel) then
		GuiService.SelectedObject = nil
	end
end

local function updateResponsive()
	local camera = Workspace.CurrentCamera
	if not camera then
		return
	end
	local width = camera.ViewportSize.X
	conceptShowcase.Visible = false
	status.Visible = width < 620
	if width < 620 then
		logo.Visible = false
		floorProgress.Visible = false
		quickRail.Visible = true
		boostQuick.Visible = false
		styleQuick.Visible = false
		dailyQuick.Visible = true
		dailyQuick.Position = UDim2.fromOffset(0, 0)
		quickRail.Position = UDim2.new(1, -18, 0, 90)
		status.Position = UDim2.new(0.5, 0, 0, 16)
		status.Size = UDim2.new(1, -24, 0, 112)
		metricGrid.FillDirectionMaxCells = 2
		metricGrid.CellSize = UDim2.new(0.5, -4, 0, 44)
		metricGrid.CellPadding = UDim2.fromOffset(6, 6)
		objective.Position = UDim2.new(0.5, 0, 0, 138)
		objective.Size = UDim2.new(1, -28, 0, 72)
		feedback.Position = UDim2.new(0.5, -150, 0, 220)
		resultPanel.Position = UDim2.fromScale(0.5, 0.52)
		resultPanel.Size = UDim2.fromOffset(340, 170)
	elseif width < 1050 then
		logo.Visible = true
		floorProgress.Visible = true
		quickRail.Visible = true
		boostQuick.Visible = true
		styleQuick.Visible = true
		dailyQuick.Visible = true
		boostQuick.Position = UDim2.fromOffset(0, 0)
		styleQuick.Position = UDim2.fromOffset(0, 50)
		dailyQuick.Position = UDim2.fromOffset(0, 100)
		quickRail.Position = UDim2.new(1, -266, 0, 106)
		status.Position = UDim2.new(0.5, 0, 0, 88)
		status.Size = UDim2.new(1, -34, 0, 66)
		metricGrid.FillDirectionMaxCells = 4
		metricGrid.CellSize = UDim2.new(0.25, -5, 1, 0)
		metricGrid.CellPadding = UDim2.fromOffset(6, 0)
		objective.Position = UDim2.new(0.55, 0, 1, -88)
		objective.Size = UDim2.fromOffset(360, 64)
		feedback.Position = UDim2.new(0.5, -150, 0, 248)
		resultPanel.Position = UDim2.new(0.55, 0, 1, -112)
		resultPanel.Size = UDim2.fromOffset(320, 154)
		resultPanel.Position = UDim2.new(0.55, 0, 1, -112)
		resultPanel.Size = UDim2.fromOffset(320, 154)
	else
		logo.Visible = true
		floorProgress.Visible = true
		quickRail.Visible = true
		boostQuick.Visible = true
		styleQuick.Visible = true
		dailyQuick.Visible = true
		boostQuick.Position = UDim2.fromOffset(0, 0)
		styleQuick.Position = UDim2.fromOffset(0, 50)
		dailyQuick.Position = UDim2.fromOffset(0, 100)
		quickRail.Position = UDim2.new(1, -266, 0, 106)
		status.Position = UDim2.new(0.5, 0, 0, 88)
		status.Size = UDim2.fromOffset(620, 66)
		metricGrid.FillDirectionMaxCells = 4
		metricGrid.CellSize = UDim2.new(0.25, -5, 1, 0)
		metricGrid.CellPadding = UDim2.fromOffset(6, 0)
		objective.Position = UDim2.new(0.55, 0, 1, -88)
		objective.Size = UDim2.fromOffset(380, 64)
		feedback.Position = UDim2.new(0.5, -150, 0, 248)
	end
end

local function refreshMetrics(payload: any?)
	local floor = if payload and payload.Floor then payload.Floor else player:GetAttribute("OMFFloor") or 1
	local best = if payload and payload.BestFloor then payload.BestFloor else player:GetAttribute("OMFBest") or 0
	local streak = if payload and payload.Streak then payload.Streak else player:GetAttribute("OMFStreak") or 0
	local coins = if payload and payload.Coins then payload.Coins else player:GetAttribute("OMFCoins") or 0
	metricValues.FloorMetric.Text = tostring(floor)
	local tierStart = math.floor((math.max(1, floor) - 1) / 10) * 10 + 1
	local tierTarget = tierStart + 9
	local tierProgress = math.clamp((floor - tierStart + 1) / 10, 0, 1)
	floorProgressText.Text = ("FLOOR %02d / %02d"):format(floor, tierTarget)
	floorProgressFill.Size = UDim2.fromScale(tierProgress, 1)
	metricValues.BestMetric.Text = tostring(best)
	metricValues.StreakMetric.Text = ("x%d"):format(streak)
	metricValues.CoinsMetric.Text = ("◈ %d"):format(coins)
	conceptFloorText.Text = ("FLOOR %02d"):format(floor)
	conceptBestText.Text = ("BEST %02d"):format(best)
	conceptStreakText.Text = ("STREAK x%d"):format(streak)
	conceptCoinsText.Text = ("◈ %d"):format(coins)
	conceptProgressText.Text = ("FLOOR %02d / %02d"):format(floor, tierTarget)
	conceptProgressFill.Size = UDim2.fromScale(tierProgress, 1)
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
	currentDailyProgress = tonumber(payload.DailyProgress) or tonumber(player:GetAttribute("OMFDailyProgress")) or currentDailyProgress
	currentDailyTarget = tonumber(payload.DailyTarget) or tonumber(player:GetAttribute("OMFDailyTarget")) or currentDailyTarget
	currentDailyStreak = tonumber(payload.DailyStreak) or tonumber(player:GetAttribute("OMFDailyStreak")) or currentDailyStreak
	refreshMetrics(payload)
	refreshDailyPanel()

	objectiveTitle.Text = string.upper(tostring(payload.FamilyName or "NEXT FLOOR"))
	objectiveHint.Text = tostring(payload.Hint or "")
	objectiveHint.Visible = objectiveHint.Text ~= ""
	objectiveTitle.Position = if objectiveHint.Visible then UDim2.fromOffset(12, 8) else UDim2.fromOffset(12, 18)
	resultPanel.Visible = currentPhase == "Failed"
	objective.Visible = currentPhase ~= "Failed"
	dailyQuick.Active = currentPhase ~= "Failed"
	dailyQuick.AutoButtonColor = currentPhase ~= "Failed"
	if currentPhase == "Failed" then
		setDailyVisible(false)
		closeExternalPanels()
		local reason = tostring(payload.Reason or "Miss")
		resultTitle.Text = "FLOOR FAILED"
		resultSummary.Text = ("Floor %d  •  %s\nRetry instantly — same challenge, same rules."):format(
			tonumber(payload.Floor) or 1,
			reason
		)
		GuiService.SelectedObject = retryButton
	elseif payload.Event == "FloorComplete" then
		if payload.DailyCompleted == true then
			conceptDailyCaption.Text = "DAILY COMPLETE"
			conceptDailyTiles[1].BackgroundColor3 = Color3.fromRGB(58, 112, 89)
		end
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
			conceptDailyCaption.Text = ("LOGIN STREAK x%d"):format(tonumber(payload.DailyLoginStreak) or 1)
			conceptDailyTiles[1].BackgroundColor3 = Color3.fromRGB(58, 112, 89)
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

	logo = Instance.new("Frame")
	logo.Name = "OneMoreFloorLogo"
	logo.Position = UDim2.fromOffset(18, 10)
	logo.Size = UDim2.fromOffset(210, 74)
	logo.BackgroundTransparency = 1
	logo.Parent = screen

	local oneMore = label(
		logo,
		"OneMore",
		"ONE MORE",
		UDim2.fromOffset(174, 32),
		UDim2.fromOffset(0, 0),
		25,
		COLORS.Cyan,
		Enum.Font.GothamBlack
	)
	oneMore.TextStrokeColor3 = Color3.fromRGB(28, 41, 58)
	oneMore.TextStrokeTransparency = 0.36

	local floorWord = label(
		logo,
		"FloorWord",
		"FLOOR",
		UDim2.fromOffset(150, 34),
		UDim2.fromOffset(20, 27),
		30,
		COLORS.Warm,
		Enum.Font.GothamBlack
	)
	floorWord.TextStrokeColor3 = Color3.fromRGB(57, 45, 34)
	floorWord.TextStrokeTransparency = 0.34

	local logoTag = label(
		logo,
		"Tagline",
		"BEAT THE ROOM • CLIMB AGAIN",
		UDim2.fromOffset(204, 12),
		UDim2.fromOffset(1, 61),
		8,
		COLORS.Muted,
		Enum.Font.GothamBold
	)
	logoTag.TextXAlignment = Enum.TextXAlignment.Left

	floorProgress = Instance.new("Frame")
	floorProgress.Name = "FloorProgress"
	floorProgress.AnchorPoint = Vector2.new(0.5, 0)
	floorProgress.Position = UDim2.new(0.5, 0, 0, 14)
	floorProgress.Size = UDim2.fromOffset(292, 52)
	floorProgress.BackgroundColor3 = COLORS.Foundation
	floorProgress.BackgroundTransparency = 0.03
	floorProgress.BorderSizePixel = 0
	floorProgress.Parent = screen
	corner(floorProgress, 16)
	stroke(floorProgress, COLORS.Cyan, 0.42, 1.3)

	floorProgressText = label(
		floorProgress,
		"Text",
		"FLOOR 01 / 10",
		UDim2.new(1, -20, 0, 24),
		UDim2.fromOffset(10, 4),
		11,
		COLORS.Text,
		Enum.Font.GothamBold
	)
	floorProgressText.TextXAlignment = Enum.TextXAlignment.Center

	local progressTrack = Instance.new("Frame")
	progressTrack.Position = UDim2.fromOffset(12, 34)
	progressTrack.Size = UDim2.new(1, -24, 0, 10)
	progressTrack.BackgroundColor3 = Color3.fromRGB(25, 33, 45)
	progressTrack.BorderSizePixel = 0
	progressTrack.ClipsDescendants = true
	progressTrack.Parent = floorProgress
	corner(progressTrack, 99)

	floorProgressFill = Instance.new("Frame")
	floorProgressFill.Name = "Fill"
	floorProgressFill.Size = UDim2.fromScale(0.1, 1)
	floorProgressFill.BackgroundColor3 = COLORS.Green
	floorProgressFill.BorderSizePixel = 0
	floorProgressFill.Parent = progressTrack
	corner(floorProgressFill, 99)

	quickRail = Instance.new("Frame")
	quickRail.Name = "ConceptQuickRail"
	quickRail.AnchorPoint = Vector2.new(1, 0)
	quickRail.Position = UDim2.new(1, -18, 0, 90)
	quickRail.Size = UDim2.fromOffset(96, 146)
	quickRail.BackgroundTransparency = 1
	quickRail.Parent = screen

	boostQuick =
		actionButton(quickRail, "BoostQuick", "BOOSTS", UDim2.fromOffset(96, 42), UDim2.fromOffset(0, 0), COLORS.Cyan)
	styleQuick =
		actionButton(quickRail, "StyleQuick", "STYLE", UDim2.fromOffset(96, 42), UDim2.fromOffset(0, 50), COLORS.Warm)
	dailyQuick =
		actionButton(quickRail, "DailyQuick", "DAILY", UDim2.fromOffset(96, 42), UDim2.fromOffset(0, 100), COLORS.Green)
	dailyQuick.Name = "DailyQuick"

	boostQuick.Activated:Connect(function()
		setDailyVisible(false)
		local monetization = playerGui:FindFirstChild("OneMoreFloorMonetization")
		local panel = monetization and monetization:FindFirstChild("Panel")
		if panel and panel:IsA("Frame") then
			panel.Visible = true
		end
	end)
	styleQuick.Activated:Connect(function()
		setDailyVisible(false)
		local cosmetics = playerGui:FindFirstChild("OneMoreFloorCosmetics")
		local panel = cosmetics and cosmetics:FindFirstChild("StyleGarage")
		if panel and panel:IsA("Frame") then
			panel.Visible = true
		end
	end)
	dailyQuick.Activated:Connect(function()
		if currentPhase ~= "Failed" then
			setDailyVisible(true)
		end
	end)

	dailyBackdrop = Instance.new("Frame")
	dailyBackdrop.Name = "DailyBackdrop"
	dailyBackdrop.Size = UDim2.fromScale(1, 1)
	dailyBackdrop.BackgroundColor3 = Color3.fromRGB(7, 10, 16)
	dailyBackdrop.BackgroundTransparency = 0.34
	dailyBackdrop.BorderSizePixel = 0
	dailyBackdrop.Visible = false
	dailyBackdrop.ZIndex = 60
	dailyBackdrop.Parent = screen

	dailyPanel = Instance.new("Frame")
	dailyPanel.Name = "DailyRewardsPanel"
	dailyPanel.AnchorPoint = Vector2.new(0.5, 0.5)
	dailyPanel.Position = UDim2.fromScale(0.5, 0.5)
	dailyPanel.Size = UDim2.fromOffset(438, 304)
	dailyPanel.BackgroundColor3 = COLORS.Foundation
	dailyPanel.BorderSizePixel = 0
	dailyPanel.Visible = false
	dailyPanel.ZIndex = 61
	dailyPanel.Parent = screen
	corner(dailyPanel, 20)
	stroke(dailyPanel, COLORS.Warm, 0.30, 1.4)

	local dailyGradient = Instance.new("UIGradient")
	dailyGradient.Rotation = 115
	dailyGradient.Color = ColorSequence.new(Color3.fromRGB(57, 48, 70), COLORS.Foundation)
	dailyGradient.Parent = dailyPanel

	local dailyModalTitle = label(
		dailyPanel,
		"Title",
		"DAILY REWARDS",
		UDim2.new(1, -90, 0, 38),
		UDim2.fromOffset(20, 12),
		22,
		COLORS.Text,
		Enum.Font.GothamBlack
	)
	dailyModalTitle.ZIndex = 62

	dailyCloseButton = actionButton(
		dailyPanel,
		"Close",
		"×",
		UDim2.fromOffset(46, 44),
		UDim2.new(1, -60, 0, 12),
		COLORS.Raised
	)
	dailyCloseButton.TextColor3 = COLORS.Text
	dailyCloseButton.TextSize = 22
	dailyCloseButton.ZIndex = 62

	dailyStatus = label(
		dailyPanel,
		"Status",
		"LOGIN STREAK  x0",
		UDim2.new(1, -40, 0, 24),
		UDim2.fromOffset(20, 52),
		11,
		COLORS.Muted,
		Enum.Font.GothamBold
	)
	dailyStatus.TextXAlignment = Enum.TextXAlignment.Center
	dailyStatus.ZIndex = 62

	dailyRewardTiles = {}
	local rewardIcons = { "✓", "◈", "✦", "♢", "★" }
	local rewardLabels = { "LOGIN", "COINS", "BOOST", "STYLE", "RARE" }
	for index = 1, 5 do
		local tile = Instance.new("Frame")
		tile.Name = ("Day%02d"):format(index)
		tile.Position = UDim2.fromOffset(18 + (index - 1) * 81, 86)
		tile.Size = UDim2.fromOffset(73, 108)
		tile.BackgroundColor3 = COLORS.Surface
		tile.BorderSizePixel = 0
		tile.ZIndex = 62
		tile.Parent = dailyPanel
		corner(tile, 13)
		stroke(tile, if index == 1 then COLORS.Warm else COLORS.Cyan, 0.62)

		local day = label(
			tile,
			"Day",
			("DAY %d"):format(index),
			UDim2.new(1, 0, 0, 18),
			UDim2.fromOffset(0, 6),
			9,
			COLORS.Muted,
			Enum.Font.GothamBold
		)
		day.TextXAlignment = Enum.TextXAlignment.Center
		day.ZIndex = 63
		local icon = label(
			tile,
			"Icon",
			rewardIcons[index],
			UDim2.new(1, 0, 0, 42),
			UDim2.fromOffset(0, 28),
			24,
			if index == 1 then COLORS.Green else COLORS.Warm,
			Enum.Font.GothamBlack
		)
		icon.TextXAlignment = Enum.TextXAlignment.Center
		icon.ZIndex = 63
		local reward = label(
			tile,
			"Reward",
			rewardLabels[index],
			UDim2.new(1, 0, 0, 18),
			UDim2.fromOffset(0, 80),
			9,
			COLORS.Text,
			Enum.Font.GothamBold
		)
		reward.TextXAlignment = Enum.TextXAlignment.Center
		reward.ZIndex = 63
		table.insert(dailyRewardTiles, tile)
	end

	dailyChallenge = label(
		dailyPanel,
		"Challenge",
		"DAILY FLOOR GOAL  0 / 5",
		UDim2.new(1, -40, 0, 24),
		UDim2.fromOffset(20, 204),
		11,
		COLORS.Cyan,
		Enum.Font.GothamBold
	)
	dailyChallenge.TextXAlignment = Enum.TextXAlignment.Center
	dailyChallenge.ZIndex = 62

	local dailyAutoNote = label(
		dailyPanel,
		"AutoNote",
		"LOGIN + DAILY GOAL REWARDS APPLY AUTOMATICALLY",
		UDim2.new(1, -40, 0, 22),
		UDim2.fromOffset(20, 234),
		9,
		COLORS.Muted,
		Enum.Font.GothamBold
	)
	dailyAutoNote.TextXAlignment = Enum.TextXAlignment.Center
	dailyAutoNote.ZIndex = 62

	local dailyDone = actionButton(
		dailyPanel,
		"Done",
		"DONE",
		UDim2.fromOffset(150, 42),
		UDim2.new(0.5, -75, 1, -54),
		COLORS.Green
	)
	dailyDone.ZIndex = 62
	dailyCloseButton.Activated:Connect(function()
		setDailyVisible(false)
	end)
	dailyDone.Activated:Connect(function()
		setDailyVisible(false)
	end)
	dailyBackdrop.InputBegan:Connect(function(input)
		if input.UserInputType == Enum.UserInputType.MouseButton1 then
			setDailyVisible(false)
		end
	end)

	conceptShowcase = Instance.new("Frame")
	conceptShowcase.Name = "ConceptShowcase"
	conceptShowcase.Size = UDim2.fromScale(1, 1)
	conceptShowcase.BackgroundTransparency = 1
	conceptShowcase.Visible = false
	conceptShowcase.Parent = screen

	local function gradient(panel: GuiObject, first: Color3, second: Color3, rotation: number?)
		local item = Instance.new("UIGradient")
		item.Rotation = rotation or 110
		item.Color = ColorSequence.new(first, second)
		item.Parent = panel
	end

	local function rgb(values): Color3
		return Color3.fromRGB(values[1], values[2], values[3])
	end

	local metricsPill = Instance.new("Frame")
	metricsPill.Name = "ConceptMetricsPill"
	metricsPill.Position = UDim2.fromOffset(18, 90)
	metricsPill.Size = UDim2.fromOffset(316, 48)
	metricsPill.BackgroundColor3 = COLORS.Foundation
	metricsPill.BackgroundTransparency = 0.03
	metricsPill.BorderSizePixel = 0
	metricsPill.Parent = conceptShowcase
	corner(metricsPill, 14)
	stroke(metricsPill, COLORS.Cyan, 0.52, 1.2)
	gradient(metricsPill, Color3.fromRGB(37, 55, 70), COLORS.Foundation)
	conceptFloorText = label(
		metricsPill,
		"Floor",
		"FLOOR 01",
		UDim2.fromOffset(76, 34),
		UDim2.fromOffset(10, 7),
		10,
		COLORS.Text,
		Enum.Font.GothamBold
	)
	conceptBestText = label(
		metricsPill,
		"Best",
		"BEST 00",
		UDim2.fromOffset(72, 34),
		UDim2.fromOffset(86, 7),
		10,
		COLORS.Warm,
		Enum.Font.GothamBold
	)
	conceptStreakText = label(
		metricsPill,
		"Streak",
		"STREAK x0",
		UDim2.fromOffset(88, 34),
		UDim2.fromOffset(158, 7),
		10,
		COLORS.Green,
		Enum.Font.GothamBold
	)
	conceptCoinsText = label(
		metricsPill,
		"Coins",
		"◈ 0",
		UDim2.fromOffset(58, 34),
		UDim2.fromOffset(248, 7),
		10,
		COLORS.Warm,
		Enum.Font.GothamBold
	)
	conceptCoinsText.TextXAlignment = Enum.TextXAlignment.Right

	local styleShowcase = Instance.new("Frame")
	styleShowcase.Name = "FloorStyleShowcase"
	styleShowcase.AnchorPoint = Vector2.new(0, 1)
	styleShowcase.Position = UDim2.new(0, 18, 1, -18)
	styleShowcase.Size = UDim2.fromOffset(308, 208)
	styleShowcase.BackgroundColor3 = COLORS.Foundation
	styleShowcase.BackgroundTransparency = 0.02
	styleShowcase.BorderSizePixel = 0
	styleShowcase.Parent = conceptShowcase
	corner(styleShowcase, 20)
	stroke(styleShowcase, COLORS.Cyan, 0.38, 1.3)
	gradient(styleShowcase, Color3.fromRGB(46, 61, 78), COLORS.Foundation)
	label(
		styleShowcase,
		"Title",
		"FLOOR STYLE",
		UDim2.fromOffset(180, 28),
		UDim2.fromOffset(15, 8),
		17,
		COLORS.Text,
		Enum.Font.GothamBlack
	)
	local featureHint = label(
		styleShowcase,
		"Hint",
		"FEATURED",
		UDim2.fromOffset(88, 20),
		UDim2.fromOffset(202, 11),
		9,
		COLORS.Cyan,
		Enum.Font.GothamBold
	)
	featureHint.TextXAlignment = Enum.TextXAlignment.Right

	local featured = {
		CosmeticConfig.Items.trail_cyan,
		CosmeticConfig.Items.win_burst,
		CosmeticConfig.Items.theme_neon,
	}
	for index, item in featured do
		local accent = rgb(item.ColorRGB)
		local card = Instance.new("Frame")
		card.Name = ("Featured%02d"):format(index)
		card.Position = UDim2.fromOffset(14 + (index - 1) * 98, 43)
		card.Size = UDim2.fromOffset(88, 151)
		card.BackgroundColor3 = COLORS.Surface
		card.BorderSizePixel = 0
		card.Parent = styleShowcase
		corner(card, 14)
		stroke(card, accent, 0.42, 1)
		gradient(card, accent:Lerp(COLORS.Surface, 0.72), COLORS.Foundation)
		local preview = Instance.new("Frame")
		preview.Position = UDim2.fromOffset(8, 8)
		preview.Size = UDim2.fromOffset(72, 58)
		preview.BackgroundColor3 = Color3.fromRGB(17, 24, 34)
		preview.BorderSizePixel = 0
		preview.Parent = card
		corner(preview, 11)
		stroke(preview, accent, 0.20, 1)
		local mark = Instance.new("Frame")
		mark.AnchorPoint = Vector2.new(0.5, 0.5)
		mark.Position = UDim2.fromScale(0.5, 0.5)
		mark.Size = UDim2.fromOffset(if index == 2 then 28 else 46, if index == 2 then 28 else 12)
		mark.Rotation = if index == 1 then -20 elseif index == 3 then 18 else 45
		mark.BackgroundColor3 = accent
		mark.BorderSizePixel = 0
		mark.Parent = preview
		corner(mark, if index == 2 then 99 else 7)
		local name = label(
			card,
			"Name",
			item.DisplayName,
			UDim2.fromOffset(78, 34),
			UDim2.fromOffset(5, 69),
			10,
			COLORS.Text,
			Enum.Font.GothamBold
		)
		name.TextWrapped = true
		name.TextXAlignment = Enum.TextXAlignment.Center
		local cost = label(
			card,
			"Cost",
			("◈ %d"):format(item.Cost),
			UDim2.fromOffset(78, 16),
			UDim2.fromOffset(5, 103),
			9,
			COLORS.Warm,
			Enum.Font.GothamBold
		)
		cost.TextXAlignment = Enum.TextXAlignment.Center
		local open = actionButton(card, "OpenStyle", "OPEN", UDim2.fromOffset(70, 24), UDim2.fromOffset(9, 122), accent)
		open.TextSize = 9
		open.Activated:Connect(function()
			local cosmetics = playerGui:FindFirstChild("OneMoreFloorCosmetics")
			local panel = cosmetics and cosmetics:FindFirstChild("StyleGarage")
			if panel and panel:IsA("Frame") then
				panel.Visible = true
			end
		end)
	end

	local rightColumn = Instance.new("Frame")
	rightColumn.Name = "ConceptRightColumn"
	rightColumn.AnchorPoint = Vector2.new(1, 0)
	rightColumn.Position = UDim2.new(1, -18, 0, 96)
	rightColumn.Size = UDim2.fromOffset(232, 366)
	rightColumn.BackgroundTransparency = 1
	rightColumn.Parent = conceptShowcase

	local conceptProgressCard = Instance.new("Frame")
	conceptProgressCard.Name = "ConceptProgressCard"
	conceptProgressCard.Size = UDim2.fromOffset(232, 86)
	conceptProgressCard.BackgroundColor3 = COLORS.Foundation
	conceptProgressCard.BorderSizePixel = 0
	conceptProgressCard.Parent = rightColumn
	corner(conceptProgressCard, 17)
	stroke(conceptProgressCard, COLORS.Cyan, 0.38, 1.2)
	gradient(conceptProgressCard, Color3.fromRGB(42, 59, 76), COLORS.Foundation)
	label(
		conceptProgressCard,
		"Title",
		"TOWER PROGRESS",
		UDim2.fromOffset(204, 20),
		UDim2.fromOffset(13, 7),
		13,
		COLORS.Text,
		Enum.Font.GothamBlack
	)
	conceptProgressText = label(
		conceptProgressCard,
		"Progress",
		"FLOOR 01 / 10",
		UDim2.fromOffset(204, 18),
		UDim2.fromOffset(13, 28),
		9,
		COLORS.Muted,
		Enum.Font.GothamBold
	)
	conceptProgressText.TextXAlignment = Enum.TextXAlignment.Center
	local conceptTrack = Instance.new("Frame")
	conceptTrack.Position = UDim2.fromOffset(13, 55)
	conceptTrack.Size = UDim2.fromOffset(206, 14)
	conceptTrack.BackgroundColor3 = Color3.fromRGB(15, 23, 32)
	conceptTrack.BorderSizePixel = 0
	conceptTrack.ClipsDescendants = true
	conceptTrack.Parent = conceptProgressCard
	corner(conceptTrack, 99)
	conceptProgressFill = Instance.new("Frame")
	conceptProgressFill.Size = UDim2.fromScale(0.1, 1)
	conceptProgressFill.BackgroundColor3 = COLORS.Green
	conceptProgressFill.BorderSizePixel = 0
	conceptProgressFill.Parent = conceptTrack
	corner(conceptProgressFill, 99)

	local conceptDailyRewards = Instance.new("Frame")
	conceptDailyRewards.Name = "ConceptDailyRewards"
	conceptDailyRewards.Position = UDim2.fromOffset(0, 96)
	conceptDailyRewards.Size = UDim2.fromOffset(232, 128)
	conceptDailyRewards.BackgroundColor3 = COLORS.Foundation
	conceptDailyRewards.BorderSizePixel = 0
	conceptDailyRewards.Parent = rightColumn
	corner(conceptDailyRewards, 17)
	stroke(conceptDailyRewards, COLORS.Warm, 0.42, 1.2)
	gradient(conceptDailyRewards, Color3.fromRGB(57, 49, 66), COLORS.Foundation)
	label(
		conceptDailyRewards,
		"Title",
		"DAILY REWARDS",
		UDim2.fromOffset(204, 20),
		UDim2.fromOffset(13, 6),
		13,
		COLORS.Text,
		Enum.Font.GothamBlack
	)
	conceptDailyCaption = label(
		conceptDailyRewards,
		"Caption",
		"CLIMB TO KEEP STREAK",
		UDim2.fromOffset(204, 16),
		UDim2.fromOffset(13, 27),
		8,
		COLORS.Muted,
		Enum.Font.GothamBold
	)
	conceptDailyCaption.TextXAlignment = Enum.TextXAlignment.Center
	conceptDailyTiles = {}
	for index = 1, 5 do
		local tile = Instance.new("Frame")
		tile.Position = UDim2.fromOffset(9 + (index - 1) * 44, 48)
		tile.Size = UDim2.fromOffset(39, 67)
		tile.BackgroundColor3 = if index == 1 then Color3.fromRGB(59, 106, 89) else Color3.fromRGB(48, 57, 72)
		tile.BorderSizePixel = 0
		tile.Parent = conceptDailyRewards
		corner(tile, 9)
		stroke(tile, Color3.fromRGB(135, 151, 178), 0.56, 1)
		local day = label(
			tile,
			"Day",
			("D%d"):format(index),
			UDim2.fromOffset(35, 14),
			UDim2.fromOffset(2, 4),
			8,
			COLORS.Muted,
			Enum.Font.GothamBold
		)
		day.TextXAlignment = Enum.TextXAlignment.Center
		local reward = label(
			tile,
			"Reward",
			if index == 1
				then "✓"
				elseif index == 2 then "◈"
				elseif index == 3 then "✦"
				elseif index == 4 then "◆"
				else "★",
			UDim2.fromOffset(35, 31),
			UDim2.fromOffset(2, 22),
			15,
			if index == 1 then COLORS.Green else COLORS.Warm,
			Enum.Font.GothamBlack
		)
		reward.TextXAlignment = Enum.TextXAlignment.Center
		table.insert(conceptDailyTiles, tile)
	end

	local conceptVistaCard = Instance.new("Frame")
	conceptVistaCard.Name = "ConceptVistaCard"
	conceptVistaCard.Position = UDim2.fromOffset(0, 234)
	conceptVistaCard.Size = UDim2.fromOffset(232, 132)
	conceptVistaCard.BackgroundColor3 = COLORS.Foundation
	conceptVistaCard.BorderSizePixel = 0
	conceptVistaCard.Parent = rightColumn
	corner(conceptVistaCard, 17)
	stroke(conceptVistaCard, COLORS.Cyan, 0.42, 1.2)
	gradient(conceptVistaCard, Color3.fromRGB(42, 54, 75), COLORS.Foundation)
	label(
		conceptVistaCard,
		"Title",
		"NEXT FLOOR PREVIEW",
		UDim2.fromOffset(204, 20),
		UDim2.fromOffset(13, 6),
		13,
		COLORS.Text,
		Enum.Font.GothamBlack
	)
	local conceptViewport = Instance.new("ViewportFrame")
	conceptViewport.Name = "TowerPreview"
	conceptViewport.Position = UDim2.fromOffset(12, 31)
	conceptViewport.Size = UDim2.fromOffset(208, 89)
	conceptViewport.BackgroundColor3 = Color3.fromRGB(73, 84, 119)
	conceptViewport.BorderSizePixel = 0
	conceptViewport.Ambient = Color3.fromRGB(126, 139, 165)
	conceptViewport.LightColor = Color3.fromRGB(255, 214, 167)
	conceptViewport.LightDirection = Vector3.new(-1, -1, -0.5)
	conceptViewport.Parent = conceptVistaCard
	corner(conceptViewport, 12)
	local previewWorld = Instance.new("WorldModel")
	previewWorld.Parent = conceptViewport
	local previewCamera = Instance.new("Camera")
	previewCamera.CFrame = CFrame.lookAt(Vector3.new(11, 7, 14), Vector3.new(0, 1.8, 0))
	previewCamera.FieldOfView = 40
	previewCamera.Parent = conceptViewport
	conceptViewport.CurrentCamera = previewCamera
	local function previewPart(
		name: string,
		size: Vector3,
		position: Vector3,
		color: Color3,
		material: Enum.Material
	): Part
		local part = Instance.new("Part")
		part.Name = name
		part.Size = size
		part.Position = position
		part.Anchored = true
		part.CanCollide = false
		part.Color = color
		part.Material = material
		part.Parent = previewWorld
		return part
	end
	previewPart(
		"RoomFloor",
		Vector3.new(10, 0.5, 8),
		Vector3.new(0, 0, 0),
		Color3.fromRGB(54, 61, 75),
		Enum.Material.Concrete
	)
	previewPart(
		"LeftWall",
		Vector3.new(0.5, 5, 8),
		Vector3.new(-5, 2.2, 0),
		Color3.fromRGB(38, 46, 61),
		Enum.Material.Metal
	)
	previewPart(
		"RightWall",
		Vector3.new(0.5, 5, 8),
		Vector3.new(5, 2.2, 0),
		Color3.fromRGB(38, 46, 61),
		Enum.Material.Metal
	)
	previewPart("Goal", Vector3.new(3.2, 0.25, 2.1), Vector3.new(0, 0.42, -2.2), COLORS.Warm, Enum.Material.Neon)
	previewPart("Portal", Vector3.new(4.0, 3.4, 0.35), Vector3.new(0, 2.0, -3.7), COLORS.Cyan, Enum.Material.Neon)

	status = Instance.new("Frame")
	status.Name = "Status"
	status.AnchorPoint = Vector2.new(0.5, 0)
	status.Position = UDim2.new(0.5, 0, 0, 88)
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
	objective.Position = UDim2.new(0.5, 0, 0, 166)
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
	resultPanel.Size = UDim2.fromOffset(340, 170)
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
		UDim2.new(1, -30, 0, 34),
		UDim2.fromOffset(15, 8),
		19,
		COLORS.Text,
		Enum.Font.GothamBlack
	)
	resultTitle.TextXAlignment = Enum.TextXAlignment.Center
	resultSummary = label(
		resultPanel,
		"Summary",
		"",
		UDim2.new(1, -36, 0, 48),
		UDim2.fromOffset(18, 42),
		10,
		COLORS.Muted,
		Enum.Font.GothamMedium
	)
	resultSummary.TextWrapped = true
	resultSummary.TextXAlignment = Enum.TextXAlignment.Center

	retryButton = actionButton(
		resultPanel,
		"Retry",
		"RETRY  •  R / X",
		UDim2.fromOffset(220, 42),
		UDim2.new(0.5, -110, 1, -52),
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
