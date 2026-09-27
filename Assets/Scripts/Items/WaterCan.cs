using System;

[Serializable]
public class WaterCan : Item
{
    public int Radius { get; private set; }
    public int WaterLevel { get; private set; }
    public WaterCan(int radius, int waterLevel, string name = "", int cost = 20) {
        _name = ("Water Can " + name).TrimEnd();
        _cost = cost;
        _bought = false;

        Radius = radius;
        WaterLevel = waterLevel;
    }

    public override void Activate()
    {
        //���� ��� ������� ������������ ������ ��� �������
    }

    public override void Buy()
    {
        _bought = true;
    }
}
