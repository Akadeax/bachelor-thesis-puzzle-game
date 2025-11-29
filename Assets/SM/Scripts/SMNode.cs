using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SMNode : MonoBehaviour
{
    [SerializeField] private TextMeshPro nameText;

    [HideInInspector] public List<SMTransition> transitions = new();    
    
    [SerializeField] GameObject transitionPrefab;

    private SpriteRenderer _spriteRenderer;
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

    public bool Interactable { get; set; } = true;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.color = Color.white;
    }

    private void OnMouseOver()
    {
        if (!Interactable) return;
        
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
            SMLevelHandler.Instance.CurrentTrackedEdits++;
        }
    }

    public SMTransition MakeTransition(SMNode from, SMNode to)
    {
        if (from.transitions.Any(x => x.From == from && x.To == to)) return null;

        var trans = Instantiate(transitionPrefab).GetComponent<SMTransition>();
        trans.From = from;
        trans.To = to;
        trans.associatedField = null;
        trans.associatedValue = false;
        
        bool alreadyTransitionOtherWay = transitions.Any(x => x.From == to && x.To == from);
        trans.IsOffset = alreadyTransitionOtherWay;

        from.transitions.Add(trans);

        return trans;
    }

    public void ActivateNode()
    {
        if (!_spriteRenderer) return;
        _spriteRenderer.color = Color.cyan;
    }

    public void DeactivateNode()
    {
        if (!_spriteRenderer) return;
        _spriteRenderer.color = Color.white;
    }
}
