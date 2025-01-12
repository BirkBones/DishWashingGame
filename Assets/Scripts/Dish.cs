using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEditor;
class Dish : MonoBehaviour
{
    public event Action<double> OnCleanedDish; //upon cleaning the dish, the event will be triggered, also providing how dirty the dish was.
    public GameObject DishInstance; // the 3d model this script is attached to
     public Action<RaycastHit> UpdateTexture; //action that triggers when you should update the texture
    [SerializeField] Material [] materials;
    [SerializeField] Texture2D dirtMaskTextureBase;
   public  Texture2D dirtMaskTexture;
    public Renderer rend;
    public float StartDirtyness; // the amount of pixels that were dirty from the beginning
    public float CurrentCleanedness = 0; // the amount of pixels that are cleaned
    public float CleaningProgress => CurrentCleanedness / StartDirtyness; // Expression-bodied property
    public float CleaningStrictness = 0.99f; // Offset in % of what will be considered clean. Aka. for 0.99 99% and over will be accepted and the program will continue to run.
    public float CleaningPixelTreshold = 0.2f;
    void Awake(){
            DishInstance = gameObject;
            dirtMaskTexture = new Texture2D(dirtMaskTextureBase.width, dirtMaskTextureBase.height); //The next four lines copies dirtmasktexturebase 
            dirtMaskTexture.SetPixels(dirtMaskTextureBase.GetPixels()); // and sets the dirtydishmaterial to the new copy of dirtmasktexture, so that we can change the copy and see visual changes in the scene.
            dirtMaskTexture.Apply();
            rend = GetComponent<Renderer>();
            StartDirtyness = FindDirtyness(dirtMaskTexture); //Initializes the Startdirtyness to be the amount of unclean pixels.

    }
    void Update(){
//         rend.material = materials[1];
//                 rend.material.SetTexture("DirtMask",dirtMaskTexture);
// rend.material.SetTexture("DirtMask", dirtMaskTexture);
        EditorUtility.SetDirty(rend.material);
        dirtMaskTexture.Apply();
        Texture2D testTexture = new Texture2D(128, 128);
rend.material.SetTexture("DirtMask", testTexture);
    }
    public void ChangeTransform(Transform newTransform)
    {
        DishInstance.transform.position = newTransform.position;
        // DishInstance.transform.rotation = newTransform.rotation;
        // DishInstance.transform.localScale = newTransform.localScale;
    }

    public bool IsClean()
    {
        if (CleaningProgress >= CleaningStrictness){
            OnCleanedDish?.Invoke(StartDirtyness);
            Debug.Log("was it cleaned");
        }
        return (CleaningProgress >= CleaningStrictness);
    }

    float FindDirtyness(Texture2D texture){
        float dirt = 0f;
        for (int x = 0; x < texture.width; x++){ // remember (0,0) in uv coordinate is left down corner of the brushes uv map.
            for (int y = 0; y < texture.height; y++){
                dirt += (texture.GetPixel(x,y).g!=0)?1:0;
            }
        }
        return dirt;

    }

    private void CleanPixelAtRaycastHit(RaycastHit hit, Texture2D dirtBrush) //Handles both cleaning the pixel and updating the currrentcleanedness.
    {
        UnityEngine.Vector2 textureCoord = hit.textureCoord;
        int pixelX = (int)(textureCoord.x * dirtMaskTexture.width); //hold the pixel coordinates of the mesh that was hit
        int pixelY = (int)(textureCoord.y * dirtMaskTexture.height);

        int pixelXOffset = pixelX - (dirtBrush.width / 2); // dirtbrush.width / 2 is because we start in the lower corner of the brush texture. by subtracting with this we use the correct brushpixel for each dishpixel.
        int pixelYOffset = pixelY - (dirtBrush.height / 2);

        bool wasJustDirty;
        
        for (int x = 0; x < dirtBrush.width; x++){ // remember (0,0) in uv coordinate is left down corner of the brushes uv map.
            
            for (int y = 0; y < dirtBrush.height; y++){
                
                Color pixelFromBrushTexture = dirtBrush.GetPixel(x,y);
                Color pixelFromDirtMaskTexture = dirtMaskTexture.GetPixel(x+pixelXOffset,y+pixelYOffset);
                wasJustDirty = (pixelFromDirtMaskTexture.g != 0); 
                dirtMaskTexture.SetPixel(pixelXOffset + x, pixelYOffset + y, 
                new Color (0, Mathf.Clamp01(pixelFromDirtMaskTexture.g - pixelFromBrushTexture.g*0.05f), 0));
                 pixelFromDirtMaskTexture = dirtMaskTexture.GetPixel(x+pixelXOffset,y+pixelYOffset);
                CurrentCleanedness += ((pixelFromDirtMaskTexture.g == 0) && wasJustDirty) ? 1: 0;
                
            }
        }

    }

    public void HandleDishBecomeActive(){
        DishInstance.layer = LayerMask.NameToLayer("ActiveDirtyDishes");
        EventsManager.Instance.UpdateTexture += CleanPixelAtRaycastHit;
        rend.material = materials[(int)WashingState.BeingWashed];
        rend.material.SetTexture("_DirtMask",dirtMaskTexture);

    }

    public void HandleDishBecomeInactive(){
        DishInstance.layer = LayerMask.NameToLayer("Default");
        EventsManager.Instance.UpdateTexture -= CleanPixelAtRaycastHit;
        rend.material = materials[(int)WashingState.InactiveClean];

    }
}
