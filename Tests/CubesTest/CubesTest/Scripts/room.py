import System.Math as Math

tick = 0
cell.SetZeroKill(-100)

cubes = []

htest = ''

rand = Random()

sky = ObjVolume.LoadFromFile(me, 'Models/sky.obj')
sky.Scale = Vector3(200,200,200)
sky.meshes[0].Shader = 'texture'
me.AddModel(sky)

def OnStart():
	print('Press N for random cube spawn')
	print('Press M for weird cube spawn')
	print('Press P to exit game')
	floor = ObjVolume.LoadFromFile(me, 'Models/room.obj')
	floor.Position = Vector3(0, 0, 0)
	floor.AddRigidbody()
	floor.SetStatic(True)
	floor.meshes[0].Shader = 'lightmap'
	me.AddModel(floor)

def OnUpdate():
	input.TickCursor()
	sky.Position = cam.Position
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