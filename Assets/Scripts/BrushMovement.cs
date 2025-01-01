using UnityEngine;
using UnityEngine.InputSystem;
using System;
public class BrushMovement : MonoBehaviour
{
    Ray CursorRay; //Holds the ray casted by the cursor
    RaycastHit DishesRayHit; // If the ray from the cursor hits a plate, this variable
    // stores the hit.

    Vector2 LastCursorPosition = Vector2.zero;
    // [SerializeField] private Transform washingBrush;

    public Transform WashingBrush;
    float MaxDistance = 200;
    private LayerMask dishMask; //What is considered a dish?

    public static bool IsBrushMoving { get; private set; } = false;
    public static bool isBrushMovingOverSpeedTreshold { get; private set; } = false;
    public float MovingSpeedSoundBorder = 0.1f;
    [SerializeField] GameData gameData;

    public event Action<bool> OnBrushMovementToggled; // if the new movement is that the brush has speed, input = true. if the new movement is that the brush 
    // no longer has speed, input = false

    void Start(){
        dishMask = LayerMask.GetMask("ActiveDirtyDishes");

    }
    void OnDisable(){
        isBrushMovingOverSpeedTreshold = false;
    }

    void FixedUpdate()
    {
       MovementLogic();
        CursorLogic(); //Sets variables such as WashingMovement, updates Rays.

    }
    void CursorLogic(){
        if (Mouse.current!=null){
            Vector2 CurrentCursorMovementDirection = Mouse.current.position.ReadValue() - LastCursorPosition;
            bool IsCursorMoving = (CurrentCursorMovementDirection != Vector2.zero);
            CursorRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            LastCursorPosition = Mouse.current.position.ReadValue();

            bool LastBrushMovingState = IsBrushMoving;
            IsBrushMoving = (IsCursorMoving && (DishesRayHit.collider != null));
            isBrushMovingOverSpeedTreshold = IsBrushMoving & (CurrentCursorMovementDirection.magnitude > MovingSpeedSoundBorder);

            if (LastBrushMovingState != IsBrushMoving && isBrushMovingOverSpeedTreshold == true){ 
                EventsManager.Instance.InvokeOnBrushStartedMoving();
                
            }

        } 
    }
    // Handles all logic related to what functionality is implemented based on the cursorlogic.
    // noe snodige greier i 2. frame
    void MovementLogic(){
        if  (Physics.Raycast(CursorRay, out DishesRayHit,  MaxDistance, dishMask)){
                WashingBrush.position = DishesRayHit.point;
                // Debug.Log("posisjonen ble " + WashingBrush.position);   
                Debug.Log(DishesRayHit.transform);        
            }
        UpdateBrushOrientation();

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
