using sm_application;
using sm_application.Context;
using sm_application.Extension;
using sm_application.Service;

namespace Game.Context
{
    public class Game_SceneContext : SceneContextInstaller
    {
        public override void Construct()
        {
            Common.ClearLogs();
            var cameraHolder = Common.FindComponentInScene<CameraHolder>();
            Services.Get<ScreenService>().SetCameraPlace(cameraHolder.transform);
        
        }

        public override void Dispose()
        {
        
        }
    

    }
}
