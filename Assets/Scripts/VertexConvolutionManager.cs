using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexConvolutionManager : MonoBehaviour
{
    public Renderer meshRenderer;
    public Texture inputTexture;
    public Kernel kernel;
    // Start is called before the first frame update
    void Start()
    {
        if (meshRenderer == null) return;
        
        meshRenderer.material.SetTexture("_MainTex",inputTexture);
        meshRenderer.material.SetMatrix("_ConvolutionMatrix",kernel.GetKernelMatrix());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
