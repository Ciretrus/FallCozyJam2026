using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private WeedCollector weedCollector;
    [SerializeField] private ItemManager itemManager;

    private MoneySaver moneySaver;

    private void Awake()
    {
        if (itemManager != null)
            moneySaver = itemManager.GetMoneySaver();
    }

    private void OnEnable()
    {
        if (weedCollector != null)
            weedCollector.OnWeedCollected += HandleWeedCollected;
    }

    private void OnDisable()
    {
        if (weedCollector != null)
            weedCollector.OnWeedCollected -= HandleWeedCollected;
    }

    private void HandleWeedCollected(Weed weed)
    {
        if (moneySaver == null)
        {
            return;
        }

        if (weed == null) return;

        int reward = weed.Reward;
        moneySaver.IncreaseMoney(reward);
    }
}