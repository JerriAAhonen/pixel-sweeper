using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
	[SerializeField] private CanvasGroup menu;
	[SerializeField] private CanvasGroup difficultyChoice;

	[SerializeField] private Button play;
	[SerializeField] private Button difficulty1;
	[SerializeField] private Button difficulty2;
	[SerializeField] private Button difficulty3;
	//TODO: Custom
	[SerializeField] private Button exit;
	[Space]
	[SerializeField] private LevelController levelController;

	private void Start()
	{
		play.onClick.AddListener(OnPlay);
		difficulty1.onClick.AddListener(() => OnDifficultySelected(0));
		difficulty2.onClick.AddListener(() => OnDifficultySelected(1));
		difficulty3.onClick.AddListener(() => OnDifficultySelected(2));
		exit.onClick.AddListener(OnExit);
		
		ShowMenu();
	}

	private void ShowMenu()
	{
		menu.SetVisible(true);
		difficultyChoice.SetVisible(false);
	}

	private void HideMenu()
	{
		menu.SetVisible(false);
		difficultyChoice.SetVisible(false);
	}

	private void OnPlay()
	{
		menu.SetVisible(false);
		difficultyChoice.SetVisible(true);
	}

	private void OnDifficultySelected(int difficulty)
	{
		levelController.CreateGrid((DifficultyLevel) difficulty);

		HideMenu();
	}

	private void OnExit()
	{
		Application.Quit();
	}
}
