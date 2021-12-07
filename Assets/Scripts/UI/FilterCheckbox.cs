using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Toggle))]
public class FilterCheckbox : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private Kernel kernel;

    public UnityAction<Kernel,bool> onFilterChange;

    // Start is called before the first frame update
    void Start()
    {
        toggle.GetComponentInChildren<Text>().text = kernel.Name;
    }

    public void HandleToggle(bool isOn)
    {
        onFilterChange.Invoke(kernel,toggle.isOn);
    }

    public void SetIsOn(bool isOn)
    {
        toggle.SetIsOnWithoutNotify(isOn);
    }
}
