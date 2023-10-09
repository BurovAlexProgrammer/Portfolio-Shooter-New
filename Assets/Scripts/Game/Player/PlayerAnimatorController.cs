using Game.Constants;
using UnityEngine;

namespace Game
{
    public class PlayerAnimatorController: MonoBehaviour
    {
        [SerializeField] private Player _player;
        [SerializeField] private Animator _animator;

        public void SetSpeed(float speed)
        {
            _animator.SetFloat(AnimatorParams.Speed, speed);
        }
    }
}