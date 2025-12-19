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

    private void Update()
    {
        if (TTutorialHandler.Instance is null) return;
        _text.enabled = TTutorialHandler.Instance.CurrentStep?.convertible ?? true;
    }

    private void OnMouseDown()
    {
        if (TTutorialHandler.Instance is not null)
        {
            // we are in tutorial
            if (!TTutorialHandler.Instance.CurrentStep!.convertible) return;
        }
        
        _node.ConvertNode();
    }
}