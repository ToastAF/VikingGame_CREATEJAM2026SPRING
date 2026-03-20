using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int currentFloor = 1;

    private void Awake()
    {
        Instance = this;
    }

    public float GetModifier()
    {
        return Mathf.Pow(1.1f, currentFloor);
    }
}