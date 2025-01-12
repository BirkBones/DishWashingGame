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

    public event Action<bool> OnBrushMovementToggled; // if the new movement is that the brush has speed, input = true. if the new movement is that the brush 
    // no longer has speed, input = false

    DishRendering dishrenderer;
    void Start(){
        dishMask = LayerMask.GetMask("ActiveDirtyDishes");

    }
    void Awake (){
        dishrenderer = GetComponent<DishRendering>();
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
