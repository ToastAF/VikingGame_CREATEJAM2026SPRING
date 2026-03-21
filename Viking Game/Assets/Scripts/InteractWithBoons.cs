using UnityEngine;


public class InteracWithBoons : MonoBehaviour
{
    GameObject UICanvas;

    public void OpenUI()
    {
        UICanvas.SetActive(true);
    }

    public void CloseUI()
    {
        UICanvas.SetActive(false);
    }
}
