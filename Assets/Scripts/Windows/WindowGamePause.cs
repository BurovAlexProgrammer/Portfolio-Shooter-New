using smApplication.Scripts.Extension;
using sm_application.Scripts.Main.Events;
using sm_application.Scripts.Main.Service;
using smApplication.Scripts.UI;
using Cysharp.Threading.Tasks;
using sm_application.Scripts.Main.Service;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace sm_application.Scripts.Main.UI.Window
{
    public class WindowGamePause : WindowView
    {
        [SerializeField] private Toggle _musicToggle;
        [SerializeField] private Toggle _soundsToggle;
        [SerializeField] private Button _returnGameButton;
        [SerializeField] private Button _restartGameButton;
        [SerializeField] private Button _quitGameButton;
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private DialogView _quitGameDialog;

        private GameStateService _gameStateService;
        private SettingsService _settingsService;
        private ControlService _controlService;

        private void Awake()
        {
            _gameStateService = Services.Get<GameStateService>();
            _settingsService = Services.Get<SettingsService>();
            _controlService = Services.Get<ControlService>();
            _controlService.Controls.Menu.Pause.BindAction(BindActions.Started, ReturnGame);
            _restartGameButton.onClick.AddListener(RestartGame);
            _returnGameButton.onClick.AddListener(ReturnGame);
            _quitGameButton.onClick.AddListener(ShowQuitGameDialog);
            _mainMenuButton.onClick.AddListener(GoToMainMenu);
            _musicToggle.onValueChanged.AddListener(OnMusicSwitch);
            _soundsToggle.onValueChanged.AddListener(OnSoundsSwitch);
            _quitGameDialog.Confirm += OnQuitDialogConfirm;
            _quitGameDialog.Switched += OnSwitchDialog;
            _canvasGroup.interactable = false;
            gameObject.SetActive(false);
        }
        
        private void Start()
        {
            _musicToggle.isOn = _settingsService.Audio.MusicEnabled;
            _soundsToggle.isOn = _settingsService.Audio.SoundEnabled;
        }

        private void OnDestroy()
        {
            _controlService.Controls.Menu.Pause.UnbindAction(BindActions.Started, ReturnGame);
            _restartGameButton.onClick.RemoveListener(RestartGame);
            _returnGameButton.onClick.RemoveListener(ReturnGame);
            _quitGameButton.onClick.RemoveListener(ShowQuitGameDialog);
            _mainMenuButton.onClick.RemoveListener(GoToMainMenu);
            _musicToggle.onValueChanged.RemoveListener(OnMusicSwitch);
            _soundsToggle.onValueChanged.RemoveListener(OnSoundsSwitch);
            _quitGameDialog.Confirm -= OnQuitDialogConfirm;
            _quitGameDialog.Switched -= OnSwitchDialog;
        }

        private void OnSwitchDialog(bool state)
        {
            base.OnDialogSwitched(state);
        }

        private async void GoToMainMenu()
        {
            await Close();
            new GoToMainMenuEvent().Fire();
        }

        private async void RestartGame()
        {
            await Close();
            new RestartGameEvent().Fire();
        }
        
        private async void ReturnGame()
        {
            await Close();
            new ReturnFromPauseToGameEvent().Fire();
        }

        private void ReturnGame(InputAction.CallbackContext ctx)
        {
            new ReturnFromPauseToGameEvent().Fire();
        }

        private void OnMusicSwitch(bool newValue)
        {
            _settingsService.Audio.MusicEnabled = newValue;
            _settingsService.Save();
        }
    
        private void OnSoundsSwitch(bool newValue)
        {
            _settingsService.Audio.SoundEnabled = newValue;
            _settingsService.Save();
        }
    
        private void ShowQuitGameDialog()
        {
            _quitGameDialog.Show().Forget();
        }

        private void OnQuitDialogConfirm(bool result)
        {
            QuitDialogConfirmAsync(result).Forget();
        }
        
        private async UniTaskVoid QuitDialogConfirmAsync(bool result)
        {
            if (result)
            {
                await Close();
                _gameStateService.RestoreTimeSpeed();
                new QuitGameEvent().Fire();
                return;
            }
        
            _quitGameDialog.Close().Forget();
        }
    }
}