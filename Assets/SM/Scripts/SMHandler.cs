using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class SMHandler : MonoBehaviour
{
    [SerializeField] private GameObject smNodePrefab;
    [SerializeField] private GameObject fieldDisplayPrefab;
    [SerializeField] private GameObject sceneNodeStartPoint;

    [HideInInspector] public SMLevelData smLevelData;

    private Camera _camera;
    public static SMHandler Instance { get; private set; }


    public SMNode NodeDragging { get; set; }
    public Vector2 NodeDraggingOffset { get; set; }

    public SMNode NodeHovering { get; set; }

    public SMNode NodeTransitionStart { get; set; }


    public GBlackboard Blackboard { get; set; } = new GBlackboard();
    public List<GAnimation> Animations { get; set; }


    public List<SMNode> Nodes { get; set; } = new();

    public void Restart()
    {
        Nodes.Clear();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _camera = Camera.main;
            InitData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitData()
    {
        smLevelData = SMLevelHandler.Instance.GetCurrentLevel();

        foreach (GBlackboardField field in smLevelData.blackboardFields)
        {
            Blackboard.fields.Add(field);
        }
        
        float height = 0f;
        foreach (var field in Blackboard.fields)
        {
            var display = Instantiate(fieldDisplayPrefab).GetComponent<SMFieldDisplay>();
            var pos = transform.position;
            pos.y += height;
            height -= 1f;

            display.transform.position = pos;
            display.Field = field;
        }

        // Spawn initial animations and transitions
        SpawnFromLevelData(smLevelData.initialAnimations, smLevelData.initialTransitions);
    }

    public void SpawnFromLevelData(List<SMInitialNode> nodes, List<SMInitialTransition> transitions)
    {
        foreach (var go in FindObjectsOfType<SMNode>(true)) Destroy(go.gameObject);
        foreach (var go in FindObjectsOfType<SMTransition>(true)) Destroy(go.gameObject);

        foreach (SMInitialNode initialNode in nodes)
        {
            var node = MakeNewNode(initialNode.name);
            node.transform.position += new Vector3(initialNode.offset.x, initialNode.offset.y, 0);
        }

        foreach (SMInitialTransition trans in transitions)
        {
            var from = Nodes.First(x => x.NodeAnimation.name == trans.from);
            var to = Nodes.First(x => x.NodeAnimation.name == trans.to);

            SMTransition newTrans = from.MakeTransition(from, to);
            newTrans.associatedField = Blackboard.GetField(trans.field);
            newTrans.associatedValue = trans.value;
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

        node.transform.position = new Vector3(sceneNodeStartPoint.transform.position.x,
            sceneNodeStartPoint.transform.position.y, 0);

        Nodes.Add(node);

        return node;
    }

    public bool CheckSolution()
    {
        List<SMTransition> transitions = new();
        foreach (var node in Nodes)
        {
            transitions.AddRange(node.transitions);
        }

        foreach (SMSolution solution in smLevelData.solutions)
        {
            if (solution.solutionTransitions.Count != transitions.Count) continue;
            
            bool thisSolutionCorrect = true;
            foreach (SMInitialTransition correctTrans in solution.solutionTransitions)
            {
                bool correct = transitions.Any(x => x.From.NodeAnimation.name == correctTrans.from &&
                                                    x.To.NodeAnimation.name == correctTrans.to &&
                                                    x.associatedField?.name == correctTrans.field &&
                                                    x.associatedValue == correctTrans.value);

                if (correct) continue;
                thisSolutionCorrect = false;
            }

            if (thisSolutionCorrect) return true;
        }
        

        return false;
    }
}