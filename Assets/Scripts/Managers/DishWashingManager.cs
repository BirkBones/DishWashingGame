using UnityEngine;
using UnityEngine.UI;


//using System.Numerics;
public class DishWashingManager : MonoBehaviour
{   
    //General
    [SerializeField] private static int amountOfDishes;
    [SerializeField] private DishManager dishManager;
    [SerializeField] private BrushMovement brush;
    DishManager dishes = new DishManager(amountOfDishes);
    // Visuals
    [SerializeField] private Slider progressBar;
    void Awake(){
        brush = GetComponent<BrushMovement>();
    }

}