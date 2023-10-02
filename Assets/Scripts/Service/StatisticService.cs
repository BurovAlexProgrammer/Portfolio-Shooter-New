using System;
using System.Collections.Generic;
using System.IO;
using Game.DTO;
using Newtonsoft.Json;
using sm_application.Service;
using UnityEngine;

namespace Game.Service
{
    public class StatisticService : IService, IDisposable
    {
        public Action<string, string> RecordChanged;

        private Dictionary<string, object> _cachedSessionRecords;
        private Dictionary<string, string> _savedSessionRecords;
        private string _storedFolder;
        private string _storedFolderPath;

        public void Construct()
        {
            _cachedSessionRecords = new Dictionary<string, object>();
            _savedSessionRecords = new Dictionary<string, string>();
            _storedFolder ??= Application.dataPath + "/StoredData/";
            _storedFolderPath = _storedFolder + "Statistic.data";
            LoadFromFile();
        }

        public void Dispose()
        {
            SaveToFile();
            ResetSession();
            GC.SuppressFinalize(this);
        }

        ~StatisticService()
        {
            SaveToFile();
            ResetSession();
        }

        public string GetValue(string recordName)
        {
            return _savedSessionRecords[recordName];
        }

        public void AddValue(string recordName, int value)
        {
            if (value == 0) return;
            
            var cachedValue = (int)_cachedSessionRecords[recordName];
            _cachedSessionRecords[recordName] = cachedValue + value;
            RecordChanged?.Invoke(recordName, _cachedSessionRecords[recordName].ToString());
        }
        
        public void AddValue(string recordName, float value)
        {
            if (value == 0) return;
            
            var cachedValue = (float)_cachedSessionRecords[recordName];
            _cachedSessionRecords[recordName] = cachedValue + value;
            RecordChanged?.Invoke(recordName, _cachedSessionRecords[recordName].ToString());
        }

        public float GetFloat(string recordName)
        {
            return float.Parse(_savedSessionRecords[recordName]);
        }
        
        public int GetInteger(string recordName)
        {
            return int.Parse(_savedSessionRecords[recordName]);
        }
        
        public void ResetSession()
        {
            _cachedSessionRecords.Clear();
        }

        public void SaveToFile()
        {
            var data = JsonConvert.SerializeObject(_savedSessionRecords);
            File.WriteAllText(_storedFolderPath, data);
        }

        public void SetScores(int value)
        {
            SetRecord(StatisticData.Scores, value.ToString());
            var maxScores = GetInteger(StatisticData.MaxScores);
            maxScores = Mathf.Max(maxScores, maxScores);
            SetRecord(StatisticData.MaxScores, maxScores.ToString());
        }

        public void EndGameDataSaving()
        {
            CalculateSessionDuration();
            SaveToFile();
        }

        private void SetRecord(string recordName, string value)
        {
            _savedSessionRecords[recordName] = value;
            RecordChanged?.Invoke(recordName, _savedSessionRecords[recordName]);
        }
        
        private void LoadFromFile()
        {
            Directory.CreateDirectory(_storedFolder);

            if (!File.Exists(_storedFolderPath))
            {
                Debug.LogWarning($"Stored file '{_storedFolderPath}' not found. Created empty statistic file.");
                var emptyRecords = JsonConvert.SerializeObject(_savedSessionRecords);
                File.WriteAllText(_storedFolderPath, emptyRecords);
            }

            var json = File.ReadAllText(_storedFolderPath);
            _savedSessionRecords = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }
        
        private void CalculateSessionDuration()
        {
            var sessionDuration = GetFloat(StatisticData.LastGameSessionDuration);
            var longestSession = GetFloat(StatisticData.LongestGameSessionDuration);
            var averageSession = GetFloat(StatisticData.AverageGameSessionDuration);
            longestSession = Mathf.Max(longestSession, sessionDuration);
            averageSession = averageSession == 0 ? sessionDuration : (averageSession * 3 + sessionDuration) / 4f;
            SetRecord(StatisticData.LongestGameSessionDuration, longestSession.ToString());
            SetRecord(StatisticData.AverageGameSessionDuration, averageSession.ToString());
        }
    }
}