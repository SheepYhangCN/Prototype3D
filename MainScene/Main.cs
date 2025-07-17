using Godot;
using System;

public partial class Main : Node3D
{
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		//if(Input.IsActionJustPressed("pause")){GetTree().Quit();}
		if (Input.IsActionJustPressed("pause"))
		{
			var focus = GetNode<SpringArm3D>("Player/SpringArm").Get("focus").AsBool();
			Input.MouseMode = (focus ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured);
			GetNode<SpringArm3D>("Player/SpringArm").Set("focus", !focus);
		}
	}
}
