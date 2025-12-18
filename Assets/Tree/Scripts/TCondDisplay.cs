using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TCondDisplay : MonoBehaviour
{
    private TextMeshPro _text;
    private TNode _node;
    
    private void Awake()
    {
        _text = GetComponent<TextMeshPro>();
        _node = transform.parent.parent.GetComponent<TNode>();
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
        if (_node.Node is not TreeNodeCondition tCond) return;
        var fields = THandler.Instance.levelData.blackboardFields;
        int index = fields.FindIndex(x => x.name == tCond.BlackboardField);

        int newIndex = index + 1;
        if (newIndex >= fields.Count)
        {
            newIndex = 0;
        }
        
        tCond.BlackboardField = fields[newIndex].name;
    }

    private void Update()
    {
        if (_node.Node is not TreeNodeCondition tCond) return;
        _text.text = tCond.BlackboardField;
    }
}