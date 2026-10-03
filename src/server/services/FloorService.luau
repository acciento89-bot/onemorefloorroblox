--!strict

local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local RunService = game:GetService("RunService")
local TweenService = game:GetService("TweenService")
local Workspace = game:GetService("Workspace")

local shared = ReplicatedStorage:WaitForChild("OneMoreFloor"):WaitForChild("Shared")
local ChallengeConfig = require(shared.config.ChallengeConfig)
local CosmeticConfig = require(shared.config.CosmeticConfig)
local GameConfig = require(shared.config.GameConfig)
local ChallengeRules = require(shared.game.ChallengeRules)
local FloorStateRules = require(shared.game.FloorStateRules)
local SelectorRules = require(shared.game.SelectorRules)

local ChallengeFactory = require(script.Parent.ChallengeFactory)
local ProfileService = require(script.Parent.ProfileService)
local RetentionRules = require(shared.profile.RetentionRules)
local WorldService = require(script.Parent.WorldService)

local FloorService = {}

type Course = {
	Player: Player,
	Lane: WorldService.Lane,
	Floor: number,
	BestFloor: number,
	Streak: number,
	Coins: number,
	Assisted: boolean,
	Phase: string,
	Seed: number,
	History: { any },
	Runtime: ChallengeFactory.Runtime?,
	Selection: any,
	Params: any,
	DeadlineClock: number,
	DeadlineServer: number,
	Token: number,
	Connections: { RBXScriptConnection },
	CharacterConnections: { RBXScriptConnection },
	RateStartedAt: number,
	RateCount: number,
	ShowHint: boolean,
	PendingLoginReward: number,
	PendingLoginStreak: number,
}

local courses: { [Player]: Course } = {}
local nextLaneIndex = 0
local floorAction: RemoteEvent
local floorState: RemoteEvent

local function setMovement(player: Player, enabled: boolean)
	local character = player.Character
	local humanoid = character and character:FindFirstChildOfClass("Humanoid")
	if not humanoid then
		return
	end
	humanoid.UseJumpPower = true
	humanoid.WalkSpeed = if enabled then GameConfig.Movement.WalkSpeed else 0
	humanoid.JumpPower = if enabled then GameConfig.Movement.JumpPower else 0
end

local function rootPart(player: Player): BasePart?
	local character = player.Character
	local root = character and character:FindFirstChild("HumanoidRootPart")
	if root and root:IsA("BasePart") then
		return root
	end
	return nil
end

local function disconnectAll(connections: { RBXScriptConnection })
	for _, connection in connections do
		connection:Disconnect()
	end
	table.clear(connections)
end

local function playWinEffect(course: Course)
	local profile = ProfileService.get(course.Player)
	local root = rootPart(course.Player)
	if not profile or not root or profile.Settings.ReducedMotion then
		return
	end
	local item = CosmeticConfig.Items[profile.Equipped.WinEffect]
	if not item or item.Id == "win_none" then
		return
	end

	local effect = Instance.new("Part")
	effect.Name = "OMFWinEffect"
	effect.Shape = Enum.PartType.Cylinder
	effect.Size = Vector3.new(0.16, 3.2, 3.2)
	effect.CFrame = CFrame.new(root.Position - Vector3.new(0, 2.1, 0)) * CFrame.Angles(0, 0, math.rad(90))
	effect.Anchored = true
	effect.CanCollide = false
	effect.CanTouch = false
	effect.CanQuery = false
	effect.CastShadow = false
	effect.Material = Enum.Material.Neon
	effect.Color = Color3.fromRGB(item.ColorRGB[1], item.ColorRGB[2], item.ColorRGB[3])
	effect.Transparency = 0.18
	effect.Parent = Workspace

	local targetSize = if item.Id == "win_ring" then Vector3.new(0.12, 13, 13) else Vector3.new(0.12, 9, 9)
	TweenService:Create(effect, TweenInfo.new(0.45, Enum.EasingStyle.Quint, Enum.EasingDirection.Out), {
		Size = targetSize,
		Transparency = 1,
	}):Play()
	task.delay(0.55, function()
		if effect.Parent then
			effect:Destroy()
		end
	end)
end

local function challengeDefinition(course: Course)
	if not course.Selection then
		return nil
	end
	return ChallengeConfig.Families[course.Selection.FamilyId]
end

local function syncProfile(course: Course)
	local profile = ProfileService.get(course.Player)
	if not profile then
		return
	end
	local changed = profile.BestFloor ~= course.BestFloor or profile.Coins ~= course.Coins
	profile.BestFloor = course.BestFloor
	profile.Coins = course.Coins
	if changed then
		ProfileService.markDirty(course.Player)
	end
end

local function syncAttributes(course: Course)
	local player = course.Player
	local definition = challengeDefinition(course)
	player:SetAttribute("OMFFloor", course.Floor)
	player:SetAttribute("OMFBest", course.BestFloor)
	player:SetAttribute("OMFStreak", course.Streak)
	player:SetAttribute("OMFCoins", course.Coins)
	player:SetAttribute("OMFPhase", course.Phase)
	player:SetAttribute("OMFAssisted", course.Assisted)
	player:SetAttribute("OMFFamily", if course.Selection then course.Selection.FamilyId else "")
	player:SetAttribute("OMFFamilyName", if definition then definition.DisplayName else "")
	player:SetAttribute("OMFHint", if definition and course.ShowHint then definition.Hint else "")
	player:SetAttribute("OMFDeadline", course.DeadlineServer)
	local profile = ProfileService.get(player)
	if profile then
		player:SetAttribute("OMFDailyProgress", profile.DailyGoalProgress)
		player:SetAttribute("OMFDailyTarget", RetentionRules.DAILY_GOAL_TARGET)
		player:SetAttribute("OMFDailyStreak", profile.DailyStreak)
	end
end

local function emit(course: Course, extra: any?)
	syncAttributes(course)
	local definition = challengeDefinition(course)
	local payload = {
		Phase = course.Phase,
		Floor = course.Floor,
		BestFloor = course.BestFloor,
		Streak = course.Streak,
		Coins = course.Coins,
		Assisted = course.Assisted,
		Deadline = course.DeadlineServer,
		FamilyId = if course.Selection then course.Selection.FamilyId else "",
		FamilyName = if definition then definition.DisplayName else "",
		Hint = if definition and course.ShowHint then definition.Hint else "",
		TimeLimit = if course.Selection then course.Selection.TimeLimitSeconds else 0,
		DailyProgress = 0,
		DailyTarget = RetentionRules.DAILY_GOAL_TARGET,
		DailyStreak = 0,
	}
	local profile = ProfileService.get(course.Player)
	if profile then
		payload.DailyProgress = profile.DailyGoalProgress
		payload.DailyStreak = profile.DailyStreak
	end
	if type(extra) == "table" then
		for key, value in extra do
			payload[key] = value
		end
	end
	floorState:FireClient(course.Player, payload)
end

local function clearRuntime(course: Course)
	disconnectAll(course.Connections)
	if course.Runtime and course.Runtime.Model.Parent then
		course.Runtime.Model:Destroy()
	end
	course.Runtime = nil
end

local function transition(course: Course, nextPhase: string): boolean
	if not FloorStateRules.canTransition(course.Phase, nextPhase) then
		return false
	end
	course.Phase = nextPhase
	return true
end

local prepareFloor: (Course, boolean) -> ()
local failCourse: (Course, string) -> ()

local function isPlayerTouch(player: Player, hit: BasePart): boolean
	local character = hit:FindFirstAncestorOfClass("Model")
	return character ~= nil and character == player.Character
end

local function bindRuntime(course: Course)
	local runtime = course.Runtime
	if not runtime then
		return
	end
	local token = course.Token

	table.insert(
		course.Connections,
		runtime.Exit.Touched:Connect(function(hit)
			if
				course.Token ~= token
				or course.Phase ~= FloorStateRules.State.Active
				or not isPlayerTouch(course.Player, hit)
			then
				return
			end
			if not transition(course, FloorStateRules.State.Success) then
				return
			end
			local previousBest = course.BestFloor
			course.Streak += 1
			course.Coins += GameConfig.Economy.FloorCoins
			if course.Streak % GameConfig.Economy.StreakBonusEvery == 0 then
				course.Coins += GameConfig.Economy.StreakBonusCoins
			end
			if not course.Assisted then
				course.BestFloor = math.max(course.BestFloor, course.Floor)
			end
			syncProfile(course)

			local retention = nil
			local profile = ProfileService.get(course.Player)
			if profile and course.Selection then
				retention = RetentionRules.recordFloor(
					profile,
					RetentionRules.dayNumber(os.time()),
					course.Selection.FamilyId,
					course.Floor
				)
				course.Coins = profile.Coins
				ProfileService.markDirty(course.Player)
			end

			playWinEffect(course)
			setMovement(course.Player, false)
			emit(course, {
				Event = "FloorComplete",
				NewBest = course.BestFloor > previousBest,
				DailyCompleted = if retention then retention.DailyCompleted else false,
				DailyProgress = if retention then retention.DailyProgress else 0,
				DailyTarget = if retention then retention.DailyTarget else RetentionRules.DAILY_GOAL_TARGET,
				NewAchievements = if retention then retention.NewAchievements else {},
			})
			if not transition(course, FloorStateRules.State.Transition) then
				return
			end
			emit(course, { Event = "Transition" })
			task.delay(GameConfig.Floor.TransitionSeconds, function()
				if
					courses[course.Player] ~= course
					or course.Token ~= token
					or course.Phase ~= FloorStateRules.State.Transition
				then
					return
				end
				course.Floor += 1
				prepareFloor(course, false)
			end)
		end)
	)

	for _, hazard in runtime.Hazards do
		table.insert(
			course.Connections,
			hazard.Touched:Connect(function(hit)
				if
					course.Token == token
					and course.Phase == FloorStateRules.State.Active
					and isPlayerTouch(course.Player, hit)
				then
					failCourse(course, "Hazard")
				end
			end)
		)
	end

	if runtime.Switch and runtime.Gate then
		local activated = false
		table.insert(
			course.Connections,
			runtime.Switch.Touched:Connect(function(hit)
				if activated or course.Token ~= token or course.Phase ~= FloorStateRules.State.Active then
					return
				end
				if not isPlayerTouch(course.Player, hit) then
					return
				end
				activated = true
				runtime.Switch.Color = Color3.fromRGB(105, 214, 151)
				runtime.Gate.CanCollide = false
				TweenService:Create(runtime.Gate, TweenInfo.new(0.22, Enum.EasingStyle.Quint), {
					Transparency = 0.82,
				}):Play()
			end)
		)
	end

	for _, tile in runtime.Tiles do
		local triggered = false
		table.insert(
			course.Connections,
			tile.Touched:Connect(function(hit)
				if triggered or course.Token ~= token or course.Phase ~= FloorStateRules.State.Active then
					return
				end
				if not isPlayerTouch(course.Player, hit) then
					return
				end
				triggered = true
				tile.Color = course.Runtime and course.Runtime.Accent or tile.Color
				local hold = tonumber(tile:GetAttribute("HoldSeconds")) or 1
				task.delay(hold, function()
					if course.Token ~= token or course.Phase ~= FloorStateRules.State.Active or not tile.Parent then
						return
					end
					tile.CanCollide = false
					TweenService:Create(tile, TweenInfo.new(0.18), { Transparency = 1 }):Play()
				end)
			end)
		)
	end
end

local function positionCharacter(course: Course, character: Model)
	local runtime = course.Runtime
	if not runtime then
		return
	end
	local humanoid = character:WaitForChild("Humanoid", 8)
	local root = character:WaitForChild("HumanoidRootPart", 8)
	if not humanoid or not humanoid:IsA("Humanoid") or not root or not root:IsA("BasePart") then
		return
	end

	character:PivotTo(runtime.StartCFrame)
	root.AssemblyLinearVelocity = Vector3.zero
	root.AssemblyAngularVelocity = Vector3.zero
	humanoid.UseJumpPower = true
	humanoid.WalkSpeed = GameConfig.Movement.WalkSpeed
	humanoid.JumpPower = GameConfig.Movement.JumpPower

	disconnectAll(course.CharacterConnections)
	table.insert(
		course.CharacterConnections,
		humanoid.Died:Connect(function()
			failCourse(course, "CharacterDied")
		end)
	)
end

failCourse = function(course: Course, reason: string)
	if course.Phase ~= FloorStateRules.State.Active then
		return
	end
	if not transition(course, FloorStateRules.State.Failed) then
		return
	end
	course.Token += 1
	course.Streak = 0
	setMovement(course.Player, false)
	emit(course, {
		Event = "Failed",
		Reason = reason,
	})
end

prepareFloor = function(course: Course, reuseSelection: boolean)
	clearRuntime(course)
	course.Token += 1
	local token = course.Token
	course.Phase = FloorStateRules.State.Preparing

	if not reuseSelection or not course.Selection then
		local selection
		selection, course.Seed = SelectorRules.select(
			course.Seed,
			course.Floor,
			course.History,
			ChallengeConfig.FamilyOrder,
			ChallengeConfig.Families,
			ChallengeConfig.TutorialOrder
		)
		course.Selection = selection
		course.Params = ChallengeRules.parameters(selection)
		local profile = ProfileService.get(course.Player)
		local firstSeen = false
		if profile then
			firstSeen = RetentionRules.markFamilySeen(profile, selection.FamilyId)
			if firstSeen then
				ProfileService.markDirty(course.Player)
			end
		end
		course.ShowHint = course.Floor <= 3 or firstSeen
		assert(ChallengeRules.isSafe(selection, course.Params), "unsafe challenge selection")
		table.insert(course.History, selection)
		if #course.History > 8 then
			table.remove(course.History, 1)
		end
	end

	local origin = WorldService.floorOrigin(course.Lane, course.Floor)
	local profile = ProfileService.get(course.Player)
	local themeId = if profile then profile.Equipped.Theme else "theme_foundry"
	course.Runtime = ChallengeFactory.build(course.Lane.Model, course.Selection, course.Params, origin, themeId)
	course.DeadlineClock = os.clock() + course.Selection.TimeLimitSeconds
	course.DeadlineServer = Workspace:GetServerTimeNow() + course.Selection.TimeLimitSeconds
	bindRuntime(course)

	if not transition(course, FloorStateRules.State.Active) then
		error("failed to activate prepared floor")
	end
	positionCharacter(course, course.Player.Character or course.Player.CharacterAdded:Wait())
	if course.Token ~= token then
		return
	end
	emit(course, {
		Event = if reuseSelection then "Retry" else "FloorStart",
		Variant = course.Selection.Variant,
		DailyLoginReward = course.PendingLoginReward,
		DailyLoginStreak = course.PendingLoginStreak,
	})
	course.PendingLoginReward = 0
	course.PendingLoginStreak = 0
end

local function allowAction(course: Course): boolean
	local now = os.clock()
	if now - course.RateStartedAt >= 1 then
		course.RateStartedAt = now
		course.RateCount = 0
	end
	course.RateCount += 1
	return course.RateCount <= GameConfig.Limits.MaxActionRequestsPerSecond
end

local function onAction(player: Player, action: any)
	local course = courses[player]
	if not course or type(action) ~= "string" or not allowAction(course) then
		return
	end
	if action == "Retry" and course.Phase == FloorStateRules.State.Failed then
		prepareFloor(course, true)
	end
end

local function addPlayer(player: Player)
	local profile = ProfileService.load(player)
	local dailyLogin = RetentionRules.applyLogin(profile, RetentionRules.dayNumber(os.time()))
	if dailyLogin.Claimed then
		ProfileService.markDirty(player)
	end
	if player.Parent ~= Players then
		ProfileService.release(player)
		return
	end

	nextLaneIndex += 1
	local lane = WorldService.createLane(player, nextLaneIndex)
	local course: Course = {
		Player = player,
		Lane = lane,
		Floor = 1,
		BestFloor = profile.BestFloor,
		Streak = 0,
		Coins = profile.Coins,
		Assisted = false,
		Phase = FloorStateRules.State.Preparing,
		Seed = math.max(1, player.UserId * 131 + 17),
		History = {},
		Runtime = nil,
		Selection = nil,
		Params = nil,
		DeadlineClock = 0,
		DeadlineServer = 0,
		Token = 0,
		Connections = {},
		CharacterConnections = {},
		RateStartedAt = os.clock(),
		RateCount = 0,
		ShowHint = true,
		PendingLoginReward = dailyLogin.Reward,
		PendingLoginStreak = dailyLogin.Streak,
	}
	courses[player] = course

	player.CharacterAdded:Connect(function(character)
		task.defer(function()
			if courses[player] ~= course or not course.Runtime then
				return
			end
			positionCharacter(course, character)
			if course.Phase == FloorStateRules.State.Failed then
				setMovement(player, false)
			end
		end)
	end)

	task.defer(prepareFloor, course, false)
end

local function removePlayer(player: Player)
	local course = courses[player]
	if not course then
		return
	end
	syncProfile(course)
	clearRuntime(course)
	disconnectAll(course.CharacterConnections)
	WorldService.destroyLane(course.Lane)
	courses[player] = nil
	ProfileService.release(player)
end

function FloorService.refreshProfile(player: Player)
	local course = courses[player]
	local profile = ProfileService.get(player)
	if not course or not profile then
		return
	end
	course.BestFloor = profile.BestFloor
	course.Coins = profile.Coins
	emit(course, { Event = "ProfileUpdated" })
end

local function heartbeat()
	local now = os.clock()
	for _, course in courses do
		if course.Phase ~= FloorStateRules.State.Active or not course.Runtime then
			continue
		end
		if now >= course.DeadlineClock then
			failCourse(course, "Timeout")
			continue
		end

		local floorOrigin = WorldService.floorOrigin(course.Lane, course.Floor)
		local root = rootPart(course.Player)
		if root and root.Position.Y < floorOrigin.Y + GameConfig.Floor.KillPlaneOffset then
			failCourse(course, "Fall")
			continue
		end

		local elapsed = Workspace:GetServerTimeNow()
		for _, mover in course.Runtime.Movers do
			if mover.Part.Parent then
				local alpha = math.sin(elapsed * mover.Speed * 0.22 + mover.Phase)
				mover.Part.CFrame = mover.BaseCFrame + mover.Axis * (alpha * mover.Travel * 0.5)
			end
		end
	end
end

function FloorService.start()
	local remotes = ReplicatedStorage:WaitForChild("OneMoreFloorRemotes")
	floorAction = remotes:WaitForChild("FloorAction") :: RemoteEvent
	floorState = remotes:WaitForChild("FloorState") :: RemoteEvent

	floorAction.OnServerEvent:Connect(onAction)
	Players.PlayerAdded:Connect(addPlayer)
	Players.PlayerRemoving:Connect(removePlayer)
	for _, player in Players:GetPlayers() do
		task.defer(addPlayer, player)
	end
	RunService.Heartbeat:Connect(heartbeat)
end

return FloorService
