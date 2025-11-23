using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class SMFieldText : MonoBehaviour
{
    private TextMeshPro _text;
    private SMTransition _trans;
    
    [CanBeNull]
    SMBlackboardField _selectedOption;
    [CanBeNull]
    private SMBlackboardField SelectedOption
    {
        get => _selectedOption;
        set
        {
            _selectedOption = value;
            if (string.IsNullOrWhiteSpace(value?.name))
            {
                _text.text = "None";
            }
            
            _text.text = value?.name ?? "None";
            _trans.associatedField = value;
        }
    }

    private void Awake()
    {
        _trans = transform.parent.parent.GetComponent<SMTransition>();
        
        _text = GetComponent<TextMeshPro>();
        _text.color = Color.white;
    }

    private void Start()
    {
        SelectedOption = _trans.associatedField;
    }

    private void OnMouseEnter()
    {
        _text.color = Color.cyan;
    }

    private void OnMouseExit()
    {
        _text.color = Color.white;
    }

    private void OnMouseDown()
    {
        var allFields = SMHandler.Instance.Blackboard.fields;

        if (SelectedOption == null)
        {
            SelectedOption = allFields[0];
            return;
        }
        
        var index = allFields.IndexOf(SelectedOption);
        if (index == allFields.Count - 1)
        {
            SelectedOption = null;
            return;
        }
        
        SelectedOption = allFields[index + 1];
    }
}