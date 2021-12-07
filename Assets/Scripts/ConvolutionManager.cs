using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConvolutionManager : MonoBehaviour
{
    public Texture InputTexture;

    public abstract void ApplyFilters(Kernel[] kernels);

}
