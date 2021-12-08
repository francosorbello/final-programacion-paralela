using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Struct para obtener valores de debug de los compute shaders
/// </summary>
public struct DebugObject
{
    public Vector3 data;
}

/// <summary>
/// Aplica kerneles de convolución en compute shaders
/// </summary>
public class ComputeConvolutionManager : ConvolutionManager
{
    [Tooltip("Shader que aplicará los kernels.")] 
    public ComputeShader computeShader;
    
    [Tooltip("Textura que contiene la imagen con los filtros aplicados.")] 
    public RenderTexture OutputTexture;
    
    [Tooltip("Buffer donde se guardan valores para debugear.")] 
    public ComputeBuffer debugBuffer;
    
    [Range(1,4)] public int Strength = 1;
    
    [Tooltip("Lista de kernels a aplicar sobre la imagen")]
    public Kernel[] Kernels;

    //Almacena los valores del debugBuffer.
    private DebugObject[] debugData;

    private void Start() 
    {
        ApplyFilters(Kernels);
    }

    /// <summary>
    /// Aplica una serie de kernels a una imagen.
    /// <param name="kernels"> Lista de kernels a aplicar </param>
    /// </summary>
    public override void ApplyFilters(Kernel[] kernels)
    {
        //genero una nueva textura de salida
        if (OutputTexture == null)
        {
            OutputTexture = new RenderTexture(512,512,24);
            OutputTexture.enableRandomWrite = true; 
            OutputTexture.Create();
        }
        Graphics.Blit(InputTexture,OutputTexture);
        RenderTexture nTex = OutputTexture;
        
        var rend = GetComponent<RawImage>();
        if(rend != null)
            rend.texture = nTex;
        
        foreach (var kernel in kernels)
        {
            nTex = RenderComputeShader(nTex,kernel);
            
            // var rend = GetComponent<Renderer>();
            if(rend != null)
                // rend.material.SetTexture("_MainTex",nTex);
                rend.texture = nTex;
        }
    }

    /// <summary>
    /// Para una imagen y un kernel, retorna la textura con el kernel aplicado.
    /// <param name="inputTex"> Textura sobre la que aplicar el filtro. </param>
    /// <param name="kernel"> Kernel a aplicar. </param>
    /// </summary>
    private RenderTexture RenderComputeShader(RenderTexture inputTex, Kernel kernel) 
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
        computeShader.SetInt("Strength",kernel.Strenght);

        computeShader.SetMatrix("ConvolutionMatrix", kernel.GetKernelMatrix());        
        computeShader.Dispatch(0, outputTex.width / 8, outputTex.height / 8, 1);    

        // debugBuffer.GetData(debugData);
        // foreach (var item in debugData)
        // {
        //     Debug.Log(item.data);
        // }

        debugBuffer.Dispose();
        debugBuffer = null;
        return outputTex;
    }
}
