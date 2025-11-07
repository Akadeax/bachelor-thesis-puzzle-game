using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SMNode : MonoBehaviour
{
    [SerializeField] private TextMeshPro nameText;

    [HideInInspector] public List<SMTransition> transitions = new();    
    
    [SerializeField] GameObject transitionPrefab;

    private SMAnimation _nodeAnimation;
    public SMAnimation NodeAnimation
    {
        get => _nodeAnimation;
        set
        {
            _nodeAnimation = value;
            nameText.text = _nodeAnimation.name;
        }
    }
    
    private void OnMouseOver()
    {
        SMHandler.Instance.NodeHovering = this;
        
        if (Input.GetMouseButtonDown(0))
        {
            SMHandler.Instance.NodeDragging = this;
            
            Vector3 mouseWorldPos = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
            SMHandler.Instance.NodeDraggingOffset = transform.position - mouseWorldPos; 
        }

        if (Input.GetMouseButtonDown(1))
        {
            SMHandler.Instance.NodeTransitionStart = this;
        }

        bool hoveringAny = SMHandler.Instance.NodeHovering is not null;
        bool hasStart = SMHandler.Instance.NodeTransitionStart is not null;
        bool startNotSelf = SMHandler.Instance.NodeTransitionStart != this;
        if (Input.GetMouseButtonUp(1) && hoveringAny && hasStart && startNotSelf)
        {
            SMNode from = SMHandler.Instance.NodeTransitionStart;
            SMNode to = this;

            MakeTransition(from, to);
        }
    }

    public void MakeTransition(SMNode from, SMNode to)
    {
        if (transitions.Any(x => x.From == from && x.To == to)) return;

        var trans = Instantiate(transitionPrefab).GetComponent<SMTransition>();
        trans.From = from;
        trans.To = to;
            
        bool alreadyTransitionOtherWay = transitions.Any(x => x.From == to && x.To == from);
        trans.IsOffset = alreadyTransitionOtherWay;

        from.transitions.Add(trans);
    }
}
