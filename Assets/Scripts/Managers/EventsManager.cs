using UnityEngine;
using System;

//nyttig for decoupling :)
public class EventsManager : MonoBehaviour
{
    public static EventsManager Instance;

    public event Action OnBrushStartedMoving;
    public event Action OnPlateFinished;

    public event Action<bool> PlayBrushingSound;

    void Awake() {
        if (Instance == null){
            Instance = this;
        }

    }


    public void InvokeOnBrushStartedMoving(){
        OnBrushStartedMoving?.Invoke();
    }

}

