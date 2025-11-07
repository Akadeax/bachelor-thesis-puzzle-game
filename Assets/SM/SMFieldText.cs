using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class SMFieldText : MonoBehaviour
{
    private TextMeshPro _text;

    [CanBeNull]
    SMBlackboardField _selectedOption;
    [CanBeNull]
    private SMBlackboardField SelectedOption
    {
        get => _selectedOption;
        set
        {
            _selectedOption = value;
            _text.text = value?.name ?? "None";
            
            SMTransition transition = transform.parent.parent.GetComponent<SMTransition>();
            transition.associatedField = value;
        }
    }

    private void Awake()
    {
        _text = GetComponent<TextMeshPro>();
        _text.color = Color.white;
        SelectedOption = null;
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
        var allFields = SMHandler.Instance.Blackboard.Fields;

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