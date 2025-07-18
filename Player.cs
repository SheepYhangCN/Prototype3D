using Godot;
using System;

public partial class Player : CharacterBody3D
{
	bool flashlight = false;
	[Export] float Speed = 5.0f;
	[Export] bool gravity_toggle = true;
	[Export] bool overlay = true;
	[Export] bool collision = true;
	Vector3 original_position = Vector3.Zero;
	Vector3 original_rotation = Vector3.Zero;

	float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _Ready()
	{
		original_position = Position;
		original_rotation = Rotation;
	}

	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("overlay")){overlay=!overlay;}
		if(Input.IsActionJustPressed("gravity")){gravity_toggle=!gravity_toggle;}
		if(Input.IsActionJustPressed("flashlight")){flashlight=!flashlight;}
		if(Input.IsActionJustPressed("collision")){collision=!collision;}

		var Camera=GetNode<SpringArm3D>("SpringArm");
		var CameraTPS=Camera.GetNode<Camera3D>("Camera");
		var CameraFPS=GetNode<Camera3D>("Camera3D");
		var light = GetNode<SpotLight3D>("SpotLight3D");

		if (Input.IsActionJustPressed("reset"))
		{
			gravity_toggle = true;
			collision = true;
			flashlight = false;
			Position = original_position;
			Rotation = original_rotation;
			Velocity = Vector3.Zero;
			Speed = 5.0f;
			CameraFPS.Fov = 90;
			CameraTPS.Fov = 90;
			light.LightEnergy = 5.0f;
		}

		light.Rotation = CameraFPS.Rotation;
		light.Visible = flashlight;
		light.LightEnergy += (Input.IsActionPressed("flashup") ? 0.5f : 0) + (Input.IsActionPressed("flashdown") ? -0.5f : 0);

		GetNode<CollisionShape3D>("CollisionShape3D").Disabled = !collision;

		double FPS=Engine.GetFramesPerSecond();
		if(overlay)
		{
			GetNode<Label>("Label").Text="FPS:"+FPS.ToString() + 
			"\nGravity toggle: " + gravity_toggle.ToString() + 
			"\nCollision toggle: " + collision.ToString() + 
			"\nSpeed: " + Speed.ToString() + 
			"\nFlashlight: " + flashlight.ToString() + 
			"\nFlashlight.LightEnergy: " + light.LightEnergy.ToString() + 
			"\nPlayer.Position"+Position.ToString() + 
			"\nPlayer.Velocity"+Velocity.ToString() + 
			"\nPlayer.RotationDegrees"+RotationDegrees.ToString() + 
			"\nCameraTPS.Position"+CameraTPS.Position.ToString() + 
			"\nCameraTPS.RotationDegrees"+Camera.RotationDegrees.ToString() + 
			"\nCameraFPS.RotationDegrees"+CameraFPS.RotationDegrees.ToString() + 
			"\nCamera.Fov: "+CameraFPS.Fov.ToString();
		}
		else
		{
			GetNode<Label>("Label").Text="";
		}

		CameraFPS.Fov += (Input.IsActionJustPressed("fovup") ? 5f : 0) + (Input.IsActionJustPressed("fovdown") ? -5f : 0);
		CameraTPS.Fov = CameraFPS.Fov;

		if(Input.IsActionJustPressed("fps_tps"))
		{
			CameraFPS.Current = !CameraFPS.Current;
			CameraTPS.Current = !CameraFPS.Current;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Speed += (Input.IsActionPressed("speedup") ? 0.5f : 0) + (Input.IsActionPressed("speeddown") ? -0.5f : 0);
		var sprint = Input.IsActionPressed("sprint");
		/*
		if (sprint)
		{
			Speed *= 2;
		}*/
		
		Vector3 velocity = Velocity;

		if (gravity_toggle)
		{
			if (Input.IsActionJustPressed("ui_accept"))
			{
				velocity.Y += Speed;
			}
			else if (Input.IsActionPressed("down"))
			{
				velocity.Y -= Speed;
			}
			else if (IsOnFloor())
			{
				velocity.Y = 0;
			}
			else
			{
				velocity.Y -= (float)(gravity * delta);
			}
		}
		else
		{
			if (Input.IsActionPressed("ui_accept"))
			{
				velocity.Y = Speed;
			}
			else if (Input.IsActionPressed("down"))
			{
				velocity.Y = -Speed;
			}
			else
			{
				velocity.Y = 0;
			}
		}

		if (sprint)
		{
			Speed *= 2;
		}
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		direction = direction.Rotated(Vector3.Up,GetNode<SpringArm3D>("SpringArm").Rotation.Y).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}
		Velocity = velocity;
		MoveAndSlide();
		if (sprint)
		{
			Speed /= 2;
		}
	}
}
