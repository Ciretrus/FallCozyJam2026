using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UIElements;

namespace UI.LevelScene
{
    public class LevelWindow : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        public ItemManager _itemManager;

        private class ShopItemData
        {
            public string Id;
            public int Price;
            public Sprite Icon;
        }
        private class TaskData
        {
            public string Text;
            public bool Completed;
        }

        private Label _scoreText;

        [SerializeField] private VisualTreeAsset _shopItemTemplate;
        [SerializeField] private VisualTreeAsset _shopItemSpaceTemplate;
        private VisualElement _shopContent;

        [SerializeField] private VisualTreeAsset _completedtasksTemplate;
        [SerializeField] private VisualTreeAsset _tasksTemplate;
        private VisualElement _tasks;

        private VisualElement _helpPopup;
        private Button _helpBtn;
        private Button _closeHelpBtn;

        private Button _exitBtn;

        [SerializeField] private SpriteAtlas watercanAtlas;

        private MoneySaver _moneySaver;

        private void OnEnable()
        {
            if (_uiDocument == null)
            {
                Debug.LogError("[LevelWindow] _uiDocument не назначен в инспекторе!");
                return;
            }

            var root = _uiDocument.rootVisualElement;

            _scoreText = root.Q<Label>("ScoreText");
            if (_scoreText == null)
                Debug.LogError("[LevelWindow] Label 'ScoreText' не найден в UIDocument!");

            if (_itemManager != null)
            {
                _moneySaver = _itemManager.GetMoneySaver();
                if (_moneySaver != null)
                {
                    _moneySaver.OnMoneyChanged += OnMoneyChanged;
                }
            }
            else
            {
                Debug.LogError("[LevelWindow] _itemManager не назначен в инспекторе!");
            }

            SetScoreView();

            _helpPopup = root.Q<VisualElement>("HelpPopup");
            HideHelp();
            _helpBtn = root.Q<Button>("HelpButton");
            if (_helpBtn != null)
                _helpBtn.clicked += ShowHelp;
            _closeHelpBtn = root.Q<Button>("CloseHelp");
            if (_closeHelpBtn != null)
                _closeHelpBtn.clicked += HideHelp;

            _exitBtn = root.Q<Button>("Help");
            if (_exitBtn != null)
                _exitBtn.clicked += ExitToStartScreen;

            _shopContent = root.Q<VisualElement>("ShopContent");
            _tasks = root.Q<VisualElement>("TasksTexts");

            UpdateTaskList("");
            UpdateShopList("");

            if (_helpBtn != null)
            {
                _helpBtn.clicked += ShowHelp;
                _helpBtn.clicked += () => SoundManager.Instance?.PlayUIButton();
            }

            if (_closeHelpBtn != null)
            {
                _closeHelpBtn.clicked += HideHelp;
                _closeHelpBtn.clicked += () => SoundManager.Instance?.PlayUIButton();
            }

            if (_exitBtn != null)
            {
                _exitBtn.clicked += ExitToStartScreen;
                _exitBtn.clicked += () => SoundManager.Instance?.PlayUIButton();
            }
        }

        private void OnDisable()
        {
            if (_moneySaver != null)
            {
                _moneySaver.OnMoneyChanged -= OnMoneyChanged;
            }

            if (_helpBtn != null) _helpBtn.clicked -= ShowHelp;
            if (_closeHelpBtn != null) _closeHelpBtn.clicked -= HideHelp;
            if (_exitBtn != null) _exitBtn.clicked -= ExitToStartScreen;
        }

        private void OnMoneyChanged(int newValue)
        {
            if (_scoreText != null)
                _scoreText.text = newValue.ToString();
        }

        private void SetScoreView()
        {
            if (_itemManager == null || _scoreText == null) return;
            _scoreText.text = _itemManager.GetBudget().ToString();
        }

        private void ShowHelp()
        {
            if (_helpPopup == null) return;
            _helpPopup.style.display = DisplayStyle.Flex;
            _helpPopup.BringToFront();
        }

        private void HideHelp()
        {
            if (_helpPopup == null) return;
            _helpPopup.style.display = DisplayStyle.None;
        }

        private void ExitToStartScreen()
        {
            if (SceneTransition.Instance != null)
                SceneTransition.Instance.LoadScene("StartScreen");
        }

        private void UpdateTaskList(string a)
        {
            _tasks.Clear();
            var items = new List<TaskData>
            {
                new() { Text = "Pull out all the weeds", Completed = false },
                new() { Text = "Pull out a weed", Completed = false },
                new() { Text = "Water the flower", Completed = false },
                new() { Text = "Pull out a weed", Completed = false },
                new() { Text = "Grow more flowers, you can't water a flower while there are weeds nearby", Completed = false },
                new() { Text = "Buy a hoe", Completed = false },
                new() { Text = "Pull it out", Completed = false },
                new() { Text = "Use the hand trowel", Completed = false },
                new() { Text = "Use the hoe", Completed = false },
                new() { Text = "Keep pulling out weeds and watering the flowers until a picture appears", Completed = false },
            };
            foreach (var data in items)
            {
                _tasks.Add(CreateTask(data));
            }
        }

        private void UpdateShopList(string a)
        {
            if (_shopContent == null || _itemManager == null) return;

            _shopContent.Clear();
            var items = _itemManager.GetItemList();
            if (items == null || items.Count == 0) return;

            for (int i = 0; i < items.Count - 1; ++i)
            {
                _shopContent.Add(CreateShopItem(items[i], i));
                _shopContent.Add(_shopItemSpaceTemplate.Instantiate());
            }
            _shopContent.Add(CreateShopItem(items[^1], items.Count - 1));
        }

        private VisualElement CreateTask(TaskData data)
        {
            TemplateContainer item;
            if (!data.Completed)
            {
                item = _tasksTemplate.Instantiate();
            }
            else
            {
                item = _completedtasksTemplate.Instantiate();
            }

            var task = item.Q<Label>("TasksText");
            if (task != null)
            {
                task.text = data.Text;
            }

            return item;
        }

        private VisualElement CreateShopItem(Item data, int id)
        {
            var item = _shopItemTemplate.Instantiate();
            item.name = id.ToString();

            var cost = item.Q<Button>("ShopCost");
            if (cost != null)
            {
                if (!data.Bought)
                {
                    cost.text = data.Cost.ToString();
                }
                else
                {
                    cost.text = "USE";
                    var color = cost.style.backgroundColor.value;
                    color.a = 0.5f;
                    cost.style.backgroundColor = color;
                }
                cost.clicked += () => BuyView(id);
            }

            var view = item.Q<VisualElement>("ShopView");
            if (view != null)
            {
                var sprite = GetSprite(data);
                if (sprite != null)
                {
                    view.style.backgroundImage = new StyleBackground(sprite);
                    view.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);
                }
                else
                {
                    var color = new Color(92f / 255f, 113f / 255f, 84f / 255f)
                    {
                        a = 0.5f
                    };
                    view.style.backgroundColor = color;
                }
            }

            return item;
        }

        private Sprite GetSprite(Item data)
        {
            if (data.GetType() == typeof(WaterCan))
            {
                if (watercanAtlas != null)
                {
                    var sprite = watercanAtlas.GetSprite(data.Name.Replace(" ", ""));
                    if (sprite != null) return sprite;
                }
            }
            return Resources.Load<Sprite>($"UI/Items/{data.Name.Replace(" ", "")}");
        }

        private void BuyView(int itemId)
        {
            _itemManager.BuyItem(itemId);
            UpdateShopList("");
            SetScoreView();
        }
    }
}