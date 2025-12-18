using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class TSolution
{
    public List<string> dfsNames = new();
    
}


[CreateAssetMenu(fileName = "TLevel", menuName = "T Level Data")]
public class TLevelData : ScriptableObject
{
    public int playerBehaviorIndex = 1;
    
    public List<GBlackboardField> blackboardFields = new();
    public List<GAnimation> animations = new();

    public List<TSolution> solutions = new();

    public bool SolutionCorrect(TSolution solution)
    {
        return solutions.Any(curr => curr.dfsNames.SequenceEqual(solution.dfsNames));
    }
}