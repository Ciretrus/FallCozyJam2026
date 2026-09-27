using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private WeedCollector weedCollector;
    [SerializeField] private ItemManager itemManager;

    private MoneySaver moneySaver;

    private void Start()
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
            Debug.LogWarning("[Economy] MoneySaver не найден!");
            return;
        }

        int reward = weed.Reward;
        moneySaver.IncreaseMoney(reward);
        Debug.Log($"[Economy] +{reward} за {weed.Type} сорняк. Всего: {moneySaver.Money}");
    }
}