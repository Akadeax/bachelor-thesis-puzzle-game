using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class SMNode : MonoBehaviour
{
    [SerializeField] private string nodeName = "Node";
    [SerializeField] private TextMeshPro nameText;

    [HideInInspector] public List<SMTransition> transitions = new();    
    
    [SerializeField] GameObject transitionPrefab;
    
    private void Start()
    {
        nameText.text = nodeName;
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

            if (transitions.Any(x => x.From == from && x.To == to)) return;

            var trans = Instantiate(transitionPrefab).GetComponent<SMTransition>();
            trans.From = from;
            trans.To = to;
            
            bool alreadyTransitionOtherWay = transitions.Any(x => x.From == to && x.To == from);
            trans.IsOffset = alreadyTransitionOtherWay;

            from.transitions.Add(trans);
        }
    }
}
