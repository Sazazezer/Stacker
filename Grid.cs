using Godot;
using System;

public partial class Grid : Node2D
{
	private Tile[,] grid;
	private int rows = 5;
	private int columns = 5;
	private int tileWidth = 100;
	private int tileHeight = 100;
	private int tilePaddingWidth = 15;
	private int tilePaddingHeight = 15;
	private int startingColumns = 4;
	private int startingRow = 4;
	private int currentColumn;
	private int currentRow;
	private PackedScene sceneTile;
	private Tile currentTile;
	private bool gameStart = false;
	private bool isRight;
	private Node speedTimer;
	private bool isMoving = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		currentColumn = startingColumns;
		currentRow = startingRow;
		grid = new Tile[rows, columns];
		speedTimer = GetNode<Timer>("SpeedTimer");
		sceneTile = ResourceLoader.Load<PackedScene>("res://Tile.tscn");
		currentTile = InstantiateTile(currentColumn, currentRow);
		GD.Print(currentTile);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("Action"))
		{
			Action();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GD.Print(isMoving);
	}

	private Vector2 GridReferenceToGridPosition(Vector2 GridReference)
	{
		return new Vector2((GridReference.X * (tileWidth + tilePaddingWidth)) + tilePaddingWidth, (GridReference.Y * (tileHeight + tilePaddingHeight)) + tilePaddingHeight);
	}

	private Tile InstantiateTile(int x, int y)
	{
		Vector2 startPosition = GridReferenceToGridPosition(new Vector2(x, y));
		Tile newTile = sceneTile.Instantiate() as Tile;
		grid[x, y] = newTile;
		newTile.Position = startPosition;
		AddChild(newTile);
		return newTile;
	}

	private void Action()
	{
		if (!gameStart)
		{
			gameStart = true;
			MoveCurrentTile(currentTile);
		}
		else
		{
			((Timer)speedTimer).WaitTime = ((Timer)speedTimer).WaitTime / 2;
			isMoving = false;
			StopBlock();
			CheckLose();
		}
	}

	private async void MoveCurrentTile(Tile moveTile)
	{
		while (isMoving)
		{
			await ToSignal(speedTimer, "timeout");
			if (isMoving)
			{
				if (isRight)
				{
					currentRow += 1;
					if (currentRow >= 4)
					{
						isRight = false;
					}
				}
				else
				{
					currentRow -= 1;
					if (currentRow <= 0)
					{
						isRight = true;
					}
				}
				grid[currentRow, currentColumn] = moveTile;
				moveTile.Position = GridReferenceToGridPosition(new Vector2(currentRow, currentColumn));
				GD.Print("ROW: " + currentRow + "-COLUMN: " + currentColumn);
			}
		}
	}



	private void StopBlock()
	{

	}

	private void CheckLose()
	{

	}
}
