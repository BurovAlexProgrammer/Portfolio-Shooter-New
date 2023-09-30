using sm_application.Context;
using sm_application.Service;
using Systems;

namespace Game.Context
{
    public class GameBoot : GameContext
    {
        protected override void Initialize()
        {
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
        }

        private void OnDestroy()
        {
            Services.Dispose();
            SystemsService.DisposeAllSystems();
        }
    }
}