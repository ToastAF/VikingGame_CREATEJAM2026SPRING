using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class InteracWithBoons : MonoBehaviour
{
    public GameObject UICanvas, textObject;
    BoonHandler boonHandler;

    private void Start()
    {
        boonHandler = GetComponent<BoonHandler>();
        UICanvas.SetActive(false);
    }

    public void UpdateBoons(string godName)
    {
        string tempText;

        switch (godName)
        {
            case "Thor":
                boonHandler.boons[0].level++;
                tempText = "You got Thor's rune! You're hands tremble with lightning.";
                break;
            case "Freya":
                tempText = "You got Freya's rune! You feel nature healing you.";
                boonHandler.boons[1].level++;
                break;
            case "Skadi":
                tempText = "You should not have touched this...";
                boonHandler.boons[2].level++;
                break;
            default:
                tempText = ">Error! No rune gotten!";
                break;
        }

        OpenUI(tempText);
    }

    public void OpenUI(string text)
    {
        UICanvas.SetActive(true);
        textObject.GetComponent<TextMeshProUGUI>().text = text;
    }

    public void CloseUI()
    {
        UICanvas.SetActive(false);
    }
}
