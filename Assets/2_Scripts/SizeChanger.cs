using UnityEngine;

public class SizeChanger : MonoBehaviour
{
    [SerializeField] private WeedTaker weedTaker;
    [SerializeField] private FlowerWater flowerWater;
    [SerializeField] private FlowerTouch flowerTouch;
    [SerializeField] private float handRadius = 1;
    void Start()
    {
        weedTaker.grabRadius = handRadius;
        flowerWater.waterRadius = handRadius;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
