using System;
using UnityEngine;

[Serializable]
public class WaterCan : Item
{
    public int Radius { get; private set; } = 5;
    public int WaterLevel { get; private set; } = 1;

    public WaterCan()
    {
        _name = "Water Can";
        _cost = 300;
        _bought = false;
        Radius = 5;
        WaterLevel = 1;
    }

    public WaterCan(int radius, int waterLevel, string name = "", int cost = 300)
    {
        _name = ("Water Can " + name).TrimEnd();
        _cost = cost;
        _bought = false;
        Radius = radius;
        WaterLevel = waterLevel;
    }

    public override void Activate()
    {
        if (!_bought) return;
    }

    public override void Buy()
    {
        _bought = true;
    }
}