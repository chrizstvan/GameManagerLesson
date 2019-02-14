using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
	[SerializeField] private MainMenu _mainMenu;
	[SerializeField] private PauseMenu _pauseMenu;

	[SerializeField] private Camera _dummyCamera;
	public Events.EventFadeComplete onMainMenuFadeComplete;

	private void Start()
	{
		_mainMenu.onMainMenuFadeComplete.AddListener(HandleMainMenuFadeComplete);
		GameManager.Instance.onGameStatesChange.AddListener(HandleGameStateChange);
	}
	
	private void Update()
	{
		if (GameManager.Instance.CurrentGameState != GameManager.GameState.PREGAME)
		{
			return;
		}
		
		if (Input.GetKeyDown(KeyCode.Space))
		{
			GameManager.Instance.StartGame();
		}
	}

	void HandleMainMenuFadeComplete(bool fadeOut)
	{
		onMainMenuFadeComplete.Invoke(fadeOut);
	}
	
	void HandleGameStateChange(GameManager.GameState currentState, GameManager.GameState previousState)
	{
		_pauseMenu.gameObject.SetActive(currentState == GameManager.GameState.PAUSED);
	}

	public void SetDummyCameraActive(bool active)
	{
		_dummyCamera.gameObject.SetActive(active);
	}
}
