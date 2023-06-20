using System;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace sm_application.Scripts.Main.Game.GameStates
{
    public abstract class GameStateBase
    {
        public bool EqualsState<T>() => this.GetType() == typeof(T);

        public virtual async UniTask EnterState()
        {
            await UniTask.Yield();
        }

        public virtual async UniTask ExitState()
        {
            await UniTask.Yield();
        }

        public virtual async UniTask Update()
        {
            await UniTask.Yield();
        }
        
        
    }
}