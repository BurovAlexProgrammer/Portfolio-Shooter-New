using sm_application.Scripts.Main.Service;
using Cysharp.Threading.Tasks;
using sm_application.Scripts.Main.Service;

namespace sm_application.Scripts.Main.Game.GameStates
{
    public static partial class GameState
    {
        public class CustomScene : GameStateBase
        {
            public override async UniTask EnterState()
            {
                await UniTask.Yield();
                Services.Get<GameStateService>().PrepareToPlay();
            }
        }
    }
}