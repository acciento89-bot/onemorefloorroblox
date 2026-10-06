--!strict

local ReplicatedStorage = game:GetService("ReplicatedStorage")

local shared = ReplicatedStorage:WaitForChild("OneMoreFloor"):WaitForChild("Shared")
local ChallengeConfig = require(shared.config.ChallengeConfig)
local CosmeticConfig = require(shared.config.CosmeticConfig)
local GameConfig = require(shared.config.GameConfig)

local ChallengeFactory = {}

export type Mover = {
	Part: BasePart,
	BaseCFrame: CFrame,
	Axis: Vector3,
	Travel: number,
	Speed: number,
	Phase: number,
}

export type Spinner = {
	Part: BasePart,
	BaseCFrame: CFrame,
	Speed: number,
	Phase: number,
}

export type Runtime = {
	Model: Model,
	StartCFrame: CFrame,
	Exit: BasePart,
	Hazards: { BasePart },
	Movers: { Mover },
	Spinners: { Spinner },
	Switch: BasePart?,
	Gate: BasePart?,
	Tiles: { BasePart },
	Accent: Color3,
}

local FOUNDATION = Color3.fromRGB(34, 43, 58)
local STEEL = Color3.fromRGB(66, 83, 101)
local SAFE = Color3.fromRGB(112, 127, 143)
local WARM = Color3.fromRGB(244, 166, 80)
local HAZARD = Color3.fromRGB(236, 83, 94)

local function rgb(values): Color3
	return Color3.fromRGB(values[1], values[2], values[3])
end

local function makePart(
	parent: Instance,
	name: string,
	size: Vector3,
	cframe: CFrame,
	material: Enum.Material,
	color: Color3,
	canCollide: boolean
): Part
	local item = Instance.new("Part")
	item.Name = name
	item.Size = size
	item.CFrame = cframe
	item.Anchored = true
	item.CanCollide = canCollide
	item.CanTouch = true
	item.CanQuery = true
	item.CastShadow = true
	item.Material = material
	item.Color = color
	item.TopSurface = Enum.SurfaceType.Smooth
	item.BottomSurface = Enum.SurfaceType.Smooth
	item.Parent = parent
	return item
end

local function decorative(item: BasePart)
	item.CanCollide = false
	item.CanTouch = false
	item.CanQuery = false
end

local function labelPart(part: BasePart, text: string, accent: Color3)
	local surface = Instance.new("SurfaceGui")
	surface.Face = Enum.NormalId.Front
	surface.SizingMode = Enum.SurfaceGuiSizingMode.PixelsPerStud
	surface.PixelsPerStud = 42
	surface.Parent = part

	local label = Instance.new("TextLabel")
	label.Size = UDim2.fromScale(1, 1)
	label.BackgroundTransparency = 1
	label.Font = Enum.Font.GothamBlack
	label.TextScaled = true
	label.TextColor3 = accent
	label.Text = text
	label.Parent = surface
end

local function floorPart(parent: Instance, name: string, size: Vector3, position: Vector3): Part
	return makePart(parent, name, size, CFrame.new(position), Enum.Material.Concrete, SAFE, true)
end

local function addShell(
	model: Model,
	origin: Vector3,
	accent: Color3,
	themeAccent: Color3,
	title: string,
	floorNumber: number
)
	local width = GameConfig.Floor.RoomWidth
	local depth = GameConfig.Floor.RoomDepth
	local height = GameConfig.Floor.RoomHeight

	local under = makePart(
		model,
		"RoomUnderframe",
		Vector3.new(width + 2, 0.8, depth + 2),
		CFrame.new(origin - Vector3.new(0, 1.15, 0)),
		Enum.Material.Metal,
		FOUNDATION,
		false
	)
	decorative(under)

	for _, zSign in { -1, 1 } do
		local rail = makePart(
			model,
			"ThemeRail",
			Vector3.new(width - 2.2, 0.14, 0.18),
			CFrame.new(origin + Vector3.new(0, 0.59, zSign * (depth / 2 - 0.9))),
			Enum.Material.Neon,
			themeAccent,
			false
		)
		rail.Transparency = 0.12
		rail.CastShadow = false
		decorative(rail)
	end

	for _, side in { -1, 1 } do
		local wall = makePart(
			model,
			"SideStructure",
			Vector3.new(0.75, height, depth + 1),
			CFrame.new(origin + Vector3.new(side * (width / 2 + 0.5), height / 2 - 0.5, 0)),
			Enum.Material.Metal,
			STEEL,
			false
		)
		decorative(wall)
		for z = -12, 12, 8 do
			local light = makePart(
				model,
				"GuideLight",
				Vector3.new(0.2, 0.35, 2.8),
				CFrame.new(origin + Vector3.new(side * (width / 2 + 0.92), 3.1, z)),
				Enum.Material.Neon,
				themeAccent,
				false
			)
			light.Transparency = 0.18
			decorative(light)
		end
	end

	for _, side in { -1, 1 } do
		local glass = makePart(
			model,
			"RoomGlass",
			Vector3.new(0.14, 7.4, depth - 8),
			CFrame.new(origin + Vector3.new(side * (width / 2 + 0.14), 4.4, 0)),
			Enum.Material.Glass,
			Color3.fromRGB(116, 181, 208),
			false
		)
		glass.Transparency = 0.76
		glass.CastShadow = false
		decorative(glass)
	end

	for _, z in { -10, 0, 10 } do
		local ceilingLight = makePart(
			model,
			"CeilingLight",
			Vector3.new(17, 0.12, 0.28),
			CFrame.new(origin + Vector3.new(0, height - 0.45, z)),
			Enum.Material.Neon,
			if z == 0 then accent else themeAccent,
			false
		)
		ceilingLight.Transparency = 0.10
		ceilingLight.CastShadow = false
		decorative(ceilingLight)
	end

	for _, side in { -1, 1 } do
		local routeRail = makePart(
			model,
			"RouteRail",
			Vector3.new(0.16, 0.12, depth - 5),
			CFrame.new(origin + Vector3.new(side * 14.15, 0.64, 0)),
			Enum.Material.Neon,
			themeAccent,
			false
		)
		routeRail.Transparency = 0.28
		routeRail.CastShadow = false
		decorative(routeRail)
	end

	for _, lightOffset in
		{
			Vector3.new(-10, 8.8, -8),
			Vector3.new(10, 8.8, -8),
			Vector3.new(-10, 8.8, 8),
			Vector3.new(10, 8.8, 8),
		}
	do
		local practical = makePart(
			model,
			"RoomPractical",
			Vector3.new(2.8, 0.18, 0.55),
			CFrame.new(origin + lightOffset),
			Enum.Material.Neon,
			Color3.fromRGB(220, 235, 244),
			false
		)
		practical.Transparency = 0.08
		practical.CastShadow = false
		decorative(practical)

		local point = Instance.new("PointLight")
		point.Color = if lightOffset.Z < 0 then Color3.fromRGB(255, 221, 185) else themeAccent
		point.Brightness = 1.0
		point.Range = 18
		point.Shadows = false
		point.Parent = practical
	end

	for _, z in { -depth / 2 - 0.3, depth / 2 + 0.3 } do
		local beam = makePart(
			model,
			"PortalBeam",
			Vector3.new(width + 1.5, 0.75, 0.75),
			CFrame.new(origin + Vector3.new(0, height - 1.2, z)),
			Enum.Material.Metal,
			FOUNDATION,
			false
		)
		decorative(beam)
	end

	local titleBoard = makePart(
		model,
		"FloorIdentity",
		Vector3.new(7.8, 1.4, 0.45),
		CFrame.new(origin + Vector3.new(0, 9.2, -depth / 2 + 0.35)),
		Enum.Material.Metal,
		FOUNDATION,
		false
	)
	decorative(titleBoard)
	labelPart(titleBoard, ("F%02d  •  %s"):format(floorNumber, string.upper(title)), accent)

	for _, x in { -12, 12 } do
		for _, z in { -14, 14 } do
			local post = makePart(
				model,
				"CornerPost",
				Vector3.new(0.7, height, 0.7),
				CFrame.new(origin + Vector3.new(x, height / 2 - 0.5, z)),
				Enum.Material.Metal,
				STEEL,
				false
			)
			decorative(post)
		end
	end
end

local function addExit(model: Model, origin: Vector3, accent: Color3): Part
	local exit = makePart(
		model,
		"ExitTrigger",
		Vector3.new(9, 5, 2.2),
		CFrame.new(origin + Vector3.new(0, 2.5, 14.2)),
		Enum.Material.Neon,
		accent,
		false
	)
	exit.Transparency = 0.72
	exit.CanTouch = true

	local header = makePart(
		model,
		"ExitHeader",
		Vector3.new(10.5, 0.55, 0.55),
		CFrame.new(origin + Vector3.new(0, 5.4, 14.1)),
		Enum.Material.Neon,
		accent,
		false
	)
	decorative(header)
	local left = makePart(
		model,
		"ExitFrame",
		Vector3.new(0.55, 5.5, 0.55),
		CFrame.new(origin + Vector3.new(-5, 2.75, 14.1)),
		Enum.Material.Metal,
		accent,
		false
	)
	local right = makePart(
		model,
		"ExitFrame",
		Vector3.new(0.55, 5.5, 0.55),
		CFrame.new(origin + Vector3.new(5, 2.75, 14.1)),
		Enum.Material.Metal,
		accent,
		false
	)
	decorative(left)
	decorative(right)
	return exit
end

local function addHazard(runtime: Runtime, part: BasePart)
	part:SetAttribute("OneMoreFloorHazard", true)
	part.CanTouch = true
	table.insert(runtime.Hazards, part)
end

local function buildPrecision(runtime: Runtime, origin: Vector3, params: any)
	local startDepth = 10
	local landingDepth = 9
	local startCenterZ = -10
	local startFrontEdge = startCenterZ + startDepth / 2
	local landingCenterZ = startFrontEdge + params.Gap + landingDepth / 2

	floorPart(runtime.Model, "StartDeck", Vector3.new(24, 1, startDepth), origin + Vector3.new(0, 0, startCenterZ))
	local landing = floorPart(
		runtime.Model,
		"LandingDeck",
		Vector3.new(params.LandingWidth, 1, landingDepth),
		origin + Vector3.new(params.Lateral, 0, landingCenterZ)
	)
	local target = makePart(
		runtime.Model,
		"LandingTarget",
		Vector3.new(math.max(2.2, params.LandingWidth - 2.2), 0.08, 3.4),
		landing.CFrame + Vector3.new(0, 0.55, 0),
		Enum.Material.Neon,
		runtime.Accent,
		false
	)
	target.Transparency = 0.08
	decorative(target)
	floorPart(runtime.Model, "ExitDeck", Vector3.new(18, 1, 6), origin + Vector3.new(0, 0, 13))
end

local function buildMovingPlatform(runtime: Runtime, origin: Vector3, params: any)
	floorPart(runtime.Model, "StartDeck", Vector3.new(24, 1, 8), origin + Vector3.new(0, 0, -12))
	floorPart(runtime.Model, "ExitDeck", Vector3.new(24, 1, 8), origin + Vector3.new(0, 0, 12))
	local mover = floorPart(runtime.Model, "MovingPlatform", Vector3.new(params.PlatformWidth, 0.9, 6), origin)
	mover.Material = Enum.Material.Metal
	mover.Color = runtime.Accent
	table.insert(runtime.Movers, {
		Part = mover,
		BaseCFrame = mover.CFrame,
		Axis = Vector3.new(1, 0, 0),
		Travel = params.Travel,
		Speed = params.Speed,
		Phase = 0,
	})
end

local function buildDodge(runtime: Runtime, origin: Vector3, params: any)
	floorPart(runtime.Model, "RunDeck", Vector3.new(28, 1, 30), origin)
	for index = 1, params.GateCount do
		local z = -10 + (index - 1) * (20 / math.max(1, params.GateCount - 1))
		local side = if index % 2 == 0 then 1 else -1
		local openingCenter = side * 5
		local openingHalf = params.OpeningWidth / 2
		local leftWidth = 14 + openingCenter - openingHalf
		local rightWidth = 14 - openingCenter - openingHalf
		if leftWidth > 0.5 then
			local left = makePart(
				runtime.Model,
				"HazardGate",
				Vector3.new(leftWidth, 5, 1.0),
				CFrame.new(origin + Vector3.new(-14 + leftWidth / 2, 2.5, z)),
				Enum.Material.Metal,
				HAZARD,
				true
			)
			addHazard(runtime, left)
		end
		if rightWidth > 0.5 then
			local right = makePart(
				runtime.Model,
				"HazardGate",
				Vector3.new(rightWidth, 5, 1.0),
				CFrame.new(origin + Vector3.new(14 - rightWidth / 2, 2.5, z)),
				Enum.Material.Metal,
				HAZARD,
				true
			)
			addHazard(runtime, right)
		end
	end
end

local function buildNarrow(runtime: Runtime, origin: Vector3, params: any)
	floorPart(runtime.Model, "StartDeck", Vector3.new(20, 1, 7), origin + Vector3.new(0, 0, -13))
	local segmentLength = 22 / params.SegmentCount
	for index = 1, params.SegmentCount do
		local alpha = (index - 0.5) / params.SegmentCount
		local z = -9 + alpha * 20
		local direction = if index % 2 == 0 then 1 else -1
		local x = direction * params.Turn
		floorPart(
			runtime.Model,
			"NarrowPath",
			Vector3.new(params.PathWidth, 1, segmentLength + 0.7),
			origin + Vector3.new(x, 0, z)
		)
	end
	floorPart(runtime.Model, "ExitDeck", Vector3.new(20, 1, 7), origin + Vector3.new(0, 0, 13))
end

local function buildSwitch(runtime: Runtime, origin: Vector3, params: any)
	floorPart(runtime.Model, "RunDeck", Vector3.new(28, 1, 30), origin)
	local switch = makePart(
		runtime.Model,
		"FloorSwitch",
		Vector3.new(3.2, 0.35, 3.2),
		CFrame.new(origin + Vector3.new(params.SwitchSide * params.SwitchOffset, 0.68, -5)),
		Enum.Material.Neon,
		runtime.Accent,
		true
	)
	runtime.Switch = switch
	local gate = makePart(
		runtime.Model,
		"LockedGate",
		Vector3.new(26, 6, 1),
		CFrame.new(origin + Vector3.new(0, 3, 5)),
		Enum.Material.Metal,
		Color3.fromRGB(84, 91, 103),
		true
	)
	gate:SetAttribute("RequiresSwitch", true)
	runtime.Gate = gate
end

local function buildTiles(runtime: Runtime, origin: Vector3, params: any)
	floorPart(runtime.Model, "StartDeck", Vector3.new(20, 1, 6), origin + Vector3.new(0, 0, -14))
	floorPart(runtime.Model, "ExitDeck", Vector3.new(20, 1, 6), origin + Vector3.new(0, 0, 14))
	for index = 1, params.TileCount do
		local alpha = (index - 0.5) / params.TileCount
		local z = -10 + alpha * 20
		local x = if index % 2 == 0 then 2.0 else -2.0
		local tile = floorPart(
			runtime.Model,
			"FadingTile",
			Vector3.new(params.TileWidth, 0.85, 4.2),
			origin + Vector3.new(x, 0, z)
		)
		tile:SetAttribute("HoldSeconds", params.TileHoldSeconds)
		table.insert(runtime.Tiles, tile)
	end
end

local function buildMovingWall(runtime: Runtime, origin: Vector3, params: any)
	floorPart(runtime.Model, "RunDeck", Vector3.new(28, 1, 30), origin)
	local openingHalf = params.OpeningWidth / 2
	local segmentWidth = (28 - params.OpeningWidth) / 2
	for _, side in { -1, 1 } do
		local x = side * (openingHalf + segmentWidth / 2)
		local wall = makePart(
			runtime.Model,
			"MovingWall",
			Vector3.new(segmentWidth, 6, 1.1),
			CFrame.new(origin + Vector3.new(x, 3, 1)),
			Enum.Material.Metal,
			HAZARD,
			true
		)
		addHazard(runtime, wall)
		table.insert(runtime.Movers, {
			Part = wall,
			BaseCFrame = wall.CFrame,
			Axis = Vector3.new(1, 0, 0),
			Travel = params.Travel,
			Speed = params.WallSpeed,
			Phase = if side < 0 then 0 else math.pi,
		})
	end
end

local function buildLaserSweep(runtime: Runtime, origin: Vector3, params: any)
	local deck = floorPart(runtime.Model, "RunDeck", Vector3.new(28, 1, 30), origin)
	deck.Color = Color3.fromRGB(88, 104, 122)

	local hub = makePart(
		runtime.Model,
		"LaserHub",
		Vector3.new(1.2, 2.2, 1.2),
		CFrame.new(origin + Vector3.new(0, 1.6, 0)),
		Enum.Material.Metal,
		Color3.fromRGB(54, 64, 80),
		false
	)
	decorative(hub)

	local hubGlow = makePart(
		runtime.Model,
		"LaserHubGlow",
		Vector3.new(1.45, 0.16, 1.45),
		CFrame.new(origin + Vector3.new(0, 2.75, 0)),
		Enum.Material.Neon,
		runtime.Accent,
		false
	)
	hubGlow.Transparency = 0.08
	hubGlow.CastShadow = false
	decorative(hubGlow)

	local beamCFrame = CFrame.new(origin + Vector3.new(0, params.BeamHeight, 0))
	local beam = makePart(
		runtime.Model,
		"LaserSweepBeam",
		Vector3.new(params.BeamLength, 0.48, 0.62),
		beamCFrame,
		Enum.Material.Neon,
		runtime.Accent,
		false
	)
	beam.Transparency = 0.04
	beam.CastShadow = false
	addHazard(runtime, beam)
	table.insert(runtime.Spinners, {
		Part = beam,
		BaseCFrame = beamCFrame,
		Speed = params.SweepSpeed,
		Phase = params.Phase,
	})

	for _, z in { -10, 10 } do
		local safeMark = makePart(
			runtime.Model,
			"SafeMark",
			Vector3.new(8, 0.08, 3.0),
			CFrame.new(origin + Vector3.new(0, 0.55, z)),
			Enum.Material.Neon,
			Color3.fromRGB(105, 214, 151),
			false
		)
		safeMark.Transparency = 0.32
		decorative(safeMark)
	end
end

function ChallengeFactory.build(
	parent: Instance,
	selection: any,
	params: any,
	origin: Vector3,
	themeId: string?
): Runtime
	local model = Instance.new("Model")
	model.Name = ("Floor_%03d_%s"):format(selection.Floor, selection.FamilyId)
	model:SetAttribute("FloorNumber", selection.Floor)
	model:SetAttribute("FamilyId", selection.FamilyId)
	model:SetAttribute("Variant", selection.Variant)
	model.Parent = parent

	local definition = assert(ChallengeConfig.Families[selection.FamilyId], "unknown family")
	local accent = rgb(definition.AccentRGB)
	local themeItem = CosmeticConfig.Items[themeId or "theme_foundry"]
	local themeAccent = if themeItem and themeItem.Category == "Theme" then rgb(themeItem.ColorRGB) else WARM
	model:SetAttribute("ThemeId", if themeItem then themeItem.Id else "theme_foundry")
	local runtime: Runtime = {
		Model = model,
		StartCFrame = CFrame.lookAt(origin + Vector3.new(0, 3.2, -13), origin + Vector3.new(0, 3.2, 10)),
		Exit = nil :: any,
		Hazards = {},
		Movers = {},
		Spinners = {},
		Switch = nil,
		Gate = nil,
		Tiles = {},
		Accent = accent,
	}

	addShell(model, origin, accent, themeAccent, definition.DisplayName, selection.Floor)

	if selection.FamilyId == "precision_jump" then
		buildPrecision(runtime, origin, params)
	elseif selection.FamilyId == "moving_platform" then
		buildMovingPlatform(runtime, origin, params)
	elseif selection.FamilyId == "dodge_gates" then
		buildDodge(runtime, origin, params)
	elseif selection.FamilyId == "narrow_path" then
		buildNarrow(runtime, origin, params)
	elseif selection.FamilyId == "switch_dash" then
		buildSwitch(runtime, origin, params)
	elseif selection.FamilyId == "disappearing_tiles" then
		buildTiles(runtime, origin, params)
	elseif selection.FamilyId == "moving_wall" then
		buildMovingWall(runtime, origin, params)
	elseif selection.FamilyId == "laser_sweep" then
		buildLaserSweep(runtime, origin, params)
	else
		error("unsupported family: " .. tostring(selection.FamilyId))
	end

	runtime.Exit = addExit(model, origin, accent)

	local startGuide = makePart(
		model,
		"StartGuide",
		Vector3.new(7, 0.08, 2.4),
		CFrame.new(origin + Vector3.new(0, 0.58, -13)),
		Enum.Material.Neon,
		WARM,
		false
	)
	startGuide.Transparency = 0.12
	decorative(startGuide)

	return runtime
end

return ChallengeFactory
