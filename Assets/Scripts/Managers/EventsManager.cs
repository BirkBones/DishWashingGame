using UnityEngine;
using System;
using UnityEngine.UI;

//nyttig for decoupling :)
public class EventsManager : MonoBehaviour
{
    public static EventsManager Instance;

    public event Action OnBrushStartedMoving;
    public event Action OnPlateFinished;
    public event Action<float> OnDishBecomesCleaner;
    public event Action<RaycastHit, Texture2D> UpdateTexture;


    void Awake() {
        if (Instance == null){
            Instance = this;
        }

    }


    public void InvokeOnBrushStartedMoving(){
        OnBrushStartedMoving?.Invoke();
    }

    public void InvokeOnDishBecomeCleaner(float val){
        OnDishBecomesCleaner?.Invoke(val);
    }

    public void InvokeOnRaycasthit(RaycastHit hit, Texture2D texture){
        UpdateTexture?.Invoke(hit, texture);
    }

}

