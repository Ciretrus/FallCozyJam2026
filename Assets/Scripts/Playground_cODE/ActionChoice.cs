using UnityEngine;

public class ActionChoice : MonoBehaviour
{
    public WaterCanTaker watercan;
    // public WeedTaker weeds;
    public ItemManager manager;

    void Update()
    {
        var item = manager.GetActiveItem();
        switch (item)
        {
            case WaterCan:
                watercan.TriggerUpdate();
                break;
            case Hand:
                // weeds.TriggerUpdate();
                break;
            default:
                // weeds.TriggerUpdate();
                break;
        }
    }
}