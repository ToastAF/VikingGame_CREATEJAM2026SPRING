using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int currentFloor = 1;

    private void Awake()
    {
        Instance = this;
    }

    public void NextFloor()
    {
        currentFloor++;
        if(currentFloor > 3)
        {
            SceneManager.LoadScene(3);
        }
    }

    public float GetModifier()
    {
        return Mathf.Pow(1.1f, currentFloor);
    }
}