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
    public event Action<Dish> OnFullDishRack; //upon a full dishrack, the event will be triggered along with the plate that did not get placed.


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

    public void InvokeOnDishRackBecomeFull(Dish dish){
        OnFullDishRack?.Invoke(dish);
    }

}

