--!strict

local ReplicatedStorage = game:GetService("ReplicatedStorage")

ReplicatedStorage:WaitForChild("OneMoreFloorRemotes")

local controllers = script:WaitForChild("controllers")
local CameraController = require(controllers.CameraController)
local HudController = require(controllers.HudController)

CameraController.start()
HudController.start()

print("[One More Floor] Client initialized")
