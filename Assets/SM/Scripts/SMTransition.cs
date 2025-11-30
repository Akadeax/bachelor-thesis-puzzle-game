using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class SMTransition : MonoBehaviour
{
    [SerializeField] private float menuDistance = 2f;
    [SerializeField] private float centerOffsetDist = 0.2f;
    [SerializeField] private float nodeOffsetDistMin = 0.5f;
    [SerializeField] private float nodeOffsetDistMax = 2.0f;
    [SerializeField] private AnimationCurve nodeOffsetCurve;
    
    private LineRenderer _lineRenderer;
    private EdgeCollider2D _edgeCollider;
    
    public SMNode From { get; set; }
    public SMNode To { get; set; }

    public bool Interactable { get; set; } = true;
    
    [HideInInspector] [CanBeNull] public SMBlackboardField associatedField;
    [HideInInspector] public bool associatedValue;
    
    public bool IsOffset { get; set; }

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _edgeCollider = GetComponent<EdgeCollider2D>();
        
        _lineRenderer.startColor = Color.white;
        _lineRenderer.endColor = Color.white;
        
        var child = transform.GetChild(0);
        child.gameObject.SetActive(false);
    }

    private void OnMouseOver()
    {
        if (!Interactable) return;
        
        _lineRenderer.startColor = Color.blue;
        _lineRenderer.endColor = Color.blue;

        if (Input.GetMouseButtonDown(0))
        {
            var child = transform.GetChild(0);
            child.gameObject.SetActive(!child.gameObject.activeInHierarchy);
        }
    }

    private void OnMouseExit()
    {
        _lineRenderer.startColor = Color.white;
        _lineRenderer.endColor = Color.white;
    }

    private void Update()
    {
        Vector3 from = From.transform.position;
        Vector3 to = To.transform.position;
        
        Vector3 cross = (to - from).normalized;
        cross = Vector3.Cross(cross, Vector3.forward).normalized;
        cross.z = 10f;
        
        // Set child menu position
        var menu = transform.GetChild(0);
        Vector3 midPoint = (from + to) / 2f;
        Vector2 menuPos = midPoint + cross * menuDistance;

        menu.position = menuPos;
        
        // Set transition points in renderer
        from -= cross * centerOffsetDist;
        to -= cross * centerOffsetDist;

        Vector3 dir = to - from;
        dir.z = 0;
        dir.Normalize();

        // dir.x is 0-1, depending on x pos lerp values
        dir *= Mathf.Lerp(nodeOffsetDistMin, nodeOffsetDistMax, nodeOffsetCurve.Evaluate(dir.x));
        
        from += dir;
        to -= dir;
        
        
        _lineRenderer.SetPosition(0, from);
        _lineRenderer.SetPosition(1, to);

        List<Vector2> edges = new()
        {
            _lineRenderer.GetPosition(0),
            _lineRenderer.GetPosition(1)
        };

        _edgeCollider.SetPoints(edges);
    }

    public void MarkAsUsed()
    {
        if (!isActiveAndEnabled) return;
        
        StartCoroutine(_MarkAsUsed());
    }

    private void OnEnable()
    {
        if (!_lineRenderer)
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }
        _lineRenderer.startColor = Color.white;
        _lineRenderer.endColor = Color.white;
    }

    IEnumerator _MarkAsUsed()
    {
        if (!_lineRenderer) yield break;
        
        _lineRenderer.startColor = Color.red;
        _lineRenderer.endColor = Color.red;
        yield return new WaitForSeconds(0.2f);
        
        _lineRenderer.startColor = Color.white;
        _lineRenderer.endColor = Color.white;
    }
}
