using UnityEngine;

public class LokiDies : MonoBehaviour
{
    public GameObject portal;
    private void OnDestroy()
    {
        Instantiate(portal, transform.position, Quaternion.identity);
    }
}
