    using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class SMValueText : MonoBehaviour
{
    private TextMeshPro _text;
    private SMTransition _trans;
    
    bool _selectedOption;
    private bool SelectedOption
    {
        get => _selectedOption;
        set
        {
            _selectedOption = value;
            _text.text = value ? "<color=#00FF00>yes</color>" : "<color=#FF0000>no</color>";
            
            _trans.associatedValue = value;
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
        SelectedOption = _trans.associatedValue;
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