--!strict

local Players = game:GetService("Players")
local ReplicatedStorage = game:GetService("ReplicatedStorage")
local Workspace = game:GetService("Workspace")

local player = Players.LocalPlayer
local shared = ReplicatedStorage:WaitForChild("OneMoreFloor"):WaitForChild("Shared")
local GameConfig = require(shared.config.GameConfig)

local CameraController = {}

local viewportConnection: RBXScriptConnection? = nil

local function keepAvatarVisible(character: Model)
	for _, descendant in character:GetDescendants() do
		if descendant:IsA("BasePart") and descendant.LocalTransparencyModifier ~= 0 then
			descendant.LocalTransparencyModifier = 0
		end
	end
end

local function profileFieldOfView(): number
	if player:GetAttribute("OMFReducedMotion") == true then
		return 68
	end
	local family = player:GetAttribute("OMFFamily")
	if type(family) == "string" then
		local profile = GameConfig.Camera.Profiles[family]
		if profile then
			return profile.FieldOfView
		end
	end
	return GameConfig.Camera.FieldOfView
end

local function applyCameraProfile(camera: Camera)
	camera.FieldOfView = profileFieldOfView()
end

local function bindViewport(camera: Camera)
	if viewportConnection then
		viewportConnection:Disconnect()
		viewportConnection = nil
	end
	applyCameraProfile(camera)
	viewportConnection = camera:GetPropertyChangedSignal("ViewportSize"):Connect(function()
		applyCameraProfile(camera)
	end)
end

local function attachCharacter(character: Model?)
	local camera = Workspace.CurrentCamera
	if not camera then
		return
	end

	camera.CameraType = Enum.CameraType.Custom
	applyCameraProfile(camera)

	if not character then
		return
	end

	local humanoid = character:FindFirstChildOfClass("Humanoid")
	if humanoid then
		humanoid.CameraOffset = Vector3.new(0, 1.35, 0)
		camera.CameraSubject = humanoid
	end
	keepAvatarVisible(character)
end

local function configureCharacter(character: Model)
	local humanoid = character:WaitForChild("Humanoid", 10)
	if not humanoid or not humanoid:IsA("Humanoid") then
		return
	end

	humanoid.CameraOffset = Vector3.new(0, 1.35, 0)
	attachCharacter(character)
end

function CameraController.start()
	local camera = Workspace.CurrentCamera
	if camera then
		bindViewport(camera)
	end
	attachCharacter(player.Character)

	if player.Character then
		task.defer(configureCharacter, player.Character)
	end

	player.CharacterAdded:Connect(function(character)
		task.defer(configureCharacter, character)
	end)
	player.CharacterRemoving:Connect(function()
		attachCharacter(nil)
	end)

	player:GetAttributeChangedSignal("OMFFamily"):Connect(function()
		local current = Workspace.CurrentCamera
		if current then
			applyCameraProfile(current)
		end
	end)
	player:GetAttributeChangedSignal("OMFReducedMotion"):Connect(function()
		local current = Workspace.CurrentCamera
		if current then
			applyCameraProfile(current)
		end
	end)

	Workspace:GetPropertyChangedSignal("CurrentCamera"):Connect(function()
		local current = Workspace.CurrentCamera
		if current then
			bindViewport(current)
		end
		attachCharacter(player.Character)
	end)
end

return CameraController
