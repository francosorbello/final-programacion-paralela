using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DebugObject
{
    public Vector3 data;
}

public class ConvolutionManager : MonoBehaviour
{
    public ComputeShader computeShader;
    public Texture InputTexture;
    public RenderTexture OutputTexture;
    public ComputeBuffer debugBuffer;
    public Kernel[] kernels;

    private DebugObject[] debugData;

    private void Start() 
    {
        ApplyFilters();
    }

    public void ApplyFilters()
    {
        if (OutputTexture == null)
        {
            OutputTexture = new RenderTexture(512,512,24);
            OutputTexture.enableRandomWrite = true; 
            OutputTexture.Create();
        }
        Graphics.Blit(InputTexture,OutputTexture);
        RenderTexture nTex = OutputTexture;
        var rend = GetComponent<Renderer>();
        if(rend != null)
            rend.material.SetTexture("_MainTex",nTex);

        foreach (var kernel in kernels)
        {
            nTex = RenderComputeShader(nTex,kernel.GetKernelMatrix());
            
            // var rend = GetComponent<Renderer>();
            if(rend != null)
                rend.material.SetTexture("_MainTex",nTex);
        }
    }

    private RenderTexture RenderComputeShader(RenderTexture inputTex, Matrix4x4 matrixKernel) 
    {
        RenderTexture outputTex = new RenderTexture(512,512,24);
        outputTex.enableRandomWrite = true; 
        outputTex.Create();

        debugData = new DebugObject[512];
        int positionSize = sizeof(float) * 3;
        debugBuffer = new ComputeBuffer(debugData.Length, positionSize);
        
        computeShader.SetBuffer(0,"DebugBuffer",debugBuffer);
        computeShader.SetTexture(0,"InputTexture",inputTex);
        computeShader.SetTexture(0,"Result",outputTex);

        computeShader.SetMatrix("ConvolutionMatrix", matrixKernel);
        // computeShader.Dispatch(0, 1, 1, 1);
        computeShader.Dispatch(0, outputTex.width / 8, outputTex.height / 8, 1);    

        debugBuffer.GetData(debugData);
        foreach (var item in debugData)
        {
            Debug.Log(item.data);
        }
        return outputTex;
    }
}
