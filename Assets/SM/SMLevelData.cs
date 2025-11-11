using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class SMInitialTransition
{
    public string from, to;
    public string field;
    public bool value;
}

[System.Serializable]
public class SMInitialNode
{
    public string name;
    public Vector2 offset;
}


[CreateAssetMenu(fileName = "SMLevel", menuName = "SM Level Data")]
public class SMLevelData : ScriptableObject
{
    public int playerBehaviorIndex = 1;
    
    public List<SMBlackboardField> blackboardFields = new();
    public List<SMAnimation> animations = new();

    public List<SMInitialNode> initialAnimations = new();
    public List<SMInitialTransition> initialTransitions = new();
}