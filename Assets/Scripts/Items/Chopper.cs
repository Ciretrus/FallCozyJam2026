using System;
using UnityEngine;

[Serializable]
public class Chopper : Item
{
    public int WeedsPerUse { get; private set; } = 4;
    public int RemainingUses { get; private set; } = 1;

    public Chopper()
    {
        _name = "Chopper";
        _cost = 15;
        _bought = false;
        _rebuyable = false;
        WeedsPerUse = 4;
        RemainingUses = 1;
    }

    public Chopper(int cost)
    {
        _name = "Chopper";
        _cost = cost;
        _bought = false;
        _rebuyable = false;
        WeedsPerUse = 4;
        RemainingUses = 1;
    }

    public override void Buy()
    {
        _bought = true;
        RemainingUses = 1;
    }

    public override void Activate()
    {
        if (!_bought || RemainingUses <= 0) return;
    }

    public bool TryUse()
    {
        if (RemainingUses <= 0) return false;
        RemainingUses--;
        return true;
    }
}