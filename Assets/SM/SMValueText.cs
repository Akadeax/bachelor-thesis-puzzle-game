using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class SMValueText : MonoBehaviour
{
    private TextMeshPro _text;

    bool _selectedOption;
    private bool SelectedOption
    {
        get => _selectedOption;
        set
        {
            _selectedOption = value;
            _text.text = value ? "yes" : "no";
            
            SMTransition transition = transform.parent.parent.GetComponent<SMTransition>();
            transition.associatedValue = value;
        }
    }

    private void Awake()
    {
        _text = GetComponent<TextMeshPro>();
        _text.color = Color.white;
        SelectedOption = false;
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
        SelectedOption = !SelectedOption;
    }
}