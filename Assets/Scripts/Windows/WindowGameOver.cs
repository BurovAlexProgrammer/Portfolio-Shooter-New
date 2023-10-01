using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Service;
using sm_application.Events;
using sm_application.Service;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static sm_application.Extension.Common;

namespace sm_application.UI.Window
{
    public class WindowGameOver : WindowView
    {
        [SerializeField] private TextMeshProUGUI _killsCountText;
        [SerializeField] private TextMeshProUGUI _surviveTimeText;
        [SerializeField] private RectTransform _buttonPanel;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private Button _quitGameButton;
        [SerializeField] private DialogView _quitGameDialog;

        // [Inject] private GameStateService _gameManager;

        // public event Action Opened;
        // public event Action Closed;

        private void Awake()
        {
            _retryButton.onClick.AddListener(Retry);
            _mainMenuButton.onClick.AddListener(GoToMainMenu);
            _quitGameButton.onClick.AddListener(ShowQuitGameDialog);
            _quitGameDialog.Confirm += OnQuitDialogConfirm;
        }

        private void OnDestroy()
        {
            _retryButton.onClick.RemoveAllListeners();
            _mainMenuButton.onClick.RemoveAllListeners();
            _quitGameButton.onClick.RemoveAllListeners();
        }

        private async void Retry()
        {
            await Close();
            new ReloadSceneEvent().Fire();
        }

        public override async UniTask Show()
        {
            var statisticService = Services.Get<StatisticService>(); 
            _buttonPanel.localScale = _buttonPanel.localScale.SetAsNew(x: 0f);
            _buttonPanel.SetScale(x: 0f);
            const float duration = 0.8f;
            // var kills = statisticService.GetIntegerValue(KillMonsterCount, Session);
            // var surviveTime =
            //     Mathf.RoundToInt(statisticService.GetFloatValue(LastGameSessionDuration, Session));
            await base.Show();
            
            // await DOVirtual
            //     .Int(0, surviveTime, duration, x => _surviveTimeText.text = x.Format(StringFormat.Time))
            //     .AsyncWaitForCompletion();
            //
            // await DOVirtual
            //     .Int(0, kills, duration, x => _killsCountText.text = x.ToString())
            //     .AsyncWaitForCompletion();

            await DOVirtual
                .Float(0, 1f, 0.5f, x => _buttonPanel.SetScale(x: x))
                .AsyncWaitForCompletion();
        }

        private async void GoToMainMenu()
        {
            await Close();
            new GoToMainMenuEvent().Fire();
        } 

        private void ShowQuitGameDialog()
        {
            _quitGameDialog.Show().Forget();
        }

        private void OnQuitDialogConfirm(bool result)
        {
            if (result)
            {
                new QuitGameEvent().Fire();
                return;
            }
        
            _quitGameDialog.Close().Forget();
        }
    }
}
