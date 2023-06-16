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
	[SerializeField] private float padding;
	[SerializeField] private float titlePadding;
	[SerializeField] private GridTileView gridTilePrefab;
	[SerializeField] private List<Difficulty> difficultyLevels;

	private Grid<GridObject> grid;
	private Vector3 gridOrigin;
	private Vector2Int[] bombLocations;
	private bool gameOver;

	public void CreateGrid(DifficultyLevel difficultyLevel)
	{
		gameOver = false;
		var difficultySetting = difficultyLevels.Find(x => x.DifficultyLevel == difficultyLevel);
		var gridSize = new Vector2Int(difficultySetting.GridSize, difficultySetting.GridSize);
		var bombCount = difficultySetting.BombCount;

		// Bombs

		bombLocations = new Vector2Int[bombCount];
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

			bombLocations[i] = newLocation;
		}

		// Cell size

		var screenHeight = Screen.height;
		var screenTopWorldPos = Camera.main.ScreenToWorldPoint(Vector3.up * screenHeight);
		var worldHeight = screenTopWorldPos.y * 2;
		var cellSize = (worldHeight - padding * 2) / gridSize.y;

		// Grid

		gridOrigin = new Vector3(-gridSize.x, -gridSize.y, 0) * cellSize / 2f;
		gridOrigin.y -= titlePadding;
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

	public void DestroyGrid()
	{
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}

		grid = null;
	}

	private void Update()
	{
        if (grid == null || gameOver)
			return;

        if (Input.GetMouseButtonDown(0))
		{
			var clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if (!grid.GetValue(clickPos, out var gridObject))
				return;
			grid.GetGridPosition(clickPos, out var gridPosition, true);
			handeledReavealPositions.Clear();
			HandleReveal(gridObject, gridPosition);
		}

		if (Input.GetMouseButtonDown(1))
		{
			var clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if (!grid.GetValue(clickPos, out var gridObject))
				return;
			
			if (gridObject.isRevealed)
				return;

			gridObject.hasFlag = !gridObject.hasFlag;
			gridObject.view.SetFlag(gridObject.hasFlag);
		}
	}

	private readonly List<Vector2Int> handeledReavealPositions = new();
	private void HandleReveal(GridObject gridObject, Vector2Int gridPosition)
	{
		if (handeledReavealPositions.Contains(gridPosition))
			return;

		gridObject.view.Reveal();
		gridObject.isRevealed = true;
		handeledReavealPositions.Add(gridPosition);

		if (gridObject.hasBomb)
			OnExplode(gridObject);
		else
		{
			var numberOfNeighbouringBombs = GetNumberOfNeighbouringBombs(gridPosition);
			if (numberOfNeighbouringBombs > 0)
				gridObject.view.ShowNumber(numberOfNeighbouringBombs);
			else
			{
				foreach (var pos in GetNeighboursWithoutBombs(gridPosition))
				{
					grid.GetValue(pos, out gridObject);
					HandleReveal(gridObject, pos);
				}
			}
		}
	}

	private void OnExplode(GridObject gridObject)
	{
		gridObject.view.ShowBomb(true);

		foreach (var pos in bombLocations)
		{
			grid.GetValue(pos, out var gridObj);
			if (gridObj == gridObject)
				continue;

			gridObj.view.Reveal();
			gridObj.view.ShowBomb(false);
		}

		Debug.Log("Game over!");
		gameOver = true;
	}

	private List<Vector2Int> GetNeighboursWithoutBombs(Vector2Int origin)
	{
		List<Vector2Int> neighbours = new();

		for (int x = origin.x - 1; x <= origin.x + 1; x++)
		{
			for (int y = origin.y - 1; y <= origin.y + 1; y++)
			{
				var pos = new Vector2Int(x, y);
				
				if (!grid.IsInside(pos))
					continue;

				if (pos.Equals(origin))
					continue;

				grid.GetValue(pos, out var gridObject, true);
				if (!gridObject.hasBomb)
					neighbours.Add(pos);
			}
		}

		return neighbours;
	}

	private int GetNumberOfNeighbouringBombs(Vector2Int gridPos)
	{
		var bombs = 0;

		for (int x = gridPos.x - 1; x <= gridPos.x + 1; x++)
		{
			for (int y = gridPos.y - 1; y <= gridPos.y + 1; y++)
			{
				if (grid.GetValue(new Vector2Int(x, y), out var gridObject, true))
				{
					if (gridObject.hasBomb)
						bombs++;
				}
			}
		}

		return bombs;
	}
}
