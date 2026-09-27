using UnityEngine;

public class Bush : MonoBehaviour
{
    public Flower1 flower;

    private void Awake()
    {
        flower.gameObject.SetActive(false);
    }

}