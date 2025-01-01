using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public PlayerInput PlayerInput { get; private set; }
    public Transform PlayerTransform;
    void Start()
    {
     PlayerInput = GetComponent<PlayerInput>();   
    }

    
}


public enum GameState {
    Washing = 0,
    

}