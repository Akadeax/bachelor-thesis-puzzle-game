using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SMFieldDeleteButton : MonoBehaviour
{
    private TextMeshPro _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshPro>();
        _text.color = Color.white;
    }

    private void OnMouseEnter()
    {
        _text.color = Color.red;
    }

    private void OnMouseExit()
    {
        _text.color = Color.white;
    }

    private void OnMouseDown()
    {
        var transition = transform.parent.parent.GetComponent<SMTransition>();
        transition.From.transitions.Remove(transition);
        Destroy(transition.gameObject);
        SMLevelHandler.Instance.CurrentTrackedEdits++;
    }
}
