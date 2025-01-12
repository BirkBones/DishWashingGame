using UnityEngine;
using System;
using System.Numerics;

public class DishRendering : MonoBehaviour
{
    public Action<RaycastHit> UpdateTexture;
    [SerializeField] Texture2D dirtMaskTextureBase; //Holds the original texture that will be displayed on the dirty dish
    Texture2D dirtMaskTexture; // Holds an original of dirtMasktextureBase that we can change without problems 
    [SerializeField] Texture2D dirtBrush; //holds the texture mapping the strength of the brush for each point on the brush.
    public Material DirtyDishMaterial; // holds 
    public Material CleanDishMaterial;
    public Material InActiveDirtyDishMaterial;
    float StartDirtyness; // the amount of pixels that were dirty from the beginning
    float CurrentCleanedness; // the amount of pixels that are cleaned
    float CurrentProgress; // currentcleaned / started dirty
    void Awake()
    {
        UpdateTexture = CleanPixelAtRaycastHit;
        dirtMaskTexture = new Texture2D(dirtMaskTextureBase.width, dirtMaskTextureBase.height);
        dirtMaskTexture.SetPixels(dirtMaskTextureBase.GetPixels());
        dirtMaskTexture.Apply();
        DirtyDishMaterial.SetTexture("_DirtMask",dirtMaskTexture);
        UpdateDirtyness(dirtMaskTexture, out StartDirtyness);

    }

    void Update (){
        Debug.Log(CurrentCleanedness / StartDirtyness);
    }

    void UpdateDirtyness(Texture2D texture, out float dirtyamount){
        float dirt = 0f;
        for (int x = 0; x < texture.width; x++){ // remember (0,0) in uv coordinate is left down corner of the brushes uv map.
            for (int y = 0; y < texture.height; y++){
                dirt += texture.GetPixel(x,y).g;
            }
        }
        dirtyamount = dirt;

    }


    private void CleanPixelAtRaycastHit(RaycastHit hit) //Handles both cleaning the pixel and updating the currrentcleanedness.
    {
        UnityEngine.Vector2 textureCoord = hit.textureCoord;
        int pixelX = (int)(textureCoord.x * dirtMaskTexture.width); //hold the pixel coordinates of the mesh that was hit
        int pixelY = (int)(textureCoord.y * dirtMaskTexture.height);
        Vector2Int paintPixelPos = new Vector2Int(pixelX, pixelY);

        int pixelXOffset = pixelX - (dirtBrush.width / 2); // dirtbrush.width / 2 is because we start in the lower corner of the brush texture. by subtracting with this we use the correct brushpixel for each dishpixel.
        int pixelYOffset = pixelY - (dirtBrush.height / 2);
        bool wasJustDirty = false;

        for (int x = 0; x < dirtBrush.width; x++){ // remember (0,0) in uv coordinate is left down corner of the brushes uv map.
            
            for (int y = 0; y < dirtBrush.height; y++){
                Color pixelFromBrushTexture = dirtBrush.GetPixel(x,y);
                Color pixelFromDirtMaskTexture = dirtMaskTexture.GetPixel(x+pixelXOffset,y+pixelYOffset);
                wasJustDirty = (pixelFromDirtMaskTexture.g != 0); 
                dirtMaskTexture.SetPixel(pixelXOffset + x, pixelYOffset + y, 
                new Color (0, pixelFromBrushTexture.g * pixelFromDirtMaskTexture.g, 0));
                CurrentCleanedness += ((pixelFromDirtMaskTexture.g == 0) && wasJustDirty) ? 1: 0;
            }
        }
    }
    
    
}
