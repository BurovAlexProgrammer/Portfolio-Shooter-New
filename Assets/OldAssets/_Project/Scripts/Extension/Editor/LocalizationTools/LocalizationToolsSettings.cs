using _Project.Scripts.Main.Localizations;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Extension.Editor.LocalizationTools
{
    [CreateAssetMenu(menuName = "Custom/Localization/Settings")]
    public class LocalizationToolsSettings : ScriptableObject
    {
        [SerializeField] private DefaultAsset _localeStoreFolder;
        [SerializeField] private Locales _originalLocale;

        public Locales OriginalLocale => _originalLocale;

        public string LocalizationStorePath => _localeStoreFolder == null ? null : AssetDatabase.GetAssetPath(_localeStoreFolder);
    }
}