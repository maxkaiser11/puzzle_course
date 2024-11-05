using Godot;
using System;

namespace Game;

public partial class Main : Node2D
{

	private Sprite2D cursor;
	private PackedScene buildingScene;
	private Button placeBuildingButton;

	public override void _Ready()
	{
		buildingScene = GD.Load<PackedScene>("res://scenes/building/Building.tscn");
		cursor = GetNode<Sprite2D>("Cursor");
		placeBuildingButton = GetNode<Button>("PlaceBuildingButton");

		cursor.Visible = false;

		placeBuildingButton.Pressed += OnButtonPressed;
	}

    public override void _UnhandledInput(InputEvent evt)
    {
        if (cursor.Visible && evt.IsActionPressed("left_click"))
		{
			PalceBuildingAtMousePosition();
			cursor.Visible = false;
		}
    }


    public override void _Process(double delta)
	{
		var gridPosition = GetMouseGridCellPosition();
		cursor.GlobalPosition = gridPosition * 64;
	}

	
	private Vector2 GetMouseGridCellPosition()
	{
		var mousePosition = GetGlobalMousePosition();
		var gridPosition = mousePosition / 64;
		gridPosition = gridPosition.Floor();
		return gridPosition;
	}

	private void PalceBuildingAtMousePosition()
	{
		var building = buildingScene.Instantiate<Node2D>();
		AddChild(building);

		var gridPosition = GetMouseGridCellPosition();
		building.GlobalPosition = gridPosition * 64;
	}

	private void OnButtonPressed()
	{
		cursor.Visible = true;
	}
}
