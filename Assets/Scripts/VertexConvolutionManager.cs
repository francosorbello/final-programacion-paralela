using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VertexConvolutionManager : ConvolutionManager
{
    public RawImage outputImage;
    public Kernel Identitykernel;
    
    private void Start() {
        outputImage.material.SetTexture("_MainTex",InputTexture);
        outputImage.material.SetMatrix("_ConvolutionMatrix",Identitykernel.GetKernelMatrix());        
    }

    public override void ApplyFilters(Kernel[] kernels)
    {
        outputImage.material.SetTexture("_MainTex",InputTexture);
        outputImage.material.SetMatrix("_ConvolutionMatrix",Identitykernel.GetKernelMatrix());
        foreach (var kernel in kernels)
        {
            outputImage.material.SetInt("_Strength",kernel.Strenght);
            outputImage.material.SetMatrix("_ConvolutionMatrix",kernel.GetKernelMatrix());

        }
    }
}
