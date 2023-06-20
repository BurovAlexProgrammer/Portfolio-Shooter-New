using System;
using System.Collections.Generic;
using sm_application.Scripts.Main.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static smApplication.Scripts.Extension.Common;
using static sm_application.Scripts.Main.DTO.StatisticData;

namespace sm_application.Scripts.Main.Menu
{
    public class MenuStatisticView : MenuView
    {
        [SerializeField] private Button _buttonBack;
        [SerializeField] private List<TextField> _textFields;

        private void Awake()
        {
            _buttonBack.onClick.AddListener(GoPrevMenu);
        }

        private void OnDestroy()
        {
            _buttonBack.onClick.RemoveListener(GoPrevMenu);
        }

        public override UniTask Show()
        {
            for (var i = 0; i < _textFields.Count; i++)
            {
                var recordName = Enum.Parse<RecordName>(_textFields[i].Key);
                var intValue = 0;

                switch (recordName)
                {
                    case RecordName.AverageGameSessionDuration:
                        // intValue = Mathf.RoundToInt(StatisticService.GetFloatValue(recordName));
                        _textFields[i].ValueText.text = intValue.Format(StringFormat.Time);
                        break;
                    case RecordName.LongestGameSessionDuration:
                        // intValue = Mathf.RoundToInt(StatisticService.GetFloatValue(recordName));
                        _textFields[i].ValueText.text = intValue.Format(StringFormat.Time);
                        break;
                    default:
                        // var stringValue = StatisticService.GetRecord(recordName);
                        // _textFields[i].ValueText.text = stringValue;
                        break;
                }
               
            }

            return base.Show();
        }
        
    }
}