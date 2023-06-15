using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridObject
{
	public GridTileView view;
	public bool hasBomb;
	public bool hasFlag;
	public bool isRevealed;
}

public class LevelController : MonoBehaviour
{
	[SerializeField] private bool enableGridDebug = true;
	[SerializeField] private bool createDebugText;
	[SerializeField] private GridTileView gridTilePrefab;
	[SerializeField] private DifficultyLevel difficultyLevel;
	[SerializeField] private List<Difficulty> difficultyLevels;

	private Grid<GridObject> grid;
	private Vector3 gridOrigin;

	private void Start()
	{
		var difficultySetting = difficultyLevels.Find(x => x.DifficultyLevel == difficultyLevel);
		var gridSize = new Vector2Int(difficultySetting.GridSize, difficultySetting.GridSize);
		var bombCount = difficultySetting.BombCount;
		
		// Bombs

		var bombLocations = new Vector2Int[bombCount];
		for (int i = 0; i < bombCount; i++)
		{
			var newLocation = new Vector2Int();

			do
			{
				var x = Random.Range(0, gridSize.x);
				var y = Random.Range(0, gridSize.y);
				newLocation.x = x;
				newLocation.y = y;

			} while (bombLocations.Contains(newLocation));

			Debug.Log($"Insert bomb in {newLocation}");
			bombLocations[i] = newLocation;
		}

		// Cell size

		var padding = 2f;
		var screenHeight = Screen.height;
		var screenTopWorldPos = Camera.main.ScreenToWorldPoint(Vector3.up * screenHeight);
		var worldHeight = screenTopWorldPos.y * 2;
		var cellSize = (worldHeight - padding * 2) / gridSize.y;

		// Grid

		gridOrigin = new Vector3(-gridSize.x, -gridSize.y, 0) * cellSize / 2f;
		grid = new Grid<GridObject>(
			gridSize, 
			cellSize, 
			gridOrigin, 
			false, 
			createDebugText, 
			(Vector2Int gridPos, Vector3 gridAlignedWorldPos, int _) =>
			{
				var gridObject = new GridObject();

				gridObject.view = Instantiate(gridTilePrefab, transform);
				gridObject.view.transform.position = gridAlignedWorldPos;
				gridObject.view.transform.localScale = Vector3.one * cellSize;
				gridObject.view.Init();

				if (bombLocations.Contains(gridPos))
					gridObject.hasBomb = true;

				return gridObject;
			});

		if (createDebugText) 
			grid.EnableDebug(enableGridDebug);
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			var clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			grid.GetValue(clickPos, out var gridObject);
			grid.GetGridPosition(clickPos, out var gridPosition, true);

			gridObject.view.Reveal();
			
			if (gridObject.hasBomb)
				gridObject.view.Explode();
			else
			{
				var numberOfNeighbouringBombs = GetNumberOfNeighbouringBombs(gridPosition);
				if (numberOfNeighbouringBombs > 0)
					gridObject.view.ShowNumber(numberOfNeighbouringBombs);
			}
		}

		if (Input.GetMouseButtonDown(1))
		{
			var clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			grid.GetValue(clickPos, out var gridObject);
			
			if (gridObject.isRevealed)
				return;

			gridObject.hasFlag = !gridObject.hasFlag;
			gridObject.view.SetFlag(gridObject.hasFlag);
		}
	}

	private void OnDrawGizmos()
	{
		grid?.OnDrawGizmos_DrawDebugData();
	}

	private int GetNumberOfNeighbouringBombs(Vector2Int gridPos)
	{
		var bombs = 0;

		for (int x = gridPos.x - 1; x <= gridPos.x + 1; x++)
		{
			for (int y = gridPos.y - 1; y <= gridPos.y + 1; y++)
			{
				if (grid.GetValue(new Vector2Int(x, y), out var gridObject))
				{
					if (gridObject.hasBomb)
						bombs++;
				}
			}
		}

		return bombs;
	}
}
