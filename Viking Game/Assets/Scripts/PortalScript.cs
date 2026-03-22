using UnityEngine;

public class PortalScript : MonoBehaviour
{
    public GameObject gate, e;

    public void ActivateGate() //For animation
    {
        gate.SetActive(true);
    }

    public void GoNext() // LeVel new
    {
        if (gate.activeSelf == true)
        {
            GameObject temp = GameObject.FindGameObjectWithTag("Generator");
            temp.GetComponent<RoomFirstDungeonGenerator>().GenerateDungeon(); //LETS GO, again... :3
            GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().NextFloor();
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
