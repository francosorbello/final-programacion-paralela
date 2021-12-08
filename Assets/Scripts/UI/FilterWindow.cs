using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// https://colorhunt.co/palette/d06224ae431e8a8635e9c891

/// <summary>
/// Muestra una lista de filtros y los aplica sobre una imagen.
/// </summary>
public class FilterWindow : MonoBehaviour
{
    /// <summary> Lista de Checkboxes en la UI </summary>
    private FilterCheckbox[] checkboxes;
    
    /// <summary> Lista de kernels a aplicar </summary>
    private List<Kernel> kernels = new List<Kernel>();
    
    [SerializeField,Tooltip("Objeto que contiene los checkboxes")] 
    private Transform checkboxesContainer;
    
    [SerializeField,Tooltip("Indica si se puede seleccionar más de una opcion al mismo tiempo.")] 
    private bool multipleChoice = true;
    
    [SerializeField,Tooltip("Manager para aplicar los kernels.")] 
    private ConvolutionManager convolutionManager;
    
    void Start()
    {
        checkboxes = checkboxesContainer.GetComponentsInChildren<FilterCheckbox>();
        foreach (var box in checkboxes)
        {
            box.onFilterChange += HandleFilterChange;
        }    
    }

    /// <summary>
    /// Se llama cuando se clickea una de las checkboxes
    /// </summary>
    /// <param name="kernel"> Kernel a aplicar. </param>
    /// <param name="isOn"> Indica si el checkbox está marcado o no. </param>
    public void HandleFilterChange(Kernel kernel, bool isOn)
    {
        if(isOn)
        {
            if(!multipleChoice)
            {
                foreach (var checkbox in checkboxes)
                {
                    if(checkbox.Kernel != kernel)
                    {
                        checkbox.SetIsOn(false);
                    }
                }
                kernels.Clear();
            }
            kernels.Add(kernel);
        } else 
        {
            kernels.Remove(kernel);
        }
    }

    /// <summary>
    /// Aplica los filtros seleccionados en la UI.
    /// </summary>
    public void ApplyChanges()
    {
        convolutionManager.ApplyFilters(kernels.ToArray());
    }

    /// <summary>
    /// Deselecciona todos los checkbox y remueve los filtros de la imagen.
    /// </summary>
    public void ClearChanges()
    {
        foreach (var item in checkboxes)
        {
            item.SetIsOn(false);
        }
        kernels.Clear();
        ApplyChanges();
    }
}
