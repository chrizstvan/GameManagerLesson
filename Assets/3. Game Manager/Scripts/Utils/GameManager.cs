using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

	/*
	 * TODO pause menu :
	 * 	Add method to enter / exit pause
	 *  Trigger method via escape key
	 * 	Trigger method via pause menu
	 * 	Pause simulation when in pause state
	 * 	Modify cursor to use pointer when in pause state
	 */

public class GameManager : Singleton<GameManager>
{
	//what level the game is currently in
	//load and unload game levels
	//keep track of the game state
	//generate other persistant system
		
	public enum GameState
	{
		PREGAME, RUNNING, PAUSED
	}
	
	private string _currentLevelName = String.Empty;
	private List<AsyncOperation> _loadOperation;

	public GameObject[] systemPrefabs;
	private List<GameObject> _instantiateSystemPrefabs;

	private GameState _currentGameState = GameState.PREGAME;
	public Events.EventGameStates onGameStatesChange;
	
	public GameState CurrentGameState
	{
		get { return _currentGameState; }
		private set { _currentGameState = value; }
	}

	private void Start()
	{
		DontDestroyOnLoad(gameObject);
		
		_instantiateSystemPrefabs = new List<GameObject>();
		_loadOperation = new List<AsyncOperation>();
		
		InstatntiateSystemPrefab();

        UIManager.Instance.onMainMenuFadeComplete.AddListener(HandleMainMenuFadeComplete);
	}
	
	private void Update()
	{
		if (_currentGameState == GameState.PREGAME)
		{
			return;
		}
		
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			TogglePause();
		}
	}

	void OnLoadOperationComplete(AsyncOperation ao)
	{
		if (_loadOperation.Contains(ao))
		{
			_loadOperation.Remove(ao);

			if (_loadOperation.Count == 0)
			{
				UpdateStates(GameState.RUNNING);
			}
			
		}
		Debug.Log("Load Complete");
	}

	void OnUnloadOperationComplete(AsyncOperation ao)
	{
		Debug.Log("Unload Complete");
	}

    void HandleMainMenuFadeComplete(bool fadeOut)
    {
        if (!fadeOut)
        {
            UnloadLevel(_currentLevelName);
        }
    }

	void UpdateStates(GameState state)
	{
		GameState _previousGameState = _currentGameState;
		_currentGameState = state;

		switch (state)
		{
				case GameState.PREGAME:
					Time.timeScale = 1.0f;
					break;
				
				case GameState.RUNNING:
					Time.timeScale = 1.0f;
					break;
				
				case GameState.PAUSED:
					Time.timeScale = 0.0f;
					break;
				
				default:
					break;
		}
		//dispatch messeges
		onGameStatesChange.Invoke(_currentGameState, _previousGameState);
		//ui transition between screen
	}

	void InstatntiateSystemPrefab()
	{
		GameObject prefabInstance;
		for (int i = 0; i < systemPrefabs.Length; i++)
		{
			prefabInstance = Instantiate(systemPrefabs[i]);
			_instantiateSystemPrefabs.Add(prefabInstance);
		}
	}
	
	public void LoadLevel(string levelName)
	{
		AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
		if (ao == null)
		{
			return;
			Debug.LogError("[GameManager] Unable to load level " + levelName);
		}
		ao.completed += OnLoadOperationComplete;
		_loadOperation.Add(ao);
		_currentLevelName = levelName;
	}

	public void UnloadLevel(string levelName)
	{
		AsyncOperation ao = SceneManager.UnloadSceneAsync(levelName);
		if (ao == null)
		{
			return;
			Debug.LogError("[GameManager] Unable to unload level " + levelName);
		}
		ao.completed += OnUnloadOperationComplete;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		for (int i = 0; i < _instantiateSystemPrefabs.Count; i++)
		{
			Destroy(_instantiateSystemPrefabs[i]);
		}
		_instantiateSystemPrefabs.Clear();
	}

	public void StartGame()
	{
		LoadLevel("Main");
	}

	public void TogglePause()
	{
		UpdateStates(_currentGameState == GameState.RUNNING ? GameState.PAUSED : GameState.RUNNING);
	}

	public void RestartGame()
	{
		UpdateStates(GameState.PREGAME);
	}

	public void QuitGame()
	{
		//in the future this can be auto save or anything
		
		Application.Quit();
	}
}
