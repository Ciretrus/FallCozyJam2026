using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.LevelScene
{
    public class LevelWindow : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;

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

        // link to counter
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

        private void OnEnable()
        {
            var root = _uiDocument.rootVisualElement;

            // Score
            _scoreText = root.Q<Label>("ScoreText");
            SetScoreView(0);

            // Help popup
            _helpPopup = root.Q<VisualElement>("HelpPopup");
            HideHelp();
            _helpBtn = root.Q<Button>("HelpButton");
            if (_helpBtn != null)
                _helpBtn.clicked += ShowHelp;
            _closeHelpBtn = root.Q<Button>("CloseHelp");
            if (_closeHelpBtn != null)
                _closeHelpBtn.clicked += HideHelp;

            // Exit
            _exitBtn = root.Q<Button>("Help");
            if (_exitBtn != null)
                _exitBtn.clicked += ExitToStartScreen;

            // Lists
            _shopContent = root.Q<VisualElement>("ShopContent");
            _tasks = root.Q<VisualElement>("TasksTexts");

            UpdateTaskList("");
            UpdateShopList("");
        }
        // private void OnDisable()
        // {
        //     if (_helpBtn != null) _helpBtn.clicked -= ShowHelp;
        //     if (_closeHelpBtn != null) _closeHelpBtn.clicked -= HideHelp;
        //     if (_exitBtn != null) _exitBtn.clicked -= ExitToStartScreen;
        // }
        private void SetScoreView(int value)
        {
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
            Debug.Log($"EXIT");
        }

        private void UpdateTaskList(string a)
        {
            _tasks.Clear();
            var items = new List<TaskData>
            {
                new() { Text = "sword",  Completed = true },
                new() { Text = "shield", Completed = true  },
                new() { Text = "potion", Completed = false  },
                new() { Text = "bow",    Completed = false  },
                new() { Text = "staff",  Completed = true },
            };
            foreach (var data in items)
            {
                _tasks.Add(CreateTask(data));
            }
        }

        private void UpdateShopList(string a)
        {
            _shopContent.Clear();
            var items = new List<ShopItemData>
            {
                new() { Id = "sword",  Price = 100 },
                new() { Id = "shield", Price = 50  },
                new() { Id = "potion", Price = 25  },
                new() { Id = "bow",    Price = 75  },
                new() { Id = "staff",  Price = 120 },
            };
            for (int i = 0; i < items.Count - 1; ++i)
            {
                _shopContent.Add(CreateShopItem(items[i]));
                _shopContent.Add(_shopItemSpaceTemplate.Instantiate());
            }
            _shopContent.Add(CreateShopItem(items[^1]));
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

        private VisualElement CreateShopItem(ShopItemData data)
        {
            var item = _shopItemTemplate.Instantiate();
            item.name = data.Id; // ShopCost

            var cost = item.Q<Button>("ShopCost");
            if (cost != null)
            {
                cost.text = data.Price.ToString();
                cost.clicked += () => BuyView(data.Id);
            }

            var view = item.Q<VisualElement>("ShopView");
            if (view != null)
            {
                var sprite = LoadSprite(data.Id);
                if (sprite != null)
                {
                    view.style.backgroundImage = new StyleBackground(sprite);
                    view.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
                }
            }

            return item;
        }

        private Sprite LoadSprite(string id)
        {
            return Resources.Load<Sprite>($"UI/Items/{id}"); // Resources\UI\Items
        }

        private void BuyView(string itemId)
        {
            Debug.Log($"Покупка: {itemId}");
        }
    }

}
