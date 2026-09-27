using System;
using UnityEngine;

[Serializable]
public class Hand : Item
{
    public Hand() {
        _name = "Hand";
        _cost = 0;
        _bought = true;
    }

    public override void Activate()
    {
        return;
    }

    public override void Buy()
    {
        return;
    }
}
