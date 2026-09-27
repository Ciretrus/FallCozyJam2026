using System;
using UnityEngine;

[Serializable]
public class UpgradeRadius : Item
{
    public float RadiusBonus { get; private set; } = 1.5f;

    public UpgradeRadius()
    {
        _name = "Shovel";
        _cost = 100;
        _bought = false;
        _rebuyable = false;
        RadiusBonus = 1.5f;
    }

    public UpgradeRadius(float bonus, int cost = 100)
    {
        _name = "Shovel";
        _cost = cost;
        _bought = false;
        _rebuyable = false;
        RadiusBonus = bonus;
    }

    public override void Buy()
    {
        _bought = true;
    }

    public override void Activate()
    {
        if (!_bought) return;
    }
}