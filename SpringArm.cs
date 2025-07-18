using Godot;
using System;

public partial class SpringArm : SpringArm3D
{
	private float mouse_sensitivity = 0.05f;
	private bool focus = false;

	public override void _Ready()
	{
		Vector2 m = GetViewport().GetMousePosition();
		if (m.X < GetWindow().GetSize().X && m.Y < GetWindow().GetSize().Y && m.X > 0 && m.Y > 0)
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
			focus = true;
		}
		RotationDegrees = new Vector3(RotationDegrees.X, RotationDegrees.Y + 180, RotationDegrees.Z);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		Vector2 m = GetViewport().GetMousePosition();
		if (focus && m.X < GetWindow().GetSize().X && m.Y < GetWindow().GetSize().Y && m.X > 0 && m.Y > 0)
		{
			var Player = GetTree().CurrentScene.GetNode<Node3D>("Player");
			var Camera = GetTree().CurrentScene.GetNode<Camera3D>("Player/Camera3D");

			if (@event is InputEventMouseMotion eventMouseMotion)
			{
				Vector3 currentRotation = RotationDegrees;
				currentRotation.X -= eventMouseMotion.Relative.Y * mouse_sensitivity;
				currentRotation.X = Mathf.Clamp(currentRotation.X, -90, 60);
				RotationDegrees = currentRotation;
				Vector3 cameraRotation = Camera.RotationDegrees;
				cameraRotation.X -= eventMouseMotion.Relative.Y * mouse_sensitivity;
				cameraRotation.X = Mathf.Clamp(cameraRotation.X, -90, (Camera.Current ? 90 : 60));
				Camera.RotationDegrees = cameraRotation;
				Vector3 playerRotation = Player.RotationDegrees;
				playerRotation.Y -= eventMouseMotion.Relative.X * mouse_sensitivity;
				playerRotation.Y = Mathf.Wrap(playerRotation.Y, 0, 360);
				Player.RotationDegrees = playerRotation;
			}
		}
	}

	public override void _Process(double delta)
	{
		Vector2 m = GetViewport().GetMousePosition();
		if (m.X < GetWindow().GetSize().X && m.Y < GetWindow().GetSize().Y && m.X > 0 && m.Y > 0)
		{
			if (focus || Input.IsMouseButtonPressed(MouseButton.Left) || Input.IsMouseButtonPressed(MouseButton.Right))
			{
				focus = true;
				Input.MouseMode = Input.MouseModeEnum.Captured;
			}
		}
		else
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		var CameraTPS = GetNode<Camera3D>("Camera");
		var CameraFPS = GetNode<Camera3D>("../Camera3D");

		if (CameraTPS.Current && CameraFPS.RotationDegrees.X > 60)
		{
			CameraFPS.RotationDegrees = new(Mathf.Clamp(RotationDegrees.X, -90, 60), CameraFPS.RotationDegrees.Y, CameraFPS.RotationDegrees.Z);
		}
	
		if(Input.IsActionJustPressed("fps_tps"))
		{
			CameraFPS.Current = !CameraFPS.Current;
			CameraTPS.Current = !CameraFPS.Current;
		}
	}
	/*
	public override void _Notification(int what)
	{
		if (what == MainLoop.NotificationApplicationFocusOut)
		{
			focus = false;
		}
		if (what == MainLoop.NotificationApplicationFocusIn)
		{
			focus = true;
		}
	}*/
}