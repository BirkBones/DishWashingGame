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
    public Texture2D currentBrushTexture;
    [SerializeField] GameData gameData;

    PlayerInput playerInput;
    public event Action<bool> OnBrushMovementToggled; // if the new movement is that the brush has speed, input = true. if the new movement is that the brush 
    // no longer has speed, input = false

    void Awake(){
        dishMask = LayerMask.GetMask("ActiveDirtyDishes");
        playerInput = FindAnyObjectByType<PlayerInput>();
        // playerInput.actions["Hold"].performed += CursorLogic;

    }
    void OnDisable(){
        isBrushMovingOverSpeedTreshold = false;
    }

    void FixedUpdate()
    {
       MovementLogic();
       Vector2 screenPos = playerInput.actions["Hold"].ReadValue<Vector2>();
        CursorLogic(screenPos);

    }
    void CursorLogic(Vector2 screenPos){
            Vector2 CurrentCursorMovementDirection = screenPos - LastCursorPosition;
            bool IsCursorMoving = (CurrentCursorMovementDirection != Vector2.zero);
            CursorRay = Camera.main.ScreenPointToRay(screenPos);
            LastCursorPosition = screenPos;

            bool LastBrushMovingState = IsBrushMoving;
            IsBrushMoving = (IsCursorMoving && (DishesRayHit.collider != null));
            isBrushMovingOverSpeedTreshold = IsBrushMoving & (CurrentCursorMovementDirection.magnitude > MovingSpeedSoundBorder);

            if (LastBrushMovingState != IsBrushMoving && isBrushMovingOverSpeedTreshold == true){ 
                EventsManager.Instance.InvokeOnBrushStartedMoving();
            

        } 
    }
    // Handles all logic related to what functionality is implemented based on the cursorlogic.
    // noe snodige greier i 2. frame
    void MovementLogic(){
        if  (Physics.Raycast(CursorRay, out DishesRayHit,  MaxDistance, dishMask)){
                WashingBrush.position = DishesRayHit.point;
                if (IsBrushMoving){  
                    EventsManager.Instance.InvokeOnRaycasthit(DishesRayHit, currentBrushTexture);
                }
            }
        UpdateBrushOrientation();

    }

    void UpdateBrushOrientation()
    {
    }


}
