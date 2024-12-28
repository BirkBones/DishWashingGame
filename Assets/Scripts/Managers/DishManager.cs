using System;
using UnityEngine;

class DishManager : MonoBehaviour { //DishManager manages the dishes. Dishwashingmanager manages EVERYTHING relevant for dishwashing. (including dishmanager)
    int totalDishes;
    Transform activeDishPlacement;
    Transform nextCleanDishPlacement;
    Transform dirtyDishPlacement;
    float BaseDishCleaningAmount;
    [SerializeField] private GameObject[] DishTypes;
    Dish[] dishes;
    int CurrentDishIndex = 0;
    
    public DishManager(int _totalDishes) {
        int dishTypeIndex;
        float cleaningAmount;
        totalDishes = _totalDishes;
        dishes = new Dish[_totalDishes];
        for (int i = 0; i < totalDishes; i++){
            dishTypeIndex = UnityEngine.Random.Range(0,DishTypes.Length-1);
            cleaningAmount = UnityEngine.Random.Range(BaseDishCleaningAmount, 2*BaseDishCleaningAmount);
            dishes[i] = new Dish(GetNextDirtyposition(), DishTypes[dishTypeIndex], cleaningAmount);
            Instantiate(dishes[i].DishInstance, dishes[i].DishInstance.transform.position, dishes[i].DishInstance.transform.rotation);

        }
        

    }
    private Transform GetNextDirtyposition (){
        return dirtyDishPlacement;
    }
    public void MoveToNextDish(){
        dishes[CurrentDishIndex].ChangeTransform(nextCleanDishPlacement);
        CurrentDishIndex++;
        dishes[CurrentDishIndex].ChangeTransform(activeDishPlacement);
        // Instantiate(dishes[CurrentDishIndex].DishInstance, dishes[CurrentDishIndex].DishInstance.transform.position, dishes[CurrentDishIndex].DishInstance.transform.rotation);
    }

    public void UpdateLogic(){
        if (dishes[CurrentDishIndex].IsClean()){
            MoveToNextDish();
        }   


    }

    void FixedUpdate () {

    }


}

class Dish
{
    public event Action<double> OnCleanedDish; //upon cleaning the dish, the event will be triggered, also providing how dirty the dish was.
    public float CleaningProgress { get; private set; } = 0f; // 0 = dirty, 1 = clean
    private float cleaningAmount;
    public GameObject DishInstance;
    private BrushMovement brushScript;
    public Dish(Transform _startingTransform, GameObject _dishInstance, float _cleaningAmount ){
            DishInstance = _dishInstance;
            DishInstance.transform.position = _startingTransform.position;
            cleaningAmount = _cleaningAmount;
            
    }
    public void ChangeTransform(Transform newTransform)
    {
        DishInstance.transform.position = newTransform.position;
    }

    public void Clean(float cleaningAmount)
    {
        CleaningProgress += cleaningAmount;
        CleaningProgress = Mathf.Clamp01(CleaningProgress); // Ensure it stays between 0 and 1
    }

    public bool IsClean()
    {
        if (CleaningProgress >= cleaningAmount){
            OnCleanedDish?.Invoke(cleaningAmount);
        }
        return (CleaningProgress >= cleaningAmount);
    }
}