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
    public RenderTexture renderTexture;
    public ComputeBuffer debugBuffer;

    private DebugObject[] debugData;

    private void Start() 
    {
        RenderComputeShader();
    }

    private void RenderComputeShader() 
    {
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(512,512,24);
            renderTexture.enableRandomWrite = true; 
            renderTexture.Create();
        }

        debugData = new DebugObject[512];
        int positionSize = sizeof(float) * 3;
        debugBuffer = new ComputeBuffer(debugData.Length, positionSize);
        
        computeShader.SetBuffer(0,"DebugBuffer",debugBuffer);
        computeShader.SetTexture(0,"Result",renderTexture);
        computeShader.SetTexture(0,"InputTexture",InputTexture);
        var test = new Matrix4x4(
            new Vector4(-1,-1,-1,0),
            new Vector4(-1,8,-1,0),
            new Vector4(-1,-1,-1,0),
            Vector4.zero
        );
        computeShader.SetMatrix("ConvolutionMatrix", test);
        // computeShader.Dispatch(0, 1, 1, 1);
        computeShader.Dispatch(0, renderTexture.width / 8, renderTexture.height / 8, 1);    

        debugBuffer.GetData(debugData);
        Debug.Log(debugData.Length);
        foreach (var item in debugData)
        {
            Debug.Log(item.data);
        }
        var rend = GetComponent<Renderer>();
        if(rend != null)
            rend.material.SetTexture("_MainTex",renderTexture);
    }
    

    // private void OnRenderImage(RenderTexture src, RenderTexture dest)
    // {
    //     if (renderTexture == null)
    //     {
    //         renderTexture = new RenderTexture(512,512,24);
    //         renderTexture.enableRandomWrite = true;
    //         renderTexture.Create();
    //     }

    //     debugData = new DebugObject[512];
    //     int positionSize = sizeof(float) * 3;
    //     debugBuffer = new ComputeBuffer(debugData.Length, positionSize);
        
    //     computeShader.SetBuffer(0,"DebugBuffer",debugBuffer);
    //     computeShader.SetTexture(0,"Result",renderTexture);
    //     computeShader.SetTexture(0,"InputTexture",InputTexture);
    //     // computeShader.Dispatch(0, 1, 1, 1);
    //     computeShader.Dispatch(0, renderTexture.width / 8, renderTexture.height / 8, 1);    

    //     debugBuffer.GetData(debugData);
    //     Debug.Log(debugData.Length);
    //     foreach (var item in debugData)
    //     {
    //         Debug.Log(item.data);
    //     }    
    // }
}
