rotY = 0.0
fov = 90.0
monkey = None

x=0

def OnStart():
	global monkey
	print("Hello Level!")

def OnUpdate():
	global rotY, fov, monkey, x
	monkey = em.GetEntityByName('monkey')
	rotY += 0.1

	if monkey != None:
		monkey.SetPosition(Vector3(Math.Cos(rotY)*4.0 + x,Math.Sin(rotY)*4.0,-10.0) + Vector3(0,2.5,0))

	cam.LookAtf(monkey.Transform.Position - cam.Position)

	#cam.AddRotation(Math.Sin(rotY), Math.Cos(rotY), 0.1)
	#cam.FieldOfView = MathHelper.Clamp(fov + Math.Sin(rotY) * 10,30,120)

	if input.GetState(Key_A):
		x+=1
	if input.GetState(Key_D):
		x-=1