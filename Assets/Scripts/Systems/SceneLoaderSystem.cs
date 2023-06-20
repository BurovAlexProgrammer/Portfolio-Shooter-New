using sm_application.Scripts.Main.DTO.Enums;
using sm_application.Scripts.Main.Events;
using sm_application.Scripts.Main.Service;
using sm_application.Scripts.Main.Wrappers;
using Cysharp.Threading.Tasks;

namespace sm_application.Scripts.Main.Systems
{
    public class SceneLoaderSystem : BaseSystem
    {
        private SceneLoaderService _sceneLoader;
        private GameStateService _gameStateService;

        public override void Init()
        {
            base.Init();
            _sceneLoader = Services.Get<SceneLoaderService>();
            _gameStateService = Services.Get<GameStateService>();
            
            if (_sceneLoader.IsCustomScene())
            {
                _gameStateService.SetState(GameState.CustomScene);
            }
        }
        
        public override void RemoveEventHandlers()
        {
            RemoveListener<StartupSystemsInitializedEvent>();
            RemoveListener<ShowMainMenuEvent>();
            RemoveListener<RestartGameEvent>();
            base.RemoveEventHandlers();
        }

        public override void AddEventHandlers()
        {
            base.AddEventHandlers();
            AddListener<StartupSystemsInitializedEvent>(StartupSystemsInitialized);
            AddListener<ShowMainMenuEvent>(ShowMainMenu);
            AddListener<RestartGameEvent>(OnRestartGame);
        }

        private void OnRestartGame(BaseEvent obj)
        {
            _sceneLoader.ReloadActiveScene();
        }

        private void ShowMainMenu(BaseEvent obj)
        {
            _sceneLoader.LoadSceneAsync(SceneName.MainMenu).Forget();
        }

        private void StartupSystemsInitialized(BaseEvent evnt)
        {
            Log.Info("Initialized");
            // if (_gameStateService.CurrentStateIs(GameState.CustomScene))
            // {
            //     return;
            // }
            // _sceneLoader.LoadSceneAsync(SceneName.Intro).Forget();
        }
    }
}