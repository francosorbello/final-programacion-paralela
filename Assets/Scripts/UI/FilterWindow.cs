using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterWindow : MonoBehaviour
{
    private FilterCheckbox[] checkboxes;
    private List<Kernel> kernels = new List<Kernel>();
    [SerializeField] private Transform checkboxesContainer;
    
    [SerializeField] private ConvolutionManager convolutionManager;
    
    // Start is called before the first frame update
    void Start()
    {
        checkboxes = checkboxesContainer.GetComponentsInChildren<FilterCheckbox>();
        foreach (var box in checkboxes)
        {
            box.onFilterChange += HandleFilterChange;
        }    
    }

    public void HandleFilterChange(Kernel kernel, bool isOn)
    {
        Debug.Log(isOn);
        Debug.Log(kernel.Name);
        if(isOn)
        {
            kernels.Add(kernel);
        } else 
        {
            kernels.Remove(kernel);
        }
    }

    public void ApplyChanges()
    {
        convolutionManager.ApplyFilters(kernels.ToArray());
    }

    public void ClearChanges()
    {
        kernels.Clear();
        foreach (var item in checkboxes)
        {
            item.SetIsOn(false);
        }
        ApplyChanges();
    }
}
