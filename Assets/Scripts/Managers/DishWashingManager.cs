using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using System.Collections;

//using System.Numerics;
public class DishWashingManager : MonoBehaviour
{   
    //General
    [SerializeField] private int amountOfDishes;
    private BrushMovement brush;
    // Visuals
    DishRendering dishrenderer;
    [SerializeField] private Slider progressBar;

    [SerializeField] Transform activeDishPlacement;
    [SerializeField] Transform nextCleanDishPlacement;
   [SerializeField] Transform dirtyDishPlacement;
    [SerializeField] private GameObject[] DishTypes;
    [SerializeField] float BaseDishCleaningAmount;
    Dish[] dishes;

    int CurrentDishIndex = 0;
    public float MovingSpeedSoundBorder {get{return brush.MovingSpeedSoundBorder; } private set { brush.MovingSpeedSoundBorder = value;}}
    
    void Start (){
        brush = GetComponent<BrushMovement>();
        dishrenderer = GetComponent<DishRendering>();
        InitializeDishesArray(amountOfDishes);
        MoveToNextDish(0);

    }
    void OnEnable(){
        EventsManager.Instance.OnBrushStartedMoving += CleanCurrentDish;
    }

    void OnDisable(){
        EventsManager.Instance.OnBrushStartedMoving -= CleanCurrentDish;
    }

    private Transform GetNextDirtyDishposition (){
        return dirtyDishPlacement;
    }
    
    private Transform GetNextCleanDishPosition(){
        return nextCleanDishPlacement;
    }
    public void InitializeDishesArray(int _totalDishes) {
        int dishTypeIndex;
        float cleaningAmount;
        dishes = new Dish[_totalDishes];
        for (int i = 0; i < amountOfDishes; i++){
            dishTypeIndex = UnityEngine.Random.Range(0,DishTypes.Length); // the last integer of range is not counted in.
            cleaningAmount = UnityEngine.Random.Range(BaseDishCleaningAmount, 2*BaseDishCleaningAmount);
            GameObject dishObject = Instantiate(DishTypes[dishTypeIndex], GetNextDirtyDishposition().position, DishTypes[dishTypeIndex].transform.rotation);
            dishes[i] = dishObject.GetComponent<Dish>();
        }
        
    }
    public void CleanCurrentDish (){
        StartCoroutine(CleanCurrentDishCoroutine());

        
    }

    
    public IEnumerator CleanCurrentDishCoroutine(){
            while (BrushMovement.isBrushMovingOverSpeedTreshold && !dishes[CurrentDishIndex].IsClean()){
                EventsManager.Instance.InvokeOnDishBecomeCleaner(dishes[CurrentDishIndex].CleaningProgress); //invoke event that will update the progress bar.
                yield return null;       
            }
        }
    public void MoveToNextDish(double CleaningAmount){
        if (CurrentDishIndex == 0 && dishes[CurrentDishIndex].transform.position != activeDishPlacement.position){
            dishes[CurrentDishIndex].ChangeTransform(activeDishPlacement);
            dishes[CurrentDishIndex].OnCleanedDish += MoveToNextDish;
            dishes[CurrentDishIndex].HandleDishBecomeActive();
            return;
        }
        dishes[CurrentDishIndex].OnCleanedDish -= MoveToNextDish;
        dishes[CurrentDishIndex].ChangeTransform(nextCleanDishPlacement);
        dishes[CurrentDishIndex].HandleDishBecomeInactive();
        CurrentDishIndex++;
        dishes[CurrentDishIndex].ChangeTransform(activeDishPlacement);
        dishes[CurrentDishIndex].HandleDishBecomeActive();
        dishes[CurrentDishIndex].OnCleanedDish += MoveToNextDish;
       
    }
    void Update(){
        // Debug.Log("cleaningprogress, startidryt and currentcleaned is equal to " + dishes[CurrentDishIndex].CleaningProgress);
        // Debug.Log(dishes[CurrentDishIndex].StartDirtyness);
        //         Debug.Log(dishes[CurrentDishIndex].CurrentCleanedness);
        Debug.Log(dishes[CurrentDishIndex].CleaningProgress);
        Debug.Log($"Material name: {dishes[CurrentDishIndex].rend.material.name}");

    }

}

public enum WashingState { //used for finding index for given dish state
        InactiveDirty,
        BeingWashed,
        WasJustWashed,
        InactiveClean

    }