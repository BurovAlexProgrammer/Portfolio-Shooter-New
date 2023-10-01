using sm_application.Events;
using sm_application.Events.Audio;
using sm_application.Service;
using sm_application.Systems;

namespace Game.Systems
{
    public class AudioSystem : BaseSystem
    {
        private AudioService _audioService;
        public override void Init()
        {
            base.Init();
            _audioService = sm_application.Service.Services.Get<AudioService>();
        }

        public override void RemoveEventHandlers()
        {
            RemoveListener<PlayMenuMusicEvent>();
            base.RemoveEventHandlers();
        }

        public override void AddEventHandlers()
        {
            base.AddEventHandlers();
            AddListener<PlayMenuMusicEvent>(PlayMenuMusic);
            AddListener<PlayGameMusicEvent>(PlayGameMusic);
        }

        private void PlayGameMusic(BaseEvent obj)
        {
            _audioService.PlayMusic(AudioService.MusicPlayerState.Battle);
        }

        private void PlayMenuMusic(BaseEvent obj)
        {
            _audioService.PlayMusic(AudioService.MusicPlayerState.MainMenu);
        }
    }
}