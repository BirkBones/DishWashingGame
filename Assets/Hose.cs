using UnityEngine;

public class Hose : MonoBehaviour
{
    [SerializeField] GameObject MovableHose;
    [SerializeField] GameObject ColdHandle;
    [SerializeField] GameObject HotHandle;

    public float WaterTemperature {get; private set;}
    

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake(){
    }
    void Subscribe(){

    }

    


}
