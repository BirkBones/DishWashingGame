using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Mathematics;
using UnityEngine.UI;
//using System.Numerics;
public class DishWashingManager : MonoBehaviour
{   
    //General
    [SerializeField] private Transform WashingBrush;

    [SerializeField] private Slider progressBar;

    float MaxDistance = 200;
    private LayerMask dishMask; //What is considered a dish?
    // Sound
    Ray CursorRay; //Holds the ray casted by the cursor
    RaycastHit DishesRayHit; // If the ray from the cursor hits a plate, this variable
    // stores the hit.

    Vector2 LastCursorPosition = Vector2.zero;

    bool isBrushMoving;

    // All relevant to sound
     [SerializeField] private AudioClip[] WashingSounds; //All relevant Sounds.

    void Start()
    {
        dishMask = LayerMask.GetMask("ActiveDirtyDishes");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       CursorLogic(); //Sets variables such as WashingMovement, updates Rays.
       MovementLogic();
        SoundLogic();
        Debug.Log("Slider is " + progressBar.value);

    }
    //Handles all logic related to the cursor and its movement
    void CursorLogic(){
        if (Mouse.current!=null){
            Vector2 CurrentCursorMovementDirection = Mouse.current.position.ReadValue() - LastCursorPosition;
            bool IsCursorMoving = (CurrentCursorMovementDirection != Vector2.zero);
            CursorRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            LastCursorPosition = Mouse.current.position.ReadValue();
            isBrushMoving = (IsCursorMoving && (DishesRayHit.collider != null));

        } 
    }
    // Handles all logic related to what functionality is implemented based on the cursorlogic.
    void MovementLogic(){
        if  (Physics.Raycast(CursorRay, out DishesRayHit,  MaxDistance, dishMask)){
                Debug.Log("Raycast hit");
                WashingBrush.position = DishesRayHit.point;                
            }
        UpdateBrushOrientation();

    }

    void SoundLogic(){
        AudioClip BrushingSound = WashingSounds[0];
        int SoundIndex = 0;
        if ((SoundFXManager.Instance.SoundPlaying[SoundIndex] == false) & isBrushMoving){
            StartCoroutine(SoundFXManager.Instance.PlaySoundFXClip(BrushingSound, WashingBrush, 1, SoundIndex));
        }
        if (!isBrushMoving){
            SoundFXManager.Instance.StopPlaySoundFXClip(SoundIndex);
        }
        Debug.Log(isBrushMoving);
    }

    void UpdateBrushOrientation()
    {
        // Cast a ray downward from the brush's position
        // Vector3 brushPosition = WashingBrush.position;
        // Vector3 rayDirection = Vector3.down; // Assuming the brush is moving above the frying pan

        // if (Physics.Raycast(brushPosition, rayDirection, out DishesRayHit, MaxDistance, dishMask))
        // {
        //     // Get the normal of the frying pan surface
        //     Vector3 hitNormal = DishesRayHit.normal;

        //     // Align the washing brush to the frying pan's surface normal
        //     // WashingBrush.up = Vector3.up;

        //     // // Optional: Adjust rotation to point forward
        //     // // Use the hitNormal as the "up" direction, and the brush's forward direction to calculate the final rotation
        //     // WashingBrush.rotation = Quaternion.LookRotation(WashingBrush.forward, hitNormal); 
        //     // //Lookrotation takes in a forward vector and an up vector, and creates the rotation that is needed for 

        //     WashingBrush.rotation = Quaternion.FromToRotation(WashingBrush.up, hitNormal);

        //     Debug.Log($"Aligned brush to normal: {hitNormal}");
        // }
        // else
        // {
        //     Debug.Log("No frying pan detected below the washing brush.");
        // }
    }
}


class Dishes {
    Dish[] dishes;
    int CurrentDishIndex = 0;
    
    public Dishes(){

    }

    public void MoveToNextDish(){

    }

    public void DishUpdater(){
        
    }


}

class Dish
{
    public float CleaningProgress { get; private set; } = 0f; // 0 = dirty, 1 = clean
    private float cleaningAmount;
    public GameObject DishInstance;

    public Dish(Transform _startingTransform, GameObject _dishInstance, float _startingCleanliness, float _cleaningAmount ){
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
        return (CleaningProgress >= cleaningAmount);
    }
}