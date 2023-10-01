using Game.Service;
using JetBrains.Annotations;
using sm_application.Events;
using sm_application.Systems;

namespace Game.Systems
{
    [UsedImplicitly]
    public class StatisticSystem : BaseSystem
    {
        private StatisticService _statisticService;
        
        public override void RemoveEventHandlers()
        {
            RemoveListener<GoToMainMenuEvent>();
            RemoveListener<ReloadSceneEvent>();
            RemoveListener<PlayGameEvent>();
            base.RemoveEventHandlers();
        }
        public override void AddEventHandlers()
        {
            base.AddEventHandlers();
            AddListener<GoToMainMenuEvent>(OnGoToMainMenu);
            AddListener<ReloadSceneEvent>(OnRestartGame);
            AddListener<PlayGameEvent>(OnPlayGame);
        }

        private void OnPlayGame(BaseEvent obj)
        {
            _statisticService.ResetSessionRecords();
        }

        private void OnRestartGame(BaseEvent obj)
        {
            _statisticService.EndGameDataSaving();
        }

        private void OnGoToMainMenu(BaseEvent obj)
        {
            _statisticService.EndGameDataSaving();
        }
    }
}