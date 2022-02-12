using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMenu : MonoBehaviour
{
    [SerializeField] private RectTransform _gameObjectParent;
    [SerializeField] private GameObject _baseGameObjectClass;
    private List<GameObject> AbilityClasses;

    [SerializeField] enum AbilityType
    {
        Melee,
        Range
    }
    GameObject _currentObjectReference;
    AbilityData _abilityDataRef;
    Component _currentComponent;    // Start is called before the first frame update
    void Start()
    {
        AbilityClasses = new List<GameObject>();

        //To allow for testing, the alpha must be visible. The given scene, however, assumes all UI is invisible until set otherwise. This will allow for both criteria to be met.
        GetComponent<CanvasGroup>().alpha = 0;
    }

#if UNITY_EDITOR
    Transform _playerRef;
#endif

    private void FixedUpdate()
    {
#if UNITY_EDITOR
        //if (_playerRef == null)
        //{
        //    _playerRef = PriorityChar_Manager.instance.getPlayer().transform;
        //
        //    UIElement temp;
        //    if (_playerRef != null)
        //        for (int i = 0; i < 25; ++i)
        //        {
        //            temp = new UIElement();
        //            AddClass(temp);
        //        }
        //}

#endif
    }
    public void AddClass(UIElement ability)
    {
        //Ability is only for holding the exact inherited class type. Figure out what it is and add a component based on it. The prefab should hold a script with any needed unique data.
        _currentObjectReference = Instantiate(_baseGameObjectClass, _gameObjectParent.transform);

        _abilityDataRef = _currentObjectReference.GetComponent<AbilityData>();
        _currentComponent = _currentObjectReference.AddComponent(ability.GetType());
//        (_currentComponent as UIElement).Setup(_abilityDataRef, _playerRef.GetController()); //Noted code to pass controller to element, in case of requiring player input.
        AbilityClasses.Add(_currentObjectReference);
    }

    public void RemoveObject(int ID)
    {
        if (ID > 0 && ID < AbilityClasses.Count)
            AbilityClasses.RemoveAt(ID);
    }
}
