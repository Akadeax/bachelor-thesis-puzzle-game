using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SMFieldDisplay : MonoBehaviour
{
    private TextMeshPro _text;
    public SMBlackboardField Field { get; set; }

    private void Awake()
    {
        _text = GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        string color = Field.value ? "#00FF00" : "#FF0000";
        string valText = Field.value ? "Yes" : "No";
        _text.text = $"{Field.name}: <color={color}>{valText}</color>";
    }
}
