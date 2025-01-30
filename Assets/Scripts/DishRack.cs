using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Collections;
using System;
using Unity.VisualScripting;
using Unity.Mathematics;
using NUnit.Framework.Constraints;
using System.Collections.Generic;

public class DishRack : MonoBehaviour
{
    public GameObject DishRackInstance;
    public bool OnDisplayAvailableNodes, LiveGridAdapt;
    Node[,] grid;

    public Vector2 gridWorldSize; //holds the size of the grid in unity units (aka what you could look at as meters)
    public float nodeBase; // nodeBase and nodeScaler makes it more easy to adapt to a specific nodeRadius since the numbers are so small.
    public float nodeScaler;
    public float nodeRadius { get { return nodeBase * nodeScaler;} // the radius for the nodes, aka half the sidelength of the square
    }

    float nodeDiameter {
        get {return 2 * nodeRadius;}
    }
    int gridSizeX {
        get { 
            return Mathf.RoundToInt(gridWorldSize.x / nodeDiameter); 
    }
    }
    int gridSizeY { //holds the size of the y and x direction in the array.
        get { 
            return Mathf.RoundToInt(gridWorldSize.y / nodeDiameter); }
        }

    [SerializeField] Transform bottomLeft;

    [SerializeField] public static float DishRackCenterYOffset = 0.1f; // the offset between the center of the dishrack and the actual bottom of the dishrack.
    
    List<Dish> dishes; // stores all the dishes currently in the dishrack.
    public Vector3 dishRackSize {get { return dishRackRenderer.bounds.size;}}
    [SerializeField] private Renderer dishRackRenderer;
    void Awake(){
        Debug.Log("dishrack instanttiated");
        DishRackInstance = gameObject;
        // gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        // gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
        Debug.Log("gridcreate done");
        dishes = new List<Dish>();
        dishRackRenderer = GetComponent<Renderer>();
        GameObject temp = GameObject.Find("main_frame");
        dishRackRenderer = temp.GetComponent<Renderer>();
        
    }



    Bounds test;
    
    void Update(){

    }   

    void CreateGrid() {
        grid = new Node[gridSizeX,gridSizeY];
        
        for (int x = 0; x < gridSizeX; x++){
            for (int y = 0; y < gridSizeY; y++){
                Vector3 worldSpace = bottomLeft.position + bottomLeft.transform.right * (x * nodeDiameter)
                + bottomLeft.transform.up * (y*nodeDiameter);
                grid[x,y] = new Node(true, worldSpace, x, y);
            }
    }

    }

    
    void OnDrawGizmos(){
        Matrix4x4 originalMatrix = Gizmos.matrix;

        // Vector3 gridCenter = bottomLeft.position + gridWorldSize.y * bottomLeft.up / 2 + gridWorldSize.x * bottomLeft.right / 2;
        // Gizmos.DrawWireCube(gridCenter, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        // Gizmos.matrix = Matrix4x4.TRS(Vector3.zero, DishRackInstance.transform.rotation, Vector3.one); //makes the gizmos orient nicely.
        Gizmos.DrawCube(bottomLeft.position, Vector3.one*0.04f);
        Vector3 gridCenter = bottomLeft.position + gridWorldSize.y * bottomLeft.up / 2 + gridWorldSize.x * bottomLeft.right / 2;
        
        Gizmos.matrix = Matrix4x4.TRS(gridCenter, DishRackInstance.transform.rotation, Vector3.one); //makes the gizmos orient nicely.
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        Gizmos.matrix = Matrix4x4.TRS(test.center, DishRackInstance.transform.rotation, Vector3.one); //makes the gizmos orient nicely.
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(test.size.x, test.size.y, test.size.z));

        if (OnDisplayAvailableNodes){
            if (LiveGridAdapt || !Application.isPlaying){
                Gizmos.color = Color.green;

                for (int x = 0; x < gridSizeX; x++){
                    for (int y = 0; y < gridSizeY; y++){
                        Vector3 worldSpace = bottomLeft.position + bottomLeft.transform.right * (x * nodeDiameter)
                        + bottomLeft.transform.up * (y*nodeDiameter);
                        Gizmos.matrix = Matrix4x4.TRS(worldSpace,DishRackInstance.transform.rotation,Vector3.one);
                        Gizmos.DrawCube(Vector3.zero, new Vector3(nodeDiameter, nodeDiameter, nodeDiameter)*0.8f);
                    }
                }
            }
            else {
            Color color;
            Debug.Log(this.grid != null);
            foreach (Node n in grid){
                if (n.available) color = new Color(0, 255,0);
                else color = new Color (255,0,0);
                Gizmos.color = color;
                Gizmos.matrix = Matrix4x4.TRS(n.worldPosition, DishRackInstance.transform.rotation, Vector3.one); // To get proper rotation
                Gizmos.DrawCube(Vector3.zero, new Vector3(nodeDiameter, nodeDiameter, nodeDiameter)*0.8f);
            }
            }

            Gizmos.color = Color.grey;
        }

        Gizmos.matrix = originalMatrix; //Make sure the other gizmos later are not oriented depending on the dishrack.

       
          
    }

    public Node NodeFromWorldPoint(Vector3 worldpos){
        //WOOPS : here the y coordinate represents the ground, not the height.
        float percX = Mathf.Clamp01(((worldpos.x - transform.position.x) + gridWorldSize.x / 2) / (gridWorldSize.x)); // worldpos.x - transform.x is the local position, that will say this will remain constant even if the arena along with all objects are moved by a distance.
        float percY = Mathf.Clamp01(((worldpos.z - transform.position.z) + gridWorldSize.y / 2) / (gridWorldSize.y));
        //Note that the + gridworldsize.x / 2 makes it so worldpos = (0,0) gets the middle index. 

        int indX = Mathf.RoundToInt(percX * (gridSizeX-1));
        int indY = Mathf.RoundToInt(percY * (gridSizeY-1));
        return grid[indX,indY];
    } 


    public void PlaceDish(Dish dish){
        dishes.Add(dish);
        Collider col = dish.DishInstance.GetComponent<Collider>();
        Renderer rend = dish.DishInstance.GetComponent<Renderer>();
        dish.DishInstance.transform.rotation = DishRackInstance.transform.rotation;
        dish.DishInstance.transform.rotation *= dish.rotationOnCleaned;
        for (int i = 0; i < 4; i++){
            Vector2Int dimensionsInGrid = Get2DNodeSpace(dish.GetBounds(), quaternion.identity, dish.DishInstance.transform);
            Vector2Int availableCornerInGrid = FindAvailableArea(dimensionsInGrid, out _);
            if (availableCornerInGrid != -Vector2Int.one){
                Debug.Log("should be placed" + availableCornerInGrid);
                dish.transform.position = CalculateDishCenter(dish, availableCornerInGrid, dimensionsInGrid);
                UpdateAvailability(availableCornerInGrid.x, availableCornerInGrid.y, dimensionsInGrid, false);
                dish.DishInstance.transform.SetParent(this.transform);
                return;
                }
            else {
                dish.DishInstance.transform.Rotate(new Vector3(0,-90,0), Space.World); //want to check if we can put the plate to the leftwards direction instead
                }
            }
        Debug.Log("couldnt find dishplace");
        EventsManager.Instance.InvokeOnDishRackBecomeFull(dish);


            
        }
    private void UpdateAvailability(int startX, int startY, Vector2Int dimensions, bool available){

         for (int dx = 0; dx < dimensions.x; dx++)
            {
                for (int dy = 0; dy < dimensions.y; dy++)
                {   
                if (startX + dx < gridSizeX && startY + dimensions.y < gridSizeY){
                    grid[startX + dx, startY + dy].available = available;
                }
                }
            }
    }

    public float TooBigBoundsFactor = 0.1f;
    public Vector2Int Get2DNodeSpace(Bounds bounds, Quaternion rotation, Transform dish){ //Takes in the size of the dish instance, and the rotation that is going to be applied to it when it is to bet set in the dishrack. Returns an array that shows how many nodes of space the dish will take up.
        // Bounds newbounds = RotateBounds(bounds, rotation);
        Bounds newbounds = bounds;
        Vector3 size = newbounds.size;
        // Debug.Log(size);
        var siz1 = size - dish.forward*TooBigBoundsFactor;
        var siz2 = size + dish.forward * TooBigBoundsFactor;
        size = (siz1.magnitude <= siz2.magnitude)? siz1 : siz2; //making sure that the compensation for the slightly bigger render bounds, always makes the box smaller, not larger.
        int xSize = Mathf.CeilToInt(size.x / nodeDiameter);
        int YSize = Mathf.CeilToInt(size.z / nodeDiameter); //oops; in Unity its the xz plane that makes the horisontal plane, not the xy.
        test = bounds;
        return new Vector2Int(xSize, YSize);        
    }
    public static Bounds RotateBounds(Bounds bounds, Quaternion rotation){
        // Get the original center and size of the bounds
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;

        // List of all 8 corners of the bounds
        Vector3[] corners = new Vector3[8];
        corners[0] = center + new Vector3(-extents.x, -extents.y, -extents.z);
        corners[1] = center + new Vector3(-extents.x, -extents.y, extents.z);
        corners[2] = center + new Vector3(-extents.x, extents.y, -extents.z);
        corners[3] = center + new Vector3(-extents.x, extents.y, extents.z);
        corners[4] = center + new Vector3(extents.x, -extents.y, -extents.z);
        corners[5] = center + new Vector3(extents.x, -extents.y, extents.z);
        corners[6] = center + new Vector3(extents.x, extents.y, -extents.z);
        corners[7] = center + new Vector3(extents.x, extents.y, extents.z);

        // Rotate all corners
        for (int i = 0; i < corners.Length; i++)
        {
            corners[i] = rotation * (corners[i] - center) + center;
        }

        // Calculate the new bounds
        Vector3 min = corners[0];
        Vector3 max = corners[0];
        foreach (var corner in corners)
        {
            min = Vector3.Min(min, corner);
            max = Vector3.Max(max, corner);
        }
        var ans = new Bounds((min + max) / 2, max - min);
        return ans;
    }

    public Vector2Int FindAvailableArea(Vector2Int dimensionsInGrid, out quaternion outputrotation){
        Vector2Int ans = new Vector2Int(-1,-1);
        outputrotation = quaternion.identity;
                for (int x = 0; x < grid.GetLength(0); x++){
                    for (int y = 0; y < grid.GetLength(1); y++){
                        if (IsAreaAvailable(x,y,dimensionsInGrid)){
                            ans = new Vector2Int(x,y);
                            return ans;
                        }
                    }
                }
        return ans;
        }

    public float dishYMargin = 0.02f;
    private Vector3 CalculateDishCenter(Dish dish, Vector2Int availableCornerInGrid, Vector2Int dimensions){
        Vector3 ans = new Vector3();
        // if (availableCornerInGrid.x + dimensions.x >= gridSizeX || availableCornerInGrid.y + dimensions.y>= gridSizeY) return Vector3.zero;
        ans.x = (grid[availableCornerInGrid.x, availableCornerInGrid.y].worldPosition.x 
        + grid[availableCornerInGrid.x + dimensions.x - 1 , availableCornerInGrid.y + dimensions.y - 1].worldPosition.x)/2;
        ans.z = (grid[availableCornerInGrid.x, availableCornerInGrid.y].worldPosition.z 
        + grid[availableCornerInGrid.x + dimensions.x - 1 , availableCornerInGrid.y + dimensions.y - 1].worldPosition.z - TooBigBoundsFactor / 2)/2;       
        ans.y = DishRackInstance.transform.position.y + dish.GetBounds().size.y / 2 + dishYMargin;
        return ans;
    }
    private bool IsAreaAvailable(int startX, int startY, Vector2Int dimensions)
        {
            for (int dx = 0; dx < dimensions.x; dx++)
            {
                for (int dy = 0; dy < dimensions.y; dy++)
                {   
                    if (startX + dx < gridSizeX && startY + dy < gridSizeY){
                        if (!grid[startX + dx, startY + dy].available)
                        {
                            return false; // Area is blocked
                        }
                    }
                    else {
                        return false;
                    }
                }
            }
            return true; // Area is available
        }

        
}

public class Node {
    public bool available;
    public Vector3 worldPosition;
    public int xVal; //xval and yval are indices. Oops; x and y does not correspond to x and y axes in unity, but the plane made by the x and z axes.
    public int yVal;
  
    public Node(bool _available, Vector3 _worldpos, int _xval, int _yval){
        available = _available;
        worldPosition = _worldpos;
        xVal = _xval;
        yVal = _yval;

    }


    
}
