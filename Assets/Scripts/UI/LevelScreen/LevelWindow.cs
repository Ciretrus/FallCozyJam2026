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

        private void OnEnable()
        {
            var root = _uiDocument.rootVisualElement;

            _itemManager.InitializeDefaultItems();

            _scoreText = root.Q<Label>("ScoreText");
            SetScoreView();

            var saver = _itemManager.GetMoneySaver();
            if (saver != null)
                saver.OnMoneyChanged += OnMoneyChanged;

            _helpPopup = root.Q<VisualElement>("HelpPopup");
            HideHelp();
            _helpBtn = root.Q<Button>("HelpButton");
            if (_helpBtn != null) _helpBtn.clicked += ShowHelp;
            _closeHelpBtn = root.Q<Button>("CloseHelp");
            if (_closeHelpBtn != null) _closeHelpBtn.clicked += HideHelp;

            _exitBtn = root.Q<Button>("Help");
            if (_exitBtn != null) _exitBtn.clicked += ExitToStartScreen;

            _shopContent = root.Q<VisualElement>("ShopContent");
            _tasks = root.Q<VisualElement>("TasksTexts");

            UpdateTaskList("");
            UpdateShopList("");
        }

        private void OnDisable()
        {
            var saver = _itemManager?.GetMoneySaver();
            if (saver != null)
                saver.OnMoneyChanged -= OnMoneyChanged;

            if (_helpBtn != null) _helpBtn.clicked -= ShowHelp;
            if (_closeHelpBtn != null) _closeHelpBtn.clicked -= HideHelp;
            if (_exitBtn != null) _exitBtn.clicked -= ExitToStartScreen;
        }

        private void OnMoneyChanged(int newValue)
        {
            SetScoreView();
        }

        private void SetScoreView()
        {
            if (_scoreText == null) return;
            int value = _itemManager.GetBudget();
            _scoreText.text = value.ToString();
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
                new() { Text = "Buy a hoe", Completed = false },
                new() { Text = "Use the hand trowel", Completed = false },
                new() { Text = "Use the hoe", Completed = false },
                new() { Text = "Grow more flowers", Completed = false },
                new() { Text = "Keep pulling out weeds and watering the flowers", Completed = false },
            };
            foreach (var data in items)
                _tasks.Add(CreateTask(data));
        }

        private void UpdateShopList(string a)
        {
            if (_shopContent == null) return;
            _shopContent.Clear();
            var items = _itemManager.GetItemList();

            for (int i = 0; i < items.Count - 1; ++i)
            {
                _shopContent.Add(CreateShopItem(items[i], i));
                _shopContent.Add(_shopItemSpaceTemplate.Instantiate());
            }
            _shopContent.Add(CreateShopItem(items[^1], items.Count - 1));
        }

        private VisualElement CreateTask(TaskData data)
        {
            TemplateContainer item = data.Completed
                ? _completedtasksTemplate.Instantiate()
                : _tasksTemplate.Instantiate();

            var task = item.Q<Label>("TasksText");
            if (task != null) task.text = data.Text;
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
                    var color = new Color(92f / 255f, 113f / 255f, 84f / 255f) { a = 0.5f };
                    view.style.backgroundColor = color;
                }
            }

            return item;
        }

        private Sprite GetSprite(Item data)
        {
            if (data.GetType() == typeof(WaterCan))
                return watercanAtlas.GetSprite(data.Name.Replace(" ", ""));

            return Resources.Load<Sprite>($"UI/Items/{data.Name}");
        }

        private void BuyView(int itemId)
        {
            _itemManager.BuyItem(itemId);
            UpdateShopList("");
            SetScoreView();
        }
    }
}