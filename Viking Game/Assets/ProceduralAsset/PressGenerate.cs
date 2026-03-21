using UnityEngine;

public class PressGenerate : MonoBehaviour
{
    [SerializeField] private RoomFirstDungeonGenerator generator;

    void Start()
    {
        if (generator != null)
            generator.GenerateDungeon();
        else
            Debug.LogError("Generator reference is missing.");
    }

    void Update()
    {
       
    }
}