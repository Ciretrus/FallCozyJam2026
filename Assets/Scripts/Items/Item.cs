using System;
using UnityEngine;

[Serializable]
public abstract class Item
{
    [SerializeField] protected int _cost;
    public int Cost => _cost;

    public abstract void Activate();
}
