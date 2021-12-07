using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterWindow : MonoBehaviour
{
    private FilterCheckbox[] checkboxes;
    private List<Kernel> kernels = new List<Kernel>();
    [SerializeField] private Transform checkboxesContainer;
    [SerializeField] private bool multipleChoice = true;
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
            }
            kernels.Clear();
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
