using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
	[Header("Pages")]
	[SerializeField] private CanvasGroup menu;
	[SerializeField] private CanvasGroup difficultyChoice;
	[SerializeField] private CanvasGroup core;
	[Header("Buttons")]
	[SerializeField] private Button play;
	[SerializeField] private Button difficulty1;
	[SerializeField] private Button difficulty2;
	[SerializeField] private Button difficulty3;
	//TODO: Custom
	[SerializeField] private Button exit;
	[Space]
	[SerializeField] private Button restartLevel;
	[SerializeField] private Button toMenu;
	[Header("References")]
	[SerializeField] private LevelController levelController;

	private int currentDifficultyLevel;

	private void Start()
	{
		// Menu
		play.onClick.AddListener(OnPlay);
		difficulty1.onClick.AddListener(() => OnDifficultySelected(0));
		difficulty2.onClick.AddListener(() => OnDifficultySelected(1));
		difficulty3.onClick.AddListener(() => OnDifficultySelected(2));
		exit.onClick.AddListener(OnExit);

		// Core
		restartLevel.onClick.AddListener(OnRestartLevel);
		toMenu.onClick.AddListener(OnGoToMenu);

		ShowMenu();
	}

	private void ShowMenu()
	{
		menu.SetVisible(true);
		difficultyChoice.SetVisible(false);
		core.SetVisible(false);
	}

	private void HideMenu()
	{
		menu.SetVisible(false);
		difficultyChoice.SetVisible(false);
		core.SetVisible(true);
	}

	private void OnPlay()
	{
		menu.SetVisible(false);
		difficultyChoice.SetVisible(true);
	}

	private void OnDifficultySelected(int difficulty)
	{
		currentDifficultyLevel = difficulty;
		levelController.CreateGrid((DifficultyLevel) difficulty);

		HideMenu();
	}

	private void OnRestartLevel()
	{
		levelController.DestroyGrid();
		levelController.CreateGrid((DifficultyLevel)currentDifficultyLevel);
	}

	private void OnGoToMenu()
	{
		levelController.DestroyGrid();
		ShowMenu();
	}

	private void OnExit()
	{
		Application.Quit();
	}
}
