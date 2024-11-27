using Godot;
using System;
using System.Collections.Generic;

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
	private int startingRow = 2;
	private int currentColumn;
	private int currentRow;
	private PackedScene sceneTile;
	private Tile currentTile;
	private bool gameStart = false;
	private bool isRight;
	private Node speedTimer;
	private bool isMoving = true;
	private List<int> columnCount = new List<int>();
	private bool gameOver = false;
	private Node tileContainer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		tileContainer = GetNode<Node2D>("TileContainer");
		PopulateGame();
	}

	private void PopulateGame()
	{
		currentColumn = startingColumns;
		currentRow = startingRow;
		grid = new Tile[rows, columns];
		speedTimer = GetNode<Timer>("SpeedTimer");
		sceneTile = ResourceLoader.Load<PackedScene>("res://Tile.tscn");
		currentTile = InstantiateTile(currentRow, currentColumn);
		MoveCurrentTile(currentTile);
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
	}

	private Vector2 GridReferenceToGridPosition(Vector2 GridReference)
	{
		return new Vector2((GridReference.X * (tileWidth + tilePaddingWidth)) + tilePaddingWidth, (GridReference.Y * (tileHeight + tilePaddingHeight)) + tilePaddingHeight);
	}

	private Tile InstantiateTile(int x, int y)
	{
		Vector2 startPosition = GridReferenceToGridPosition(new Vector2(x, y));
		Tile newTile = sceneTile.Instantiate() as Tile;
		newTile.isCurrentTile = true;
		newTile.UpdateColor(y);
		grid[x, y] = newTile;
		newTile.Position = startPosition;
		tileContainer.AddChild(newTile);
		return newTile;
	}

	private void Action()
	{
		if (gameOver)
		{
			Restart();
		}
		if (!gameStart)
		{
			gameStart = true;
		}
		else
		{
			StopBlock();
		}
	}

	private async void MoveCurrentTile(Tile moveTile)
	{
		GD.Print("move!");
		while (isMoving && moveTile.isCurrentTile && !gameOver)
		{
			await ToSignal(speedTimer, "timeout");
			if (isMoving && moveTile.isCurrentTile)
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
			}
		}
	}



	private void StopBlock()
	{
		columnCount.Add(currentRow);
		CheckWinLose();
		((Timer)speedTimer).WaitTime = ((Timer)speedTimer).WaitTime / 2;
		isMoving = false;
		currentTile.isCurrentTile = false;
		currentTile = null;
		currentColumn--;
		currentRow = 2;
		currentTile = InstantiateTile(currentRow, currentColumn);
		isMoving = true;
		MoveCurrentTile(currentTile);

	}

	private void CheckWinLose()
	{
		int? firstValue = null;
		foreach (var item in columnCount)
		{
			if (!firstValue.HasValue)
			{
				firstValue = item;
			}
			if (firstValue == item)
			{
				if (columnCount.Count == 5)
				{
					GD.Print("You win");
					gameOver = true;
					break;
				}
				else
				{
					GD.Print("Continue!");
				}

			}
			else
			{
				GD.Print("Lose");
				gameOver = true;
				break;
			}
		}
	}

	private void Restart()
	{
		foreach (Node item in tileContainer.GetChildren())
		{
			item.QueueFree();
		}
		for (int x = 0; x < 4; x++)
		{
			for (int y = 0; y < 4; y++)
			{
				if (grid[x, y] != null)
				{
					grid[x, y].QueueFree();
					grid[x, y] = null;
				}
			}
		}
		grid = new Tile[rows, columns];
		columnCount.Clear();
		gameOver = false;
		gameStart = false;
		((Timer)speedTimer).WaitTime = 1;
		PopulateGame();
	}
}
