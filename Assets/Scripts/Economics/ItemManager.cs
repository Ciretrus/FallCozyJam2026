using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeReference, SubclassSelector]
    private List<Item> itemList = new();
    private int ActiveItemId = 0;
    [SerializeField] private MoneySaver moneySaver = new();

    public event System.Action OnItemsChanged;

    public void InitializeDefaultItems()
    {
        if (itemList.Count > 0) return;

        itemList.Add(new Hand());
        itemList.Add(new Chopper(15));
        itemList.Add(new UpgradeRadius(1.5f, 100));
        itemList.Add(new WaterCan(5, 1, "Basic", 300));
    }

    public void BuyItem(int itemID)
    {
        if (itemID < 0 || itemID >= itemList.Count) return;

        var item = itemList[itemID];

        if (!item.Bought && !moneySaver.CanAfford(item.Cost))
        {
            Debug.Log($"[Shop] Недостаточно денег. Нужно {item.Cost}, есть {moneySaver.Money}");
            return;
        }

        if (!item.Bought || item.Rebuyable)
        {
            moneySaver.DecreaseMoney(item.Cost);
            item.Buy();
            OnItemsChanged?.Invoke();
        }

        item.Activate();
        SetActiveItem(itemID);
    }

    public List<Item> GetItemList() => itemList;
    public int GetBudget() => moneySaver.Money;
    public MoneySaver GetMoneySaver() => moneySaver;

    public Item GetActiveItem()
    {
        if (ActiveItemId < 0 || ActiveItemId >= itemList.Count) return null;
        return itemList[ActiveItemId];
    }

    public void SetActiveItem(int id)
    {
        if (id < 0 || id >= itemList.Count) return;
        ActiveItemId = id;
    }
}