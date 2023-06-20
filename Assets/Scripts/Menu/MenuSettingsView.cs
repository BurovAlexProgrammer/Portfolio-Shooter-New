using System;
using Cysharp.Threading.Tasks;
using sm_application.Scripts.Main.Localizations;
using sm_application.Scripts.Main.Service;
using sm_application.Scripts.Main.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace sm_application.Scripts.Main.Menu
{
    public class MenuSettingsView : MenuView
    {
        [SerializeField] private MenuSettingsController _menuSettingsController;
        [SerializeField] private Button _buttonSave;
        [SerializeField] private Button _buttonReset;
        [SerializeField] private VideoSettingViews _videoSettingViews;
        [SerializeField] private GameSettingViews _gameSettingViews;
        [SerializeField] private TextMeshProUGUI _textRestartRequire;

        private SettingsService _settingsService;
        private LocalizationService _localizationService;

        private void Start()
        {
            Init();
        }
        
        private void Awake()
        {
            _settingsService = Services.Get<SettingsService>();
            _localizationService = Services.Get<LocalizationService>();
            _buttonSave.onClick.AddListener(SaveSettings);
            _buttonReset.onClick.AddListener(ResetToDefault);
            var videoSettings = _settingsService.Video;
            _videoSettingViews.AntiAliasingToggle.onValueChanged.AddListener(value => videoSettings.PostProcessAntiAliasing = value);
            _videoSettingViews.BloomToggle.onValueChanged.AddListener(value => videoSettings.PostProcessBloom = value);
            _videoSettingViews.VignetteToggle.onValueChanged.AddListener(value => videoSettings.PostProcessVignette = value);
            _videoSettingViews.AmbientOcclusionToggle.onValueChanged.AddListener(value => videoSettings.PostProcessAmbientOcclusion = value);
            _videoSettingViews.DepthOfFieldToggle.onValueChanged.AddListener(value => videoSettings.PostProcessDepthOfField = value);
            _videoSettingViews.FilmGrainToggle.onValueChanged.AddListener(value => videoSettings.PostProcessFilmGrain = value);
            _gameSettingViews.CurrentLanguage.onValueChanged.AddListener(value =>
            {
                _textRestartRequire.gameObject.SetActive(true);
                _settingsService.GameSettings.CurrentLocale = (Locales)value;
            });
            
            LoadLocalizationOptions().Forget();
        }
        
        private void OnDestroy()
        {
            _buttonSave.onClick.RemoveListener(SaveSettings);
            _buttonReset.onClick.RemoveListener(ResetToDefault);
            _videoSettingViews.AntiAliasingToggle.onValueChanged.RemoveAllListeners();
            _videoSettingViews.BloomToggle.onValueChanged.RemoveAllListeners();
            _videoSettingViews.VignetteToggle.onValueChanged.RemoveAllListeners();
            _videoSettingViews.AmbientOcclusionToggle.onValueChanged.RemoveAllListeners();
            _videoSettingViews.DepthOfFieldToggle.onValueChanged.RemoveAllListeners();
            _videoSettingViews.FilmGrainToggle.onValueChanged.RemoveAllListeners();
            _gameSettingViews.CurrentLanguage.onValueChanged.RemoveAllListeners();
        }
        
        private async UniTask LoadLocalizationOptions()
        {
            await UniTask.Yield();
            var localizations = await _localizationService.GetLocalizationsAsync();
            //_gameSettingViews.CurrentLanguage.options = localizations.Values.Select(x => new TMP_Dropdown.OptionData(x.Info.FullName)).ToList();
        }

        private void Init()
        {
            _videoSettingViews.AntiAliasingToggle.isOn = _settingsService.Video.PostProcessAntiAliasing;
            _videoSettingViews.BloomToggle.isOn = _settingsService.Video.PostProcessBloom;
            _videoSettingViews.VignetteToggle.isOn = _settingsService.Video.PostProcessVignette;
            _videoSettingViews.AmbientOcclusionToggle.isOn = _settingsService.Video.PostProcessAmbientOcclusion;
            _videoSettingViews.DepthOfFieldToggle.isOn = _settingsService.Video.PostProcessDepthOfField;
            _videoSettingViews.FilmGrainToggle.isOn = _settingsService.Video.PostProcessFilmGrain;
            _gameSettingViews.CurrentLanguage.value = (int)_settingsService.GameSettings.CurrentLocale;
        }

        private void SaveSettings()
        {
            _menuSettingsController.Save();
            GoPrevMenu();
        }
        
        private void ResetToDefault()
        {
            _menuSettingsController.ResetToDefault();
            Init();
        }
        
        [Serializable]
        private class VideoSettingViews
        {
            public Toggle AntiAliasingToggle;
            public Toggle BloomToggle;
            public Toggle VignetteToggle;
            public Toggle AmbientOcclusionToggle;
            public Toggle DepthOfFieldToggle;
            public Toggle FilmGrainToggle;
        }
        
        [Serializable]
        private class GameSettingViews
        {
            public TMP_Dropdown CurrentLanguage;
        }
    }
}
