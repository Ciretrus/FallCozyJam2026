using System;

[Serializable]
public class Chopper : Item
{
    public override void Activate()
    {
        //���� ��� ������� ������������ ������ ��� �������
    }

    public override void Buy()
    {
        _bought = true;
    }
}
