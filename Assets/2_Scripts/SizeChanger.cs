using UnityEngine;

public class SizeChanger : MonoBehaviour
{
    [SerializeField] private Transform cursor;
    [SerializeField] private WeedTaker weedTaker;
    [SerializeField] private FlowerWater flowerWater;
    [SerializeField] private FlowerTouch flowerTouch;
    [SerializeField] private float handRadius = 1;
    [SerializeField] private AudioSource sound;
    void Start()
    {
        weedTaker.grabRadius = handRadius;
        flowerWater.waterRadius = handRadius;
        flowerTouch.touchRadius = handRadius;
    }

    public void ChangeSize(float newSize)
    {
        cursor.localScale = Vector3.one * newSize;
        weedTaker.grabRadius = newSize;
        flowerWater.waterRadius = newSize;
        flowerTouch.touchRadius = newSize;
        sound.PlayWithPitch(sound.clip);
    }

    
}
