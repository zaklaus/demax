import System.Math as Math

tick = 0
cell.SetZeroKill(0)

cubes = []



rand = Random()

def OnStart():
	floor = TexturedCube(me)
	floor.Position = Vector3(0, 0, 0)
	floor.Scale = Vector3(100, 1, 100)
	floor.LoadTexture('Textures/img03.jpg')
	floor.AddRigidbody()
	floor.SetStatic(True)
	me.AddModel(floor)

	test = ObjVolume.LoadFromFile(me, 'Models/multimesh.obj')
	test.Position = Vector3(0,3,0)
	me.AddModel(test)

def GetCube():
	for x in range(len(cubes)):
		if cubes[x].isVisible == False:
			return cubes[x]
	return None

def OnUpdate():
	if game.InputManager.GetState(Key_W) == True:
		cam.Move(0.0,1.0,0.0)
	if game.InputManager.GetState(Key_A) == True:
		cam.Move(-1.0,0.0,0.0)
	if game.InputManager.GetState(Key_S) == True:
		cam.Move(0.0,-1.0,0.0)
	if game.InputManager.GetState(Key_D) == True:
		cam.Move(1.0,0.0,0.0)
	if game.InputManager.GetState(Key_Q) == True:
		cam.Move(0.0,0.0,-1.0)
	if game.InputManager.GetState(Key_E) == True:
		cam.Move(0.0,0.0,1.0)
	if input.GetState(Key_P) == True:
		game.Quit()
	if input.GetState(Key_N) == True:
		cube = TexturedCube(me)
		cube.Position = Vector3 (rand.NextDouble()*50.0 - 25.0, rand.NextDouble()*80.0 + 5.0, rand.NextDouble()*50.0 - 25.0)
		cube.LoadTexture('Textures/img02.jpg')
		cube.AddRigidbody ()
		me.AddModel(cube)
	if input.GetState(Key_M) == True:
		cube = TexturedCube(me)
		cube.LoadTexture('Textures/img02.jpg')
		cube.Position = Vector3 (rand.NextDouble()*0.15,15,rand.NextDouble()*0.15)
		cube.AddRigidbody ()
		cube.isVisible = True
		me.AddModel(cube)

	cam.AddRotation(game.InputManager.Axis.X,game.InputManager.Axis.Y)