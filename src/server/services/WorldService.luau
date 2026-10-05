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

	Lighting.ClockTime = 17.35
	Lighting.Brightness = 2.35
	Lighting.ExposureCompensation = 0.16
	Lighting.GlobalShadows = true
	Lighting.ShadowSoftness = 0.28
	Lighting.Ambient = Color3.fromRGB(84, 96, 112)
	Lighting.OutdoorAmbient = Color3.fromRGB(132, 140, 154)

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
	color.Brightness = 0.025
	color.Contrast = 0.05
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
	local foundation = Color3.fromRGB(35, 45, 58)
	local steel = Color3.fromRGB(63, 82, 96)
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
			Color3.fromRGB(48 + index, 56 + index, 68 + index)
		)
	end

	local cityRing = Instance.new("Folder")
	cityRing.Name = "TowerDepthRing"
	cityRing.Parent = art
	for index = 1, 14 do
		local angle = index / 14 * math.pi * 2 + 0.16
		local radius = 136 + (index % 4) * 15
		local height = 42 + ((index * 9) % 6) * 11
		local position = origin + Vector3.new(math.cos(angle) * radius, -24, math.sin(angle) * radius)
		makePart(
			cityRing,
			"FarCityBlock",
			Vector3.new(15 + (index % 3) * 4, height, 16 + ((index + 1) % 3) * 3),
			CFrame.new(position + Vector3.new(0, height / 2, 0)),
			Enum.Material.Concrete,
			Color3.fromRGB(62 + index, 72 + index, 87 + index)
		)
	end

	local cloudRing = Instance.new("Folder")
	cloudRing.Name = "CloudRing"
	cloudRing.Parent = art
	for band, y in { 92, 218, 344 } do
		for index = 1, 8 do
			local angle = index / 8 * math.pi * 2 + band * 0.18
			local cloud = makePart(
				cloudRing,
				"CloudWisp",
				Vector3.new(13 + (index % 3) * 3, 3.2, 7 + (index % 2) * 2),
				CFrame.new(origin + Vector3.new(math.cos(angle) * 54, y, math.sin(angle) * 54)),
				Enum.Material.SmoothPlastic,
				Color3.fromRGB(235, 241, 248),
				0.58
			)
			cloud.Shape = Enum.PartType.Ball
			cloud.CastShadow = false
		end
	end

	local crown = Instance.new("Folder")
	crown.Name = "HeroTowerCrown"
	crown.Parent = art
	for _, side in { -1, 1 } do
		makePart(
			crown,
			"CrownPillar",
			Vector3.new(1.4, 15, 1.4),
			CFrame.new(origin + Vector3.new(side * 8.5, 248, -48)),
			Enum.Material.Marble,
			steel
		)
	end
	makePart(
		crown,
		"CrownBeam",
		Vector3.new(19, 1.4, 1.5),
		CFrame.new(origin + Vector3.new(0, 255.5, -48)),
		Enum.Material.Marble,
		steel
	)
	local crownGlow = makePart(
		crown,
		"CrownGlow",
		Vector3.new(14, 0.18, 0.28),
		CFrame.new(origin + Vector3.new(0, 256.3, -48.2)),
		Enum.Material.Neon,
		warm,
		0.08
	)
	crownGlow.CastShadow = false

	local sun = makePart(
		art,
		"SkySunDisc",
		Vector3.new(24, 24, 24),
		CFrame.new(origin + Vector3.new(78, 210, -180)),
		Enum.Material.Neon,
		Color3.fromRGB(255, 193, 118),
		0.10
	)
	sun.Shape = Enum.PartType.Ball
	sun.CastShadow = false

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
