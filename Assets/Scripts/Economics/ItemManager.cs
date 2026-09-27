using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeReference, SubclassSelector]
    private List<Item> itemList = new();
    private int ActiveItemId = 0;
    private MoneySaver moneySaver = new();
    
    public void InitializeDefaultItems()
    {
        itemList.Add(new Hand());
        itemList.Add(new WaterCan(5, 1, "The Unremarkable", 10));
        itemList.Add(new WaterCan(10, 2, "The Eh One", 20));
        itemList.Add(new WaterCan(15, 3, "The Good One", 30));
        itemList.Add(new WaterCan(20, 4, "The Great One", 40));
        itemList.Add(new WaterCan(35, 5, "The MAGNIFICENT", 50));
    }

    public void BuyItem(int itemID)
    {
        if (itemID < 0 || itemID >= itemList.Count)
        {
            return;
        }

        var item = itemList[itemID];

        if (moneySaver.Money < item.Cost)
        {
            int diff = item.Cost - moneySaver.Money;
            return;
        }

        if (!item.Bought || item.Rebuyable)
        {
            moneySaver.DecreaseMoney(item.Cost); 
            item.Buy();
        }
        item.Activate();
        SetActiveItem(itemID);
    }
    public List<Item> GetItemList()
    {
        return itemList;
    }

    public int GetBudget()
    {
        return moneySaver.Money;
    }

    public Item GetActiveItem()
    {
        return itemList[ActiveItemId];
    }
    public void SetActiveItem(int id)
    {
        ActiveItemId = id;
    }
}