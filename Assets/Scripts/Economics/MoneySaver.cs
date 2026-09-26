using System;

[Serializable]
public class MoneySaver
{
    public int Money { get; private set; } = 0;

    public void IncreaseMoney(int i)
    {
        Money += i;
    }

    public void DecreaseMoney(int i)
    {
        Money -= i;
    }
}