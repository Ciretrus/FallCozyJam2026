using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeReference, SubclassSelector]
    private List<Item> itemList = new();
    private int ActiveItemId = 0;
    private MoneySaver moneySaver = new();

    public void BuyItem(int itemID)
    {
        if (itemID < 0 || itemID >= itemList.Count)
        {
            return;
        }
        var item = itemList[itemID];

        if ((item.Bought && !item.Rebuyable) || moneySaver.Money < item.Cost)
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

    public MoneySaver GetMoneySaver()
    {
        return moneySaver;
    }
}