using System;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Object = UnityEngine.Object;

public class TNode : MonoBehaviour
{
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private TextMeshPro text;
    [SerializeField] private Vector2 trueOffset = new(2, 2);
    [SerializeField] private Vector2 falseOffset = new(2, -2);

    private TreeNodeBase _initParent;
    
    private int _depth;
    public TreeNodeBase Node { get; private set; }

    public event Action OnNodeInitialized;
    
    private void OnDestroy()
    {
        if (Node is TreeNodeCondition tNode) tNode.DestroyChildren();
    }
    
    private void Start()
    {
        Node = new TreeNodeAnimation(_initParent, this, THandler.Instance.levelData.animations[0]);
        OnNodeInitialized?.Invoke();
        
        if (Node is TreeNodeCondition) InitializeChildren();
        
    }

    private void InitializeChildren()
    {
        // Spawn children here if this is a condition
        if (Node is not TreeNodeCondition tCond) return;

        var trueNode = Instantiate(nodePrefab).GetComponent<TNode>();
        trueNode._initParent = Node;
        trueNode._depth = _depth + 1;
        trueNode.transform.position = transform.position + new Vector3(trueOffset.x, trueOffset.y * DepthToOffsetMultiplier(trueNode._depth));
        
        var falseNode = Instantiate(nodePrefab).GetComponent<TNode>();
        falseNode._initParent = Node;
        falseNode._depth = _depth + 1;
        falseNode.transform.position = transform.position + new Vector3(falseOffset.x, falseOffset.y * DepthToOffsetMultiplier(falseNode._depth));

        trueNode.OnNodeInitialized += () => tCond.trueNode = trueNode;
        falseNode.OnNodeInitialized += () => tCond.falseNode = falseNode;

        AddLineRenderer(trueNode.gameObject, Color.green);
        AddLineRenderer(falseNode.gameObject, Color.red);
    }

    private void AddLineRenderer(GameObject to, Color color)
    {
        var go = new GameObject("LineRendererChild");
        go.transform.SetParent(transform, false);
        
        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        
        lr.positionCount = 2;
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, to.transform.position);
        
        lr.startWidth = 0.2f;
        lr.endWidth = 0.2f;

        lr.sortingOrder = -5;
        
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = color;
        lr.endColor = color;
    }

    private static float DepthToOffsetMultiplier(float depth)
    {
        float val = -1f * (0.5f * depth - 2f);
        return Mathf.Clamp(val, 0.45f, 2f);
    }
    
    public void ConvertNode()
    {
        TLevelHandler.Instance.CurrentTrackedEdits++;
        foreach (var rend in GetComponentsInChildren<LineRenderer>())
        {
            Destroy(rend);
        }

        switch (Node)
        {
            case TreeNodeAnimation:
                Node = new TreeNodeCondition(Node.Parent, this, THandler.Instance.levelData.blackboardFields[0].name, null, null);
                InitializeChildren();
                break;
            case TreeNodeCondition tCondOld:
                tCondOld.DestroyChildren();
                Node = new TreeNodeAnimation(Node.Parent, this, THandler.Instance.levelData.animations[0]);
                break;
        }
    }
}

public abstract class TreeNodeBase
{
    public TreeNodeBase Parent { get; }
    public TNode NodeGO { get; }
    public bool Invalid { get; set; }

    protected TreeNodeBase(TreeNodeBase parent, TNode nodeGO)
    {
        Parent = parent;
        NodeGO = nodeGO;
    }

    public abstract GAnimation Resolve();
}

public class TreeNodeCondition : TreeNodeBase
{
    public string BlackboardField { get; set; }
    public TNode trueNode;
    public TNode falseNode;

    public TreeNodeCondition(TreeNodeBase parent, TNode nodeGO, string blackboardField, TNode trueNode, TNode falseNode)
        : base(parent, nodeGO)
    {
        BlackboardField = blackboardField;
        this.trueNode = trueNode;
        this.falseNode = falseNode;
    }

    public void DestroyChildren()
    {
        if (trueNode != null && trueNode.gameObject != null)
        {
            Object.Destroy(trueNode!.gameObject);
        }
        if (falseNode != null && falseNode.gameObject != null)
        {
            Object.Destroy(falseNode!.gameObject);
        }
    }
    
    public override GAnimation Resolve()
    {
        bool value = THandler.Instance.Blackboard.GetField(BlackboardField).value;
        if (trueNode is null || falseNode is null) return null;
        
        return value ? trueNode.Node.Resolve() : falseNode.Node.Resolve();
    }
}

public class TreeNodeAnimation : TreeNodeBase
{
    public GAnimation Animation { get; set; }

    public TreeNodeAnimation(TreeNodeBase parent, TNode nodeGO, GAnimation animation)
        : base(parent, nodeGO)
    {
        Animation = animation;
    }

    public override GAnimation Resolve()
    {
        if (THandler.Instance) THandler.Instance.LastResolvedNode = this;
        return Animation;
    }
}