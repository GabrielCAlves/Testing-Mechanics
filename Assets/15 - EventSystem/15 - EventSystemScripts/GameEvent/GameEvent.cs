using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "GameEvent", menuName = "Events/GameEvent")]
public class GameEvent : ScriptableObject
{
    /*public*/ private List<GameEventListener> listeners = new List<GameEventListener>();

    public void Raise()
    {
        foreach (var listener in listeners)
        {
            listener.OnEventRaised();
        }
    }

    public void RegisterListener(GameEventListener listener)
    {
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    public void UnregisterListener(GameEventListener listener)
    {
        if (listeners.Contains(listener))
        {
            listeners.Remove(listener);
        }
    }
}

//[System.Serializable]
//public class GameEventListener/* : MonoBehaviour*/
//{
//    public GameEvent gameEvent;
//    public UnityEvent response;

//    private void OnEnable()
//    {
//        gameEvent.RegisterListener(this);
//    }

//    private void OnDisable()
//    {
//        gameEvent.UnregisterListener(this);
//    }

//    public void OnEventRaised()
//    {
//        response.Invoke();
//    }
//}