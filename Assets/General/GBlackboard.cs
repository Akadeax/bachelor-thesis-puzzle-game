using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class GBlackboardField
{
    public string name;
    public bool value;
}

public class GBlackboard
{
    public readonly List<GBlackboardField> fields = new();

    public GBlackboardField GetField(string name)
    {
        foreach (var field in fields)
        {
            if (field.name == name) return field;
        }

        return null;
    }

    public void SetFieldIfNotNull(string name, bool value)
    {
        var field = GetField(name);
        if (field == null) return;
        
        field.value = value;
    }
}