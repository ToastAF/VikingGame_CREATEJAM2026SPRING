using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoonInteract : MonoBehaviour
{
    public GameObject eInteract;
    public InteracWithBoons booningScript;
    public string godName;


    public void DoInteraction()
    {
        booningScript.UpdateBoons(godName);
        Debug.Log("Picked up rune!");
        Destroy(gameObject);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            booningScript = collision.gameObject.GetComponent<InteracWithBoons>();
            eInteract.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            eInteract.SetActive(false);
        }
    }
}
