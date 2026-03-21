using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements.Experimental;

[System.Serializable]
public class Boon
{
    public string godName;
    public int level;

    public Boon(string name, int lvl)
    {
        godName = name;
        level = lvl;
    }
}

public class BoonHandler : MonoBehaviour
{
    public List<Boon> boons = new List<Boon>();

    private void Start()
    {
        boons.Add(new Boon("Thor", 0));
        boons.Add(new Boon("Freya", 0));
        boons.Add(new Boon("Skadi", 0));
    }

    public void BoonChooser(int whichBoon)
    {
        UpgradeBoon(boons[whichBoon], 1);
    }

    void UpgradeBoon(Boon boonToUpgrade, int levelAmount)
    {
        boonToUpgrade.level += levelAmount;
    }
}
