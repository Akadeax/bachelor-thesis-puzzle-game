using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TFieldDisplay : MonoBehaviour
{
    private TextMeshPro _text;
    public GBlackboardField Field { get; set; }

    private void Awake()
    {
        _text = GetComponent<TextMeshPro>();
        
        if (TTutorialHandler.Instance == null) return;
        gameObject.SetActive(SMTutorialHandler.Instance.CurrentStep?.blackboardVisible ?? true);
    }

    private void Update()
    {
        string color = Field.value ? "#00FF00" : "#FF0000";
        string valText = Field.value ? "Yes" : "No";
        _text.text = $"{Field.name}: <color={color}>{valText}</color>";
    }
}
