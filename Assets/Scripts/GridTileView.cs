using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridTileView : MonoBehaviour
{
	[SerializeField] private SpriteRenderer background;
	[SerializeField] private SpriteRenderer foreground;
	[SerializeField] private SpriteDatabase sb;

	public void Init()
	{
		background.sprite = sb.HiddenTile;
		foreground.enabled = false;
	}

	public void Reveal()
	{
		background.sprite = sb.RevealedTile;
	}

	public void ShowNumber(int number)
	{
		foreground.enabled = true;
		foreground.sprite = sb.GetNumberSprite(number);
	}

	public void SetFlag(bool show)
	{
		foreground.enabled = show;
		foreground.sprite = sb.Flag;
	}

	public void ShowBomb(bool exploded)
	{
		foreground.enabled = true;
		foreground.sprite = exploded ? sb.ExplodedBomb : sb.Bomb;
	}
}
