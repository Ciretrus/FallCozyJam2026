using UnityEngine;

public class ActionChoice : MonoBehaviour
{
    public WaterCanTaker watercan;
    public WeedTaker weeds;
    public ItemManager manager;

    void Update()
    {
        if (manager == null) return;

        var item = manager.GetActiveItem();
        switch (item)
        {
            case WaterCan:
                if (watercan != null)
                    watercan.TriggerUpdate();
                break;
            case Hand:
                if (weeds != null)
                    weeds.enabled = true;
                break;
            default:
                if (weeds != null)
                    weeds.enabled = true;
                break;
        }
    }
}