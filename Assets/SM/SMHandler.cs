using System;
using UnityEngine;

public class SMHandler : MonoBehaviour
{
    private Camera _camera;
    public static SMHandler Instance { get; private set; }
    
    
    public SMNode NodeDragging { get; set; }
    public Vector2 NodeDraggingOffset { get; set; }

    public SMNode NodeHovering { get; set; }
    
    public SMNode NodeTransitionStart { get; set; }
    
    
    public SMBlackboard Blackboard { get; set; }
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _camera = Camera.main;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            NodeDragging = null;
        }
        else if (Input.GetMouseButtonUp(1) && NodeTransitionStart is not null && NodeHovering is null)
        {
            NodeTransitionStart = null;
        }

        if (ReferenceEquals(NodeDragging, null)) return;
        
        Vector3 pos = _camera.ScreenToWorldPoint(Input.mousePosition);
        pos.z = transform.position.z;
        transform.position = pos;
        
        NodeDragging.transform.position = pos + new Vector3(NodeDraggingOffset.x, NodeDraggingOffset.y, 0);
    }

    private void LateUpdate()
    {
        NodeHovering = null;
    }
}
