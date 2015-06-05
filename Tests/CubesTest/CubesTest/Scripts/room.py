import System.Math as Math

tick = 0.00
cell.SetZeroKill(-100)

cubes = []

htest = ''

rotY = 1

rand = Random()

sky = ObjVolume.LoadFromFile(me, 'Models/sky.obj')
sky.Scale = Vector3(200,200,200)
sky.meshes[0].Shader = 'texture'
me.AddModel(sky)

def OnStart():
	print('Press N for random cube spawn')
	print('Press M for weird cube spawn')
	print('Press P to exit game')

	#z = ObjVolume.LoadFromFileAnim(me, 'test', 'Models/anim', 2)
	#z.Position = Vector3(0, 0, 0)
	#me.AddModel(z)

	#z.PlayAnim('test')

	for x in range(0,5):
		for y in range(0,5):
			f = ObjVolume.LoadFromFile(me, 'Models/room.obj')
			f.Position = Vector3(x*15, -10, y*15)
			f.meshes[0].Shader = 'lightmap'
			cubes.append(f)
			me.AddModel(f)
	rotY = 0
	

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
	if input.GetState(Key_G) == True:
		pass


	cam.AddRotation(game.InputManager.Axis.X,game.InputManager.Axis.Y)