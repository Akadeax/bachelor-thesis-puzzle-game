using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TAnimDisplay : MonoBehaviour
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
        if (_node.Node is not TreeNodeAnimation tAnim) return;
        var animations = THandler.Instance.levelData.animations;
        int index = animations.IndexOf(tAnim.Animation);

        int newIndex = index + 1;
        if (newIndex >= animations.Count)
        {
            newIndex = 0;
        }
        
        tAnim.Animation = animations[newIndex];
    }

    private void Update()
    {
        if (_node.Node is not TreeNodeAnimation tAnim) return;
        _text.text = tAnim.Animation.name;
    }
}
