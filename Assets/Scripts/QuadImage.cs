using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class QuadImage : MonoBehaviour
{
    [SerializeField] private Renderer quadRenderer;

    public void SetTexture(Texture texture)
    {
        quadRenderer.material.SetTexture("_MainTex",texture);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if(quadRenderer == null)
        {
            quadRenderer = GetComponent<Renderer>();
        }    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
