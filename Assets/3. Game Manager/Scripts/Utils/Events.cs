using UnityEngine.Events;

public class Events
{
    [System.Serializable]
    public class EventGameStates : UnityEvent<GameManager.GameState, GameManager.GameState> { }
    
    [System.Serializable]
    public class EventFadeComplete : UnityEvent<bool> { }
    
}
