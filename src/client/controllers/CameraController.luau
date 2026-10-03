--!strict

local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local RunService = game:GetService("RunService")
local Workspace = game:GetService("Workspace")

local player = Players.LocalPlayer
local shared = ReplicatedStorage:WaitForChild("OneMoreFloor"):WaitForChild("Shared")
local GameConfig = require(shared.config.GameConfig)

local CameraController = {}

local currentDistance = GameConfig.Camera.Distance
local currentHeight = GameConfig.Camera.Height
local currentFocusLead = GameConfig.Camera.FocusLead
local currentFieldOfView = GameConfig.Camera.FieldOfView

local function keepAvatarVisible(character: Model)
	for _, descendant in character:GetDescendants() do
		if descendant:IsA("BasePart") and descendant.LocalTransparencyModifier ~= 0 then
			descendant.LocalTransparencyModifier = 0
		end
	end
end

local function characterParts(): (Model?, Humanoid?, BasePart?)
	local character = player.Character
	if not character then
		return nil, nil, nil
	end
	local humanoid = character:FindFirstChildOfClass("Humanoid")
	local root = character:FindFirstChild("HumanoidRootPart")
	if humanoid and root and root:IsA("BasePart") then
		return character, humanoid, root
	end
	return character, humanoid, nil
end
local function activeProfile()
	local family = player:GetAttribute("OMFFamily")
	if type(family) == "string" then
		local profile = GameConfig.Camera.Profiles[family]
		if profile then
			return profile
		end
	end
	return GameConfig.Camera
end

local function desired(root: BasePart): CFrame
	local focus = root.Position + Vector3.new(0, GameConfig.Camera.FocusHeight, currentFocusLead)
	local position = root.Position + Vector3.new(0, currentHeight, -currentDistance)
	return CFrame.lookAt(position, focus)
end

function CameraController.start()
	local camera = Workspace.CurrentCamera
	if camera then
		camera.FieldOfView = GameConfig.Camera.FieldOfView
	end

	RunService:BindToRenderStep("OneMoreFloorCamera", Enum.RenderPriority.Camera.Value + 1, function(dt)
		local activeCamera = Workspace.CurrentCamera
		local character, humanoid, root = characterParts()
		if not activeCamera or not character or not humanoid or not root then
			return
		end

		keepAvatarVisible(character)
		activeCamera.CameraType = Enum.CameraType.Scriptable
		activeCamera.CameraSubject = humanoid

		local profile = activeProfile()
		local blendAlpha = 1 - math.exp(-GameConfig.Camera.ProfileBlendSpeed * math.max(0, dt))
		currentDistance += (profile.Distance - currentDistance) * blendAlpha
		currentHeight += (profile.Height - currentHeight) * blendAlpha
		currentFocusLead += (profile.FocusLead - currentFocusLead) * blendAlpha
		currentFieldOfView += (profile.FieldOfView - currentFieldOfView) * blendAlpha
		activeCamera.FieldOfView = currentFieldOfView

		local target = desired(root)
		local alpha = 1 - math.exp(-GameConfig.Camera.Smoothness * math.max(0, dt))
		activeCamera.CFrame = activeCamera.CFrame:Lerp(target, alpha)
	end)
end

return CameraController
