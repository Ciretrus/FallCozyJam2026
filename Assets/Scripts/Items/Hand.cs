using System;
using UnityEngine;

[Serializable]
public class Hand : Item
{
    public Hand()
    {
        _name = "Hand";
        _cost = 0;
        _bought = true;
        _rebuyable = false;
    }

    public override void Activate() { }
    public override void Buy() { }
}