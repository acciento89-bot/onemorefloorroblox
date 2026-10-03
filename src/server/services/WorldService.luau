--!strict

local Lighting = game:GetService("Lighting")
local Workspace = game:GetService("Workspace")

local ReplicatedStorage = game:GetService("ReplicatedStorage")
local shared = ReplicatedStorage:WaitForChild("OneMoreFloor"):WaitForChild("Shared")
local GameConfig = require(shared.config.GameConfig)

local WorldService = {}

export type Lane = {
	Model: Model,
	LaneIndex: number,
	Origin: Vector3,
	Environment: Folder,
}

local worldRoot: Folder? = nil
local lightingConfigured = false

local function makePart(
	parent: Instance,
	name: string,
	size: Vector3,
	cframe: CFrame,
	material: Enum.Material,
	color: Color3,
	transparency: number?
): Part
	local item = Instance.new("Part")
	item.Name = name
	item.Size = size
	item.CFrame = cframe
	item.Anchored = true
	item.CanCollide = false
	item.CanTouch = false
	item.CanQuery = false
	item.CastShadow = true
	item.Material = material
	item.Color = color
	item.Transparency = transparency or 0
	item.TopSurface = Enum.SurfaceType.Smooth
	item.BottomSurface = Enum.SurfaceType.Smooth
	item.Parent = parent
	return item
end

local function configureLighting()
	if lightingConfigured then
		return
	end
	lightingConfigured = true

	Lighting.ClockTime = 18.1
	Lighting.Brightness = 2.0
	Lighting.ExposureCompensation = 0.02
	Lighting.GlobalShadows = true
	Lighting.ShadowSoftness = 0.28
	Lighting.Ambient = Color3.fromRGB(66, 76, 91)
	Lighting.OutdoorAmbient = Color3.fromRGB(112, 121, 136)

	local atmosphere = Lighting:FindFirstChildOfClass("Atmosphere") or Instance.new("Atmosphere")
	atmosphere.Density = 0.24
	atmosphere.Haze = 1.25
	atmosphere.Glare = 0.08
	atmosphere.Color = Color3.fromRGB(201, 220, 231)
	atmosphere.Decay = Color3.fromRGB(93, 112, 135)
	atmosphere.Parent = Lighting

	local bloom = Lighting:FindFirstChildOfClass("BloomEffect") or Instance.new("BloomEffect")
	bloom.Intensity = 0.14
	bloom.Size = 18
	bloom.Threshold = 1.7
	bloom.Parent = Lighting

	local color = Lighting:FindFirstChildOfClass("ColorCorrectionEffect") or Instance.new("ColorCorrectionEffect")
	color.Brightness = 0.01
	color.Contrast = 0.08
	color.Saturation = -0.03
	color.TintColor = Color3.fromRGB(244, 248, 255)
	color.Parent = Lighting

	local clouds = Workspace.Terrain:FindFirstChildOfClass("Clouds") or Instance.new("Clouds")
	clouds.Cover = 0.22
	clouds.Density = 0.32
	clouds.Color = Color3.fromRGB(235, 241, 245)
	clouds.Parent = Workspace.Terrain
end

local function decorateLane(lane: Lane)
	local art = lane.Environment
	local origin = lane.Origin
	local foundation = Color3.fromRGB(21, 28, 38)
	local steel = Color3.fromRGB(45, 59, 73)
	local warm = Color3.fromRGB(244, 166, 80)

	for _, side in { -1, 1 } do
		local x = origin.X + side * 24
		makePart(
			art,
			"TowerSpine",
			Vector3.new(1.5, 520, 1.5),
			CFrame.new(x, 250, origin.Z),
			Enum.Material.Metal,
			steel
		)
		for y = 18, 500, 32 do
			makePart(
				art,
				"TowerRib",
				Vector3.new(8.5, 0.55, 0.72),
				CFrame.new(origin.X + side * 20.5, y, origin.Z + ((y / 32) % 2) * 3),
				Enum.Material.Metal,
				foundation
			)
		end
	end

	for index = 1, 11 do
		local angle = index / 11 * math.pi * 2
		local radius = 82 + (index % 3) * 14
		local height = 36 + ((index * 7) % 5) * 12
		local position = origin + Vector3.new(math.cos(angle) * radius, -14, math.sin(angle) * radius)
		makePart(
			art,
			"CityMass",
			Vector3.new(12 + (index % 3) * 4, height, 14 + ((index + 1) % 3) * 3),
			CFrame.new(position + Vector3.new(0, height / 2, 0)),
			Enum.Material.Concrete,
			Color3.fromRGB(31 + index, 38 + index, 49 + index)
		)
	end

	local beacon = makePart(
		art,
		"TowerBeacon",
		Vector3.new(4.5, 0.32, 4.5),
		CFrame.new(origin + Vector3.new(0, 235, -48)),
		Enum.Material.Neon,
		warm,
		0.12
	)
	beacon.CastShadow = false
	local light = Instance.new("PointLight")
	light.Color = warm
	light.Brightness = 0.9
	light.Range = 18
	light.Shadows = false
	light.Parent = beacon
end

function WorldService.start()
	configureLighting()
	local existing = Workspace:FindFirstChild("OneMoreFloorWorld")
	if existing then
		existing:Destroy()
	end
	local root = Instance.new("Folder")
	root.Name = "OneMoreFloorWorld"
	root.Parent = Workspace
	worldRoot = root
end

function WorldService.createLane(player: Player, laneIndex: number): Lane
	assert(worldRoot, "WorldService.start must run first")
	local lane = Instance.new("Model")
	lane.Name = ("Lane_%d"):format(player.UserId)
	lane:SetAttribute("OwnerUserId", player.UserId)
	lane.Parent = worldRoot

	local environment = Instance.new("Folder")
	environment.Name = "EnvironmentArt"
	environment.Parent = lane
	local value: Lane = {
		Model = lane,
		LaneIndex = laneIndex,
		Origin = Vector3.new((laneIndex - 1) * 180, 8, 0),
		Environment = environment,
	}
	decorateLane(value)
	return value
end

function WorldService.floorOrigin(lane: Lane, floorNumber: number): Vector3
	local floor = math.max(1, math.floor(floorNumber))
	return lane.Origin + Vector3.new(0, (floor - 1) * GameConfig.Floor.FloorRise, 0)
end

function WorldService.destroyLane(lane: Lane)
	if lane.Model.Parent then
		lane.Model:Destroy()
	end
end

return WorldService
