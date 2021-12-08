using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Elemento UI para activar o desactivar un filtro.
/// </summary>
[RequireComponent(typeof(Toggle))]
public class FilterCheckbox : MonoBehaviour
{
    [SerializeField,Tooltip("UI del objeto.")] 
    private Toggle toggle;
    
    [SerializeField,Tooltip("Kernel a aplicar.")] 
    private Kernel kernel;
    public Kernel Kernel=> kernel;

    /// <summary>
    /// Evento lanzado cuando se clickea el checkbox.
    ///<para>Envía el kernel relacionado a la UI y si fue activado o desactivado.</para>
    /// </summary>
    public UnityAction<Kernel,bool> onFilterChange;

    void Start()
    {
        toggle.GetComponentInChildren<Text>().text = kernel.Name;
    }

    /// <summary>
    /// Lanza onFilterChange.
    /// </summary>
    public void HandleToggle(bool isOn)
    {
        onFilterChange.Invoke(kernel,toggle.isOn);
    }

    /// <summary>
    /// Permite setar el checkbox como (des)activado sin lanzar el evento onFilterChange.
    /// </summary>
    public void SetIsOn(bool isOn)
    {
        toggle.SetIsOnWithoutNotify(isOn);
    }
}
