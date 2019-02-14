using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour 
{
    // track the Animation component
    // track the AnimationClip fadein fadeout
    // function that can recieve animation events
    // function to play fade in/out animation

    [SerializeField] private Animation _mainMenuAnimator;
    [SerializeField] private AnimationClip _fadeOutAnimation;
    [SerializeField] private AnimationClip _fadeInAnimation;

    public Events.EventFadeComplete onMainMenuFadeComplete;

    private void Start()
    {
        GameManager.Instance.onGameStatesChange.AddListener(HandleGameStateChange);
    }

    public void OnFadeOutComplete()
    {
        onMainMenuFadeComplete.Invoke(true);
    }

    public void OnFadeInComplete()
    {
        onMainMenuFadeComplete.Invoke(false);
        
        UIManager.Instance.SetDummyCameraActive(true);
    }

    void HandleGameStateChange(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        if (previousState == GameManager.GameState.PREGAME && currentState == GameManager.GameState.RUNNING)
        {
            FadeOut();
        }

        if (previousState != GameManager.GameState.PREGAME && currentState == GameManager.GameState.PREGAME)
        {
            FadeIn();
        }
    }

    public void FadeIn()
    {
        _mainMenuAnimator.Stop();
        _mainMenuAnimator.clip = _fadeInAnimation;
        _mainMenuAnimator.Play();
    }

    public void FadeOut()
    {
        UIManager.Instance.SetDummyCameraActive(false);
        _mainMenuAnimator.Stop();
        _mainMenuAnimator.clip = _fadeOutAnimation;
        _mainMenuAnimator.Play();
    }
}
