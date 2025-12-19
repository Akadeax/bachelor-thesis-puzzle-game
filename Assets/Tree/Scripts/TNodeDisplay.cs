using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TNodeDisplay : MonoBehaviour
{
    [SerializeField] private Sprite animationSprite;
    [SerializeField] private Sprite conditionSprite;

    [SerializeField] private GameObject animNodeMenu;
    [SerializeField] private GameObject condNodeMenu;
    
    private TNode _node;
    private SpriteRenderer _spriteRenderer;
    private TextMeshPro _text;

    private bool isLastResolved;
    
    private void Awake()
    {
        _node = GetComponent<TNode>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _text = transform.GetChild(0).GetComponent<TextMeshPro>();
    }

    private void Start()
    {
        animNodeMenu.SetActive(false);
        condNodeMenu.SetActive(false);
    }

    private void Update()
    {
        _spriteRenderer.sprite = _node.Node is TreeNodeCondition ? conditionSprite : animationSprite;

        switch (_node.Node)
        {
            case TreeNodeCondition tCond:
                animNodeMenu.SetActive(false);
                _text.text = tCond.BlackboardField;
                break;
            case TreeNodeAnimation tAnim:
                condNodeMenu.SetActive(false);
                _text.text = tAnim.Animation.name;
                break;
        }

        isLastResolved = THandler.Instance?.LastResolvedNode == _node.Node;
        if (isHovered) return;
        _spriteRenderer.color = isLastResolved ? new Color(0.165f, 0.875f, 0.961f) : Color.white; 
    }

    private bool isHovered;
    private void OnMouseEnter()
    {
        if (!_node.Interactable) return;
        
        isHovered = true;
        if (isLastResolved)
        {
            _spriteRenderer.color = new Color(0.235f, 0.659f, 0.710f);
        }
        else
        {
            _spriteRenderer.color = new Color(0.749f, 0.973f, 1.0f);
        }
    }

    private void OnMouseExit()
    {
        isHovered = false;
        _spriteRenderer.color = Color.white;
    }

    private void OnMouseDown()
    {
        if (!_node.Interactable) return;
        
        switch (_node.Node)
        {
            case TreeNodeCondition:
                condNodeMenu.SetActive(!condNodeMenu.activeSelf);
                if (condNodeMenu.activeSelf) TLevelHandler.Instance.CurrentTrackedEdits++;
                break;
            case TreeNodeAnimation:
                animNodeMenu.SetActive(!animNodeMenu.activeSelf);
                if (animNodeMenu.activeSelf) TLevelHandler.Instance.CurrentTrackedEdits++;
                break;
        }
    }
}
