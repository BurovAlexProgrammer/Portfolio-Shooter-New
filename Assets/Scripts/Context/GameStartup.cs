using Events;
using sm_application.Constants;
using sm_application.Context;
using sm_application.Service;
using Systems;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Context
{
    public class GameStartup : MonoBehaviour
    {
        private static string _initSceneName;

        private void Awake()
        {
            _initSceneName ??= SceneManager.GetActiveScene().name;
            
            if (!AppContext.IsExist)
            {
                PreloadBootScene();
                return;
            }
            
            Services.Register<StatisticService>();
            Services.Register<GameStateService>();
            Services.Register<HttpService>();
            Services.Register<UserService>();

            SystemsService.Bind<ControlSystem>();
            SystemsService.Bind<GameStateSystem>();
            SystemsService.Bind<HttpSystem>();
            SystemsService.Bind<UserSystem>();
            SystemsService.Bind<AudioSystem>();
            SystemsService.Bind<SceneLoaderSystem>();
            
            new GameStartupInitializedEvent().Fire();
        }

        private void PreloadBootScene()
        {
            SceneManager.LoadScene(App.BootScene);
        }

        private void OnDestroy()
        {
            Services.Dispose();
            SystemsService.Dispose();
        }
    }
}