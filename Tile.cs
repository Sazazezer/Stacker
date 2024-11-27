using Godot;
using System;

public partial class Tile : Polygon2D
{
	private Color tileColor;
	public bool isCurrentTile;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void UpdateColor(int value)
	{
		switch (value)
		{
			case (0):
				tileColor = new Color("eee3da");
				break;
			case (1):
				tileColor = new Color("eddfc8");
				break;
			case (2):
				tileColor = new Color("f2b178");
				break;
			case (3):
				tileColor = new Color("f59562");
				break;
			case (4):
				tileColor = new Color("f57c5f");
				break;
		}
		Color = tileColor;
	}

	public void Delete()
	{
		this.QueueFree();
	}
}
