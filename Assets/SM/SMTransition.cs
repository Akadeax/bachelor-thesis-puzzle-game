    using System;
    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMTransition : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private EdgeCollider2D _edgeCollider;
    
    public SMNode From { get; set; }
    public SMNode To { get; set; }
    
    public bool IsOffset { get; set; }

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _edgeCollider = GetComponent<EdgeCollider2D>();
    }

    private void OnMouseOver()
    {
        print("over transition");
    }

    private void Update()
    {
        Vector3 from = From.transform.position;
        Vector3 to = To.transform.position;
        
        Vector3 cross = (to - from).normalized;
        cross = Vector3.Cross(cross, Vector3.forward).normalized * 0.2f;
        cross.z = 100f;

        Vector3 delta = IsOffset ? cross : -cross;
        
        from += cross;
        to += cross;

        Vector3 dir = to - from;
        dir.z = 0;
        dir.Normalize();

        dir *= 0.5f;
        
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
}
