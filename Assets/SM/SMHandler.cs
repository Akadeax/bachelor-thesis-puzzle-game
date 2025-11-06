using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class SMHandler : MonoBehaviour
{
    [SerializeField] private GameObject smNodePrefab;
    [SerializeField] private SMLevelData smLevelData;
    [SerializeField] private GameObject sceneNodeStartPoint;
    
    private Camera _camera;
    public static SMHandler Instance { get; private set; }


    public SMNode NodeDragging { get; set; }
    public Vector2 NodeDraggingOffset { get; set; }

    public SMNode NodeHovering { get; set; }

    public SMNode NodeTransitionStart { get; set; }


    public SMBlackboard Blackboard { get; set; }
    public List<SMAnimation> Animations { get; set; }


    public List<SMNode> Nodes { get; set; } = new();
    

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

    private void Start()
    {
        // Spawn initial animations and transitions
        foreach (SMInitialNode initialNode in smLevelData.initialAnimations)
        {
            var node = MakeNewNode(initialNode.name);
            node.transform.position += new Vector3(initialNode.offset.x, initialNode.offset.y, 0);
        }

        foreach (SMInitialTransition trans in smLevelData.initialTransitions)
        {
            var from = Nodes.First(x => x.NodeAnimation.name == trans.from);
            var to = Nodes.First(x => x.NodeAnimation.name == trans.to);
            
            from.MakeTransition(from, to);
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


    public SMNode MakeNewNode(string newAnimationName)
    {
        var node = Instantiate(smNodePrefab).GetComponent<SMNode>();
        
        Assert.IsTrue(smLevelData.animations.Any(x => x.name == newAnimationName));
        node.NodeAnimation = smLevelData.animations.First(x => x.name == newAnimationName);
        
        node.transform.position = new Vector3(sceneNodeStartPoint.transform.position.x, sceneNodeStartPoint.transform.position.y, 0);
        
        Nodes.Add(node);
        
        return node;
    }
}