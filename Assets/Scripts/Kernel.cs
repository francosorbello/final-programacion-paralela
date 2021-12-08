using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable Object que representa un kernel de convolución.
/// </summary>
[CreateAssetMenu(fileName = "Kernel", menuName = "Kernel Convolution/New Kernel", order = 0)]
public class Kernel : ScriptableObject 
{
    public string Name; //nombre del kernel
    [Min(1)] public int Strenght = 1; //fuerza con la que se aplica el kernel a la imagen.
    [Space]
    [Header("Convolution Matrix")]
    [SerializeField]private Vector3 row1;
    [SerializeField]private Vector3 row2;
    [SerializeField]private Vector3 row3;

    /// <summary>
    /// Transforma los valores del scriptable object en una matriz
    /// </summary>
    public Matrix4x4 GetKernelMatrix()
    {
        return new Matrix4x4(
            new Vector4(row1.x,row1.y,row1.z,0),
            new Vector4(row2.x,row2.y,row2.z,0),
            new Vector4(row3.x,row3.y,row3.z,0),
            Vector4.zero
        );
    }
}