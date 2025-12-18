using System;
using TMPro;
using UnityEngine;

public class TConvertDisplay : MonoBehaviour
{
    private TextMeshPro _text;
    private TNode _node;
    
    private void Awake()
    {
        _text = GetComponent<TextMeshPro>();
        _node = transform.parent.parent.GetComponent<TNode>();
    }

    private void Start()
    {
        _text.color = Color.white;
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
        _node.ConvertNode();
    }
}