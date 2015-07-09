import System.Math as Math

cell.SetZeroKill(-100)

rand = Random()

sky = ObjVolume.LoadFromFile(me, 'Models/sky.obj')
sky.Scale = Vector3(200,200,200)
sky.meshes[0].Shader = 'texture'
me.AddModel(sky)

old = ObjVolume.LoadFromFile(me, 'Models/wpo.obj')
old.meshes[0].Shader = 'light'
me.AddModel(old)

def OnStart():
	cam.Move(0.0,5.0,0.0)
	
def OnKeyDown(key):
	if key == 'P':
		game.Quit()
	if key == 'Z':
		print('42 ')

def OnUpdate():
	global sky
	input.TickCursor()
	sky.Position = cam.Position
	
	if input.GetState('W'):
		cam.Move(0.0,1.0,0.0)
	if input.GetState('S'):
		cam.Move(0.0,-1.0,0.0)
	if input.GetState('A'):
		cam.Move(-1.0,0.0,0.0)
	if input.GetState('D'):
		cam.Move(1.0,0.0,0.0)
	if input.GetState('Q'):
		cam.Move(0.0,0.0,-1.0)
	if input.GetState('E'):
		cam.Move(0.0,0.0,1.0)


	cam.AddRotation(game.InputManager.Axis.X,game.InputManager.Axis.Y)