using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rota la camara sobre el eje Y alrededor de un punto.
/// </summary>
public class CameraRotator : MonoBehaviour
{
    [Min(1)] public float speed = 1;
    [Tooltip("Controla si la rotación está activada o no.")] public bool rotationActive = true;

    // Update is called once per frame
    void Update()
    {
        if(rotationActive)
            transform.Rotate(0, speed * Time.deltaTime, 0 );
    }
}
