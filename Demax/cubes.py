import clr
import System.Math as Math

tick = 0

def OnStart():
	pass

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
	cam.AddRotation(game.InputManager.Axis.X,game.InputManager.Axis.Y)