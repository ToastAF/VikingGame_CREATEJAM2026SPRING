using UnityEngine;

public class PortalScript : MonoBehaviour
{
    public GameObject gate, e;
    bool canGoNext = false;

    public void ActivateGate() //For animation
    {
        gate.SetActive(true);
        canGoNext = true;
    }

    public void GoNext() // LeVel new
    {
        if (canGoNext)
        {
            GameObject temp = GameObject.FindGameObjectWithTag("Generator");
            temp.GetComponent<RoomFirstDungeonGenerator>().GenerateDungeon(); //LETS GO, again... :3
            GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().NextFloor();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            e.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            e.SetActive(false);
        }
    }
}
