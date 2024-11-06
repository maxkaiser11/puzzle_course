using Godot;
using System.Collections.Generic;

namespace Game;

public partial class Main : Node
{

	private Sprite2D cursor;
	private PackedScene buildingScene;
	private Button placeBuildingButton;
	private TileMapLayer highlightTilemapLayer;

	private Vector2? hoveredGridCell;
	private HashSet<Vector2> occupiedCells = new();

	public override void _Ready()
	{
		buildingScene = GD.Load<PackedScene>("res://scenes/building/Building.tscn");
		cursor = GetNode<Sprite2D>("Cursor");
		placeBuildingButton = GetNode<Button>("PlaceBuildingButton");
		highlightTilemapLayer = GetNode<TileMapLayer>("HighlightTileMapLayer");

		cursor.Visible = false;

		placeBuildingButton.Pressed += OnButtonPressed;
	}

	public override void _UnhandledInput(InputEvent evt)
	{
		if (hoveredGridCell.HasValue && evt.IsActionPressed("left_click") && !occupiedCells.Contains(hoveredGridCell.Value))
		{
			PalceBuildingAtHoveredCellPosition();
			cursor.Visible = false;
		}
	}


	public override void _Process(double delta)
	{
		var gridPosition = GetMouseGridCellPosition();
		cursor.GlobalPosition = gridPosition * 64;
		if (cursor.Visible && (!hoveredGridCell.HasValue || hoveredGridCell.Value != gridPosition))
		{
			hoveredGridCell = gridPosition;
			UpdateHighlightTilemapLayer();
		}
	}


	private Vector2 GetMouseGridCellPosition()
	{
		var mousePosition = highlightTilemapLayer.GetGlobalMousePosition();
		var gridPosition = mousePosition / 64;
		gridPosition = gridPosition.Floor();
		return gridPosition;
	}

	private void PalceBuildingAtHoveredCellPosition()
	{
		if (!hoveredGridCell.HasValue) return;

		var building = buildingScene.Instantiate<Node2D>();
		AddChild(building);

		building.GlobalPosition = hoveredGridCell.Value * 64;
		occupiedCells.Add(hoveredGridCell.Value);

		hoveredGridCell = null;
		UpdateHighlightTilemapLayer();
	}

	private void UpdateHighlightTilemapLayer()
	{
		highlightTilemapLayer.Clear();
		if (!hoveredGridCell.HasValue)
		{
			return;
		}

		for (var x = hoveredGridCell.Value.X - 3; x <= hoveredGridCell.Value.X + 3; x++)
		{
			for (var y = hoveredGridCell.Value.Y - 3; y <= hoveredGridCell.Value.Y + 3; y++)
			{
				highlightTilemapLayer.SetCell(new Vector2I((int)x, (int)y), 0, Vector2I.Zero);
			}
		}
	}

	private void OnButtonPressed()
	{
		cursor.Visible = true;
	}
}
