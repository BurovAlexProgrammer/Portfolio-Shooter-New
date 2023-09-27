using sm_application.Extension;
using Cysharp.Threading.Tasks;

namespace sm_application.Game.GameStates
{
    public static partial class GameState
    {
        public class Boot : GameStateBase
        {
            public override async UniTask EnterState()
            {
                //Services.Get<SceneLoaderService>().ShowScene();
                await 3f.WaitInSeconds();
            }
        }
    }
}