--!strict

local ReplicatedStorage = game:GetService("ReplicatedStorage")

ReplicatedStorage:WaitForChild("OneMoreFloorRemotes")

local controllers = script:WaitForChild("controllers")
local CameraController = require(controllers.CameraController)
local CosmeticController = require(controllers.CosmeticController)
local HudController = require(controllers.HudController)
local MonetizationController = require(controllers.MonetizationController)

CameraController.start()
HudController.start()
CosmeticController.start()
MonetizationController.start()

print("[One More Floor] Client initialized")
