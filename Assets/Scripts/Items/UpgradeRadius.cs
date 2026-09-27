using System;

[Serializable]
public class UpgradeRadius : Item
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
