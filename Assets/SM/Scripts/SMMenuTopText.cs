using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SMMenuTopText : MonoBehaviour
{
    private TextMeshPro _text;
    private SMTransition _trans;

    private void Awake()
    {
        _trans = transform.parent.parent.GetComponent<SMTransition>();
        
        _text = GetComponent<TextMeshPro>();
        _text.color = Color.white;
    }

    private void Start()
    {
        string from = _trans.From.NodeAnimation.name;
        string to = _trans.To.NodeAnimation.name;

        _text.text = $"go from {from} to {to} if";
    }

}
