using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SMBlackboardField
{
    public string name;
    public bool value;
}

public class SMBlackboard
{
    public List<SMBlackboardField> Fields = new();
}
