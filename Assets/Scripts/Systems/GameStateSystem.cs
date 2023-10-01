using JetBrains.Annotations;
using gameState = Game.Constants.GameState;
using Game.Service;
using sm_application.Events;
using sm_application.Service;
using sm_application.Systems;
using sm_application.Wrappers;
using sm_application.Extension;
using UnityEngine.InputSystem;

namespace Game.Systems
{
    [UsedImplicitly]
    public class GameStateSystem : BaseSystem
    {
        private GameStateService _gameStateService;
        private ControlService _controlService;
        private SceneLoaderService _sceneLoader;
        private bool _transaction;
        
        public override void Init()
        {
            base.Init();
            _gameStateService = sm_application.Service.Services.Get<GameStateService>();
            _controlService = sm_application.Service.Services.Get<ControlService>();
            _sceneLoader = sm_application.Service.Services.Get<SceneLoaderService>();
            _controlService.Controls.Player.Pause.BindAction(BindActions.Started, PauseGame);
        }

        public override void Dispose()
        {
            base.Dispose();
            sm_application.Service.Services.Get<ControlService>().Controls.Player.Pause.UnbindAction(BindActions.Started, PauseGame);
        }

        public override void RemoveEventHandlers()
        {
            base.RemoveEventHandlers();
            RemoveListener<GameOverEvent>();
            RemoveListener<BootAppInitializedEvent>();
            RemoveListener<IntroEndEvent>();
            RemoveListener<ReloadSceneEvent>();
            RemoveListener<ShowMainMenuEvent>();
            RemoveListener<GoToMainMenuEvent>();
            RemoveListener<QuitGameEvent>();
        }
        
        public override void AddEventHandlers()
        {
            base.AddEventHandlers();
            AddListener<GameOverEvent>(OnGameOver);
            AddListener<BootAppInitializedEvent>(StartupSystemsInitialized);
            AddListener<IntroEndEvent>(IntroEnded);
            AddListener<ReloadSceneEvent>(OnGameRestart);
            AddListener<ShowMainMenuEvent>(GoToMainMenu);
            AddListener<GoToMainMenuEvent>(GoToMainMenu);
            AddListener<QuitGameEvent>(OnQuitGame);
        }

        private void OnQuitGame(BaseEvent obj)
        {
            _gameStateService.QuitGame();
        }

        private void GoToMainMenu(BaseEvent obj)
        {
            _gameStateService.RestoreTimeSpeed();
            _gameStateService.SetState(gameState.MainMenu);
        }

        private void OnGameRestart(BaseEvent baseEvent)
        {
            _gameStateService.IsGameOver = false;
            _gameStateService.RestoreTimeSpeed();
        }

        private void IntroEnded(BaseEvent baseEvent)
        {
            new ShowMainMenuEvent().Fire();
        }

        private async void StartupSystemsInitialized(BaseEvent baseEvent)
        {
            if (_gameStateService.CurrentStateIs(gameState.CustomScene))
            {
                return;
            }
            _gameStateService.SetState(gameState.Intro);
            // await 3f.WaitInSeconds();
            await 0.3f.WaitInSeconds();
            new IntroEndEvent().Fire();
        }
        
        public async void PauseGame(InputAction.CallbackContext ctx)
        {
            if (_transaction) return;
            if (_gameStateService.CurrentStateIsNot(gameState.PlayGame, gameState.CustomScene)) return;

            Log.Info("Game paused to menu.");

            _transaction = true;
            _gameStateService.SetPause(true);
            _controlService.Controls.Player.Disable();
            _controlService.SetMenuMode();

            await _gameStateService.FluentSetTimeScale(0f, 1f);

            _controlService.Controls.Menu.Enable();
            _transaction = false;
        }
        
        public async void ReturnGame()
        {
            if (_transaction) return;
            if (_gameStateService.IsGameOver) return;
            if (_gameStateService.CurrentStateIsNot(gameState.PlayGame, gameState.CustomScene)) return;

            Log.Info("Game returned from pause.");
            
            _transaction = true;
            _gameStateService.SetPause(false);
            _controlService.Controls.Player.Enable();
            _controlService.SetPlayMode();

            await _gameStateService.FluentSetTimeScale(1f, 1f);

            _controlService.Controls.Menu.Disable();
            _transaction = false;
        }
        
        private async void OnGameOver(BaseEvent baseEvent)
        {
            Log.Info("Game Over");
            _controlService.Controls.Player.Disable();

            await _gameStateService.FluentSetTimeScale(1f, 1f);

            _controlService.SetMenuMode();
            _controlService.Controls.Menu.Enable();

            _gameStateService.GameOver();
        }
    }
}