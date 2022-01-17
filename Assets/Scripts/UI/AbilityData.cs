using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AbilityData : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] protected TextMeshProUGUI _descriptionOutput;
    [SerializeField] protected string _abilityNameDesc;
    [SerializeField] protected UnityEngine.UI.Button _buttonRef;


    private void Start()
    {
        if (_descriptionOutput != null)
        {
            _descriptionOutput.text = _abilityNameDesc;
        }
    }

    public void UpdateTextMeshProRef()
    {
        _descriptionOutput.text = _abilityNameDesc;
    }

    public UnityEngine.UI.Button GetButtonRef()
    {
        return _buttonRef;
    }

}
