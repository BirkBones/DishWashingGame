using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using System.Collections;
using Unity.Mathematics;
using System.Collections.Generic;

//using System.Numerics;
public class DishWashingManager : MonoBehaviour
{   
    //General
    private BrushMovement brush;
    // Visuals
    [SerializeField] private Slider progressBar;

    [SerializeField] Transform activeDishPlacement;
    [SerializeField] Transform activeDishRackPlacement;
   [SerializeField] Transform dirtyDishPlacement;
    [SerializeField] private GameObject[] DishTypes;
    [SerializeField] private GameObject dishRack;
    [SerializeField] private List<DishRack> usedDishRacks;

    [SerializeField] private Transform kitchenBench;

    [SerializeField] float BaseDishCleaningAmount;
    List<Dish> dishes;
    DishRack currentDishRack;
    Dish currentDish;
    public int DishSizePopulator = 1; // how many dishes should be added to the dish list each time the function is called.
    int currentDishIndex = 0;
    public float MovingSpeedSoundBorder {get{return brush.MovingSpeedSoundBorder; } private set { brush.MovingSpeedSoundBorder = value;}}
    
    void Awake (){
        brush = GetComponent<BrushMovement>();
        InitializeDishesArray(DishSizePopulator);
        currentDishRack = FindObjectsByType<DishRack>(FindObjectsSortMode.None)[0];
        usedDishRacks = new List<DishRack>();

        MoveToNextDishRack();
        EventsManager.Instance.OnFullDishRack += MoveToNextDishRack;
        MoveToNextDish(0);

    }
    void OnEnable(){
        EventsManager.Instance.OnBrushStartedMoving += CleanCurrentDish;
    }

    void OnDisable(){
        EventsManager.Instance.OnBrushStartedMoving -= CleanCurrentDish;
    }

    void ChangeDishRack(){
        currentDishRack.DishRackInstance.transform.position += Vector3.left * 3;
        // currentDishRack = Instantiate(DishRack());
    }

    private Transform GetNextDirtyDishposition (){
        return dirtyDishPlacement;
    }
    
    
    public void InitializeDishesArray(int amountofDishesInitialized) {
        int dishTypeIndex;
        float cleaningAmount;
        currentDishIndex = 0;
        dishes = new List<Dish>(amountofDishesInitialized); //start of with big value for more efficiency
        for (int i = 0; i < amountofDishesInitialized; i++){
            dishTypeIndex = UnityEngine.Random.Range(0,DishTypes.Length); // the last integer of range is not counted in.
            cleaningAmount = UnityEngine.Random.Range(BaseDishCleaningAmount, 2*BaseDishCleaningAmount);
            GameObject dishObject = Instantiate(DishTypes[dishTypeIndex], GetNextDirtyDishposition().position, DishTypes[dishTypeIndex].transform.rotation);
            Debug.Log(i);
            dishes.Add(dishObject.GetComponent<Dish>());
        }
        
    }
    public void CleanCurrentDish (){
        StartCoroutine(CleanCurrentDishCoroutine());

        
    }

    
    public IEnumerator CleanCurrentDishCoroutine(){
            while (BrushMovement.isBrushMovingOverSpeedTreshold && !dishes[currentDishIndex].IsClean()){
                EventsManager.Instance.InvokeOnDishBecomeCleaner(dishes[currentDishIndex].CleaningProgress); //invoke event that will update the progress bar.
                yield return null;       
            }
        }
    public void MoveToNextDish(double CleaningAmount){
        if (currentDishIndex == 0 && !(dishes[0].CleaningProgress >= dishes[0].CleaningStrictness)){
            dishes[0].ChangeTransform(activeDishPlacement);
            dishes[0].OnCleanedDish += MoveToNextDish;
            dishes[0].HandleDishBecomeActive();
        }
        else {
            dishes[currentDishIndex].OnCleanedDish -= MoveToNextDish;
            currentDishRack.PlaceDish(dishes[currentDishIndex]);
            dishes[currentDishIndex].HandleDishBecomeInactive();
            currentDishIndex++;
            if (currentDishIndex < dishes.Count){
                dishes[currentDishIndex].ChangeTransform(activeDishPlacement);
                dishes[currentDishIndex].HandleDishBecomeActive();
                dishes[currentDishIndex].OnCleanedDish += MoveToNextDish;
            }
            else {
                InitializeDishesArray(DishSizePopulator);
                dishes[0].ChangeTransform(activeDishPlacement);
                dishes[0].OnCleanedDish += MoveToNextDish;
                dishes[0].HandleDishBecomeActive();
            }
        }
    }

    public void MoveToNextDishRack(Dish dish = null){ //Takes in a dish that did not get space on the previous rack
    Debug.Log("changing dishracks");
        if (dish != null){
            usedDishRacks.Add(currentDishRack);
            GameObject temprack = Instantiate(dishRack); // instantiate and allocate new dish rack ¨
            temprack.transform.position = currentDishRack.transform.position;
            currentDishRack = temprack.GetComponent<DishRack>();
            currentDishRack.PlaceDish(dish);
            foreach (DishRack rack in usedDishRacks){ // move all the previous dishracks one to the left. 
                rack.DishRackInstance.transform.position += kitchenBench.right * (new Vector2(rack.dishRackSize.x, rack.dishRackSize.z).magnitude);
            }
        }
        else {
            currentDishRack = FindObjectsByType<DishRack>(FindObjectsSortMode.None)[0];
        }
        
        }
        





    }

    



public enum WashingState { //used for finding index for given dish state
        InactiveDirty,
        BeingWashed,
        WasJustWashed,
        InactiveClean

    }