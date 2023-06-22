using Events;
using Service;
using sm_application.Scripts.Main.Game;
using sm_application.Scripts.Main.Service;
using sm_application.Scripts.Main.Systems;
using Systems;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Context
{
    public class GameContextInstaller : MonoBehaviour
    {
        private static string _initSceneName;

        private void Awake()
        {
            _initSceneName ??= SceneManager.GetActiveScene().name;
            
            if (!AppContext.IsExist)
            {
                PreloadAppContext();
                return;
            }
            
            Services.Register<SceneLoaderService>();
            Services.Register<StatisticService>();
            Services.Register<GameStateService>();
            Services.Register<HttpService>();

            SystemsService.Bind<SceneLoaderSystem>();
            SystemsService.Bind<GameStateSystem>();
            SystemsService.Bind<HttpSystem>();
            
            new StartupGameInitializedEvent().Fire();
        }

        private void PreloadAppContext()
        {
            SceneManager.LoadScene("Boot");
        }

        private void OnDestroy()
        {
            Services.Dispose();
            SystemsService.Dispose();
        }
    }
}