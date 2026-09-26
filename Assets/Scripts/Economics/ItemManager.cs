using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeReference, SubclassSelector]
    private List<Item> itemList = new();

    private MoneySaver moneySaver = new();

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

        moneySaver.DecreaseMoney(item.Cost);
        item.Activate();
    }
}