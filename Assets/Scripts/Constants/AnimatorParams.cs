using UnityEngine;

namespace Game.Constants
{
    public class AnimatorParams
    {
        private static int Hash(string paramName) => Animator.StringToHash(paramName);
        
        public static readonly int IsWalking = Hash("IsWalking");
        public static readonly int IsRunning = Hash("IsRunning");
        public static readonly int Velocity = Hash("f_Velocity");

    }
}