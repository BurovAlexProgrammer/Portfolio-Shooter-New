using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Game.DTO;
using Newtonsoft.Json;
using sm_application.Service;
using UnityEngine;

namespace Game.Service
{
    public class StatisticService : IService, IDisposable
    {
        public Action<string, object> RecordChanged;

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

            VerifyRecord(recordName, typeof(int));
            var cachedValue = (int)_cachedSessionRecords[recordName];
            _cachedSessionRecords[recordName] = cachedValue + value;
            RecordChanged?.Invoke(recordName, _cachedSessionRecords[recordName]);
        }

        public void AddValue(string recordName, float value)
        {
            if (value == 0) return;

            VerifyRecord(recordName, typeof(float));
            var cachedValue = (float)_cachedSessionRecords[recordName];
            _cachedSessionRecords[recordName] = cachedValue + value;
            RecordChanged?.Invoke(recordName, _cachedSessionRecords[recordName]);
        }

        private void SetValue(string recordName, object value)
        {
            _cachedSessionRecords[recordName] = value;
            RecordChanged?.Invoke(recordName, _cachedSessionRecords[recordName]);
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
            SetValue(StatisticData.Scores, value);
            var maxScores = GetInteger(StatisticData.MaxScores);
            maxScores = Mathf.Max(maxScores, maxScores);
            SetValue(StatisticData.MaxScores, maxScores.ToString());
        }

        public void EndGameDataSaving()
        {
            CalculateSessionDuration();
            SaveToFile();
        }

        private void VerifyRecord(string recordName, Type type = null)
        {
            if (type == typeof(int) || type == typeof(uint))
            {
                if (!_cachedSessionRecords.ContainsKey(recordName)) _cachedSessionRecords.Add(recordName, 0);
                
                return;
            }

            if (type == typeof(float))
            {
                if (!_cachedSessionRecords.ContainsKey(recordName)) _cachedSessionRecords.Add(recordName, 0f);
                
                return;
            }

            throw new Exception("CheckRecord() not implemented type");
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
            SetValue(StatisticData.LongestGameSessionDuration, longestSession.ToString());
            SetValue(StatisticData.AverageGameSessionDuration, averageSession.ToString());
        }
    }
}