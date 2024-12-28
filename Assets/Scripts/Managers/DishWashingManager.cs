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
    [SerializeField] private BrushMovement brush;
    // Visuals
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
        InitializeDishesArray(amountOfDishes);
        MoveToFirstDish();

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
            dishes[i] = new Dish(GetNextDirtyDishposition(), dishObject, cleaningAmount);
        }
        
    }

    private void MoveToFirstDish(){
        dishes[CurrentDishIndex].ChangeTransform(activeDishPlacement);
        dishes[CurrentDishIndex].OnCleanedDish += MoveToNextDish;
    } 

    public void CleanCurrentDish (){
        StartCoroutine(CleanCurrentDishCoroutine());
    }
    public IEnumerator CleanCurrentDishCoroutine(){
            while (BrushMovement.isBrushMovingOverSpeedTreshold && !dishes[CurrentDishIndex].IsClean()){
                dishes[CurrentDishIndex].Clean(Time.deltaTime);     
                yield return null;       
            }
        }
    public void MoveToNextDish(double CleaningAmount){
        dishes[CurrentDishIndex].OnCleanedDish -= MoveToNextDish;
        dishes[CurrentDishIndex].ChangeTransform(nextCleanDishPlacement);
        CurrentDishIndex++;
        dishes[CurrentDishIndex].ChangeTransform(activeDishPlacement);
        dishes[CurrentDishIndex].OnCleanedDish += MoveToNextDish;

    }
    void Update (){
    //     for (int i = 0; i < amountOfDishes; i++){
    //         if (CurrentDishIndex > 0){
    //             Debug.Log(CurrentDishIndex);
    //         }
    // }
    }

}

class Dish
{
    public event Action<double> OnCleanedDish; //upon cleaning the dish, the event will be triggered, also providing how dirty the dish was.
    public float CleaningProgress { get; private set; } = 0f; // 0 = dirty, 1 = clean
    private float cleaningAmount;
    public GameObject DishInstance;
    public Dish(Transform _startingTransform, GameObject _dishInstance, float _cleaningAmount ){
            DishInstance = _dishInstance;
            DishInstance.transform.position = _startingTransform.position;
            cleaningAmount = _cleaningAmount;
            
    }

    public void ChangeTransform(Transform newTransform)
    {
        DishInstance.transform.position = newTransform.position;
        // DishInstance.transform.rotation = newTransform.rotation;
        // DishInstance.transform.localScale = newTransform.localScale;
    }

    public void Clean(float cleaningIncrease)
    {
        CleaningProgress += cleaningIncrease;
        CleaningProgress = Mathf.Clamp(CleaningProgress,0,cleaningAmount); // Ensure it stays between 0 and 1
        IsClean();
    }

    public bool IsClean()
    {
        if (CleaningProgress >= cleaningAmount){
            OnCleanedDish?.Invoke(cleaningAmount);
        }
        return (CleaningProgress >= cleaningAmount);
    }
}
