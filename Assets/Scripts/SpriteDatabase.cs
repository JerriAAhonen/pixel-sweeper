using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SpriteDatabase : ScriptableObject
{
	[SerializeField] private Sprite hiddenTile;
	[SerializeField] private Sprite revealedTile;
	[SerializeField] private Sprite bomb;
	[SerializeField] private Sprite explodedBomb;
	[SerializeField] private Sprite flag;
	[SerializeField] private List<Sprite> numberSprites;

	public Sprite HiddenTile => hiddenTile;
	public Sprite RevealedTile => revealedTile;
	public Sprite Bomb => bomb;
	public Sprite ExplodedBomb => explodedBomb;
	public Sprite Flag => flag;

	public Sprite GetNumberSprite(int num) => numberSprites[num - 1];
}
