using sm_application.Context;
using sm_application.Events;
using UnityEngine;

namespace Game.Context
{
    public class Intro_SceneContext : SceneContextInstaller
    {
        [SerializeField] private float _displayDuration;
        [SerializeField] private string _nextScene;

        
        public override void Construct()
        {
        }

        public override void Dispose()
        {
            
        }

        private void Update()
        {
            _displayDuration -= Time.deltaTime;

            if (_displayDuration <= 0f)
            {
                new RequireLoadSceneEvent() { NextSceneName = _nextScene }.Fire();
                Destroy(gameObject);
            }
        }
    }
}