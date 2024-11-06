using Game.Manager;
using Game.Resources.Building;
using Game.UI;
using Godot;

namespace Game;

public partial class Main : Node
{

    private GridManager gridManager;
    private Sprite2D cursor;
    private Node2D ySortRoot;
    private GameUI gameUI;

    private Vector2I? hoveredGridCell;
    private BuildingResource toPlaceBuildingResource;

    public override void _Ready()
    {
        gridManager = GetNode<GridManager>("GridManager");
        cursor = GetNode<Sprite2D>("Cursor");
        ySortRoot = GetNode<Node2D>("YSortRoot");
        gameUI = GetNode<GameUI>("GameUI");

        cursor.Visible = false;

        gameUI.BuildingResourceSelected += OnBuildingResourceaSelected;
        gridManager.ResourceTilesUpdated += OnResourceTilesUpdated;
    }

    public override void _UnhandledInput(InputEvent evt)
    {
        if (hoveredGridCell.HasValue && evt.IsActionPressed("left_click") && gridManager.IsTilePositionBuildable(hoveredGridCell.Value))
        {
            PalceBuildingAtHoveredCellPosition();
            cursor.Visible = false;
        }
    }


    public override void _Process(double delta)
    {
        var gridPosition = gridManager.GetMouseGridCellPosition();
        cursor.GlobalPosition = gridPosition * 64;
        if (toPlaceBuildingResource != null && cursor.Visible && (!hoveredGridCell.HasValue || hoveredGridCell.Value != gridPosition))
        {
            hoveredGridCell = gridPosition;
            gridManager.ClearHighlightedTiles();
            gridManager.HighlightExpandedBuildableTiles(hoveredGridCell.Value, toPlaceBuildingResource.BuildableRadius);
            gridManager.HighlightResourceTiles(hoveredGridCell.Value, toPlaceBuildingResource.ResourceRadius);
        }
    }



    private void PalceBuildingAtHoveredCellPosition()
    {
        if (!hoveredGridCell.HasValue) return;

        var building = toPlaceBuildingResource.BuildingScene.Instantiate<Node2D>();
        ySortRoot.AddChild(building);

        building.GlobalPosition = hoveredGridCell.Value * 64;

        hoveredGridCell = null;
        gridManager.ClearHighlightedTiles();
    }

    private void OnBuildingResourceaSelected(BuildingResource buildingResource)
    {
        toPlaceBuildingResource = buildingResource;
        cursor.Visible = true;
        gridManager.HighlightBuildableTiles();
    }


    private void OnResourceTilesUpdated(int resourceCount)
    {
        GD.Print(resourceCount);
    }
}
