using System;
using UnityEngine;

[Serializable]
public class MoneySaver
{
    [SerializeField] private int _money = 10;
    public int Money => _money;

    public event Action<int> OnMoneyChanged;

    public void IncreaseMoney(int amount)
    {
        if (amount <= 0) return;
        _money += amount;
        OnMoneyChanged?.Invoke(_money);
    }

    public void DecreaseMoney(int amount)
    {
        if (amount <= 0) return;
        _money = Mathf.Max(0, _money - amount);
        OnMoneyChanged?.Invoke(_money);
    }

    public bool CanAfford(int cost) => _money >= cost;
}