local fs = require("@lune/fs")

return function()
	local source = fs.readFile("src/client/controllers/CameraController.luau")

	assert(
		string.find(source, "camera.CameraType = Enum.CameraType.Custom", 1, true),
		"One More Floor camera must stay player-controlled"
	)
	assert(
		not string.find(source, "Enum.CameraType.Scriptable", 1, true),
		"camera must not force Scriptable follow/yaw"
	)
	assert(not string.find(source, "BindToRenderStep", 1, true), "camera must not overwrite view direction every frame")
	assert(
		not string.find(source, "camera.CFrame =", 1, true),
		"camera controller must not replace player camera rotation"
	)
end
