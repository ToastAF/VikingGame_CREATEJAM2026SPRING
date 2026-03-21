using UnityEngine;


[CreateAssetMenu(fileName = "SimpleRandomWalkData", menuName = "ProceduralAsset/SimpleRandomWalkData", order = 0)]
public class SimpleRandomWalkSO : ScriptableObject
{
    public int iterations = 10;
    public int walkLength = 10;
    public bool startRandomlyEachIteration = true;
}
