import System.Math as Math

cell.SetZeroKill(-100)

rand = Random()

for x in range(30):
	old = ObjVolume.ImportModel(me, 'Models/wpo')
	old.meshes[0].Shader = 'light'
	old.Position = Vector3(0+x*30.0,0.0,-50.0)
	old.AddLOD('Models/wpo_low', 20, 'mod')
	me.AddModel(old)

colac = ObjVolume.LoadOBJ(me, 'Models/untitled')
colac.Scale = Vector3(10, 10, 10)
me.AddModel(colac)

for x in range(5):
	anim = ObjVolume.ImportAnim(me, 'explode', 'Models/anim', 0)
	anim.PlayAnim('explode')
	anim.FrameStep(0)
	anim.SetPosition(Vector3(-5+x*4.0,0.0,-10.0), True)
	me.AddModel(anim)

def OnStart():
	cam.Move(0.0,5.0,0.0)
	
def OnKeyDown(key):
	if key == 'P':
		game.Quit()
	if key == 'Z':
		print('42 ')

def OnUpdate():
	#global sky
	input.TickCursor()
	#sky.Position = cam.Position
	
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