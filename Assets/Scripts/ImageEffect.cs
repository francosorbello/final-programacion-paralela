using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Aplica un efecto de postprocessing a la pantalla.
/// </summary>
[ExecuteInEditMode]
public class ImageEffect : MonoBehaviour
{
    [Tooltip("Material con el efecto a aplicar")] public Material EffectMaterial;

    private void Start() {
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        Graphics.Blit(src,dest,EffectMaterial);
    }
}
