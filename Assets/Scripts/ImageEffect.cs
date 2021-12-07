using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ImageEffect : MonoBehaviour
{
    public Material EffectMaterial;

    private void Start() {
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        Graphics.Blit(src,dest,EffectMaterial);
    }
}
