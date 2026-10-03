--!strict

local ReplicatedStorage = game:GetService("ReplicatedStorage")

local shared = ReplicatedStorage:WaitForChild("OneMoreFloor"):WaitForChild("Shared")
local RemoteDefinitions = require(shared.remotes.RemoteDefinitions)

local existing = ReplicatedStorage:FindFirstChild("OneMoreFloorRemotes")
if existing then
	existing:Destroy()
end

local remotes = Instance.new("Folder")
remotes.Name = "OneMoreFloorRemotes"
remotes.Parent = ReplicatedStorage

for _, definition in RemoteDefinitions do
	local instance = if definition.Kind == "Event" then Instance.new("RemoteEvent") else Instance.new("RemoteFunction")
	instance.Name = definition.Name
	instance.Parent = remotes
end

local WorldService = require(script.services.WorldService)
local ProfileService = require(script.services.ProfileService)
local FloorService = require(script.services.FloorService)

ProfileService.start()
WorldService.start()
FloorService.start()

print("[One More Floor] Server initialized")
