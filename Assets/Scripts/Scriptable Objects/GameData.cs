using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Objects/GameData")]
public class GameData : ScriptableObject
{
    public bool IsBrushMoving = false;
    public int Time;

    public Vector3 PlayerPosition;

}
