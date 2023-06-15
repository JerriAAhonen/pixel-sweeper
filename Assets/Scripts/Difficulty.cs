using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DifficultyLevel { EasyPeasy, IGotThis, WTF }

[CreateAssetMenu]
public class Difficulty : ScriptableObject
{
	[SerializeField] private DifficultyLevel difficultyLevel;
	[SerializeField] private int gridSize;
	[SerializeField] private int bombCount;

	public DifficultyLevel DifficultyLevel => difficultyLevel;
	public int GridSize => gridSize;
	public int BombCount => bombCount;
}
