using sm_application.Scripts.Main.Service;
using sm_application.Scripts.Main.Service;
using UnityEngine;

namespace sm_application.Scripts.Main.Menu
{
    public class MenuSettingsController : MonoBehaviour
    {
        public void Apply()
        {
            var settingService = Services.Get<SettingsService>();
            settingService.Save();
            settingService.Apply();
        }
        
        public void Save()
        {
            var settingService = Services.Get<SettingsService>();
            settingService.Save();
            settingService.Apply();
        }
        
        public void ResetToDefault()
        {
            var settingService = Services.Get<SettingsService>();
            settingService.Restore();
            settingService.Apply();
        }

        public void Bind(bool value, ref bool settingsValue)
        {
            settingsValue = value;
        }
    }
}