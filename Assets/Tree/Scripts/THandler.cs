using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class THandler : MonoBehaviour
{
    #region Singleton
    public static THandler Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            levelData = FindObjectOfType<TLevelHandler>().GetCurrentLevel();
            Blackboard.fields.AddRange(levelData.blackboardFields);
            return;
        }
        
        Destroy(gameObject);
    }
    #endregion
    
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject sceneNodeStartPoint;
    [SerializeField] private GameObject fieldDisplayPrefab;
    
    [HideInInspector] public TLevelData levelData;
    
    public GBlackboard Blackboard { get; set; } = new();
    
    private TNode _rootNode;
    public TNode RootNode => _rootNode;
    
    public TreeNodeAnimation LastResolvedNode { get; set; }
    
    private void Start()
    {
        float height = 0f;
        foreach (var field in Blackboard.fields)
        {
            var display = Instantiate(fieldDisplayPrefab).GetComponent<TFieldDisplay>();
            var pos = transform.position;
            pos.y += height;
            height -= 1f;

            display.transform.position = pos;
            display.Field = field;
        }

        var go = Instantiate(nodePrefab);
        _rootNode = go.GetComponent<TNode>();
        _rootNode.transform.position = new Vector3(sceneNodeStartPoint.transform.position.x, sceneNodeStartPoint.transform.position.y, 0);
    }

    public bool CheckSolution()
    {
        List<string> list = TreeNodeDfs.BuildDfsList(RootNode.Node);
        TSolution solution = new TSolution
        {
            dfsNames = list
        };
        return levelData.SolutionCorrect(solution);
    }
}

public static class TreeNodeDfs
{
    public static List<string> BuildDfsList(TreeNodeBase root)
    {
        var outList = new List<string>();
        if (root == null) return outList;

        var stack = new Stack<TreeNodeBase>();
        stack.Push(root);

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            if (node == null) continue;

            switch (node)
            {
                case TreeNodeCondition cond:
                {
                    if (!string.IsNullOrEmpty(cond.BlackboardField))
                    {
                        outList.Add(cond.BlackboardField);
                    }

                    var falseChild = cond.falseNode?.Node;
                    var trueChild = cond.trueNode?.Node;

                    if (falseChild != null) stack.Push(falseChild);
                    if (trueChild != null) stack.Push(trueChild);

                    continue;
                }
                case TreeNodeAnimation anim:
                {
                    if (anim.Animation != null)
                    {
                        outList.Add(anim.Animation.name);
                    }

                    break;
                }
            }
        }

        return outList;
    }
}