using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase abstracta que aplica una serie de filtros a una textura de entrada.
/// </summary>
public abstract class ConvolutionManager : MonoBehaviour
{
    public Texture InputTexture;

    public abstract void ApplyFilters(Kernel[] kernels);

}
