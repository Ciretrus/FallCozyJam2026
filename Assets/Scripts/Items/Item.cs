using System;
using UnityEngine;

[Serializable]
public abstract class Item
{
    [SerializeField] protected int _cost;
    [SerializeField] protected string _name;
    [SerializeField] protected bool _bought = false;
    [SerializeField] protected bool _rebuyable = false; // TODO: should we have that even?
    public int Cost => _cost;
    public string Name => _name;
    public bool Bought => _bought;
    public bool Rebuyable => _rebuyable;
    public abstract void Activate();
    public abstract void Buy();
}
