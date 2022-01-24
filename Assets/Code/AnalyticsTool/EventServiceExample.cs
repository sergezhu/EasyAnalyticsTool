using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Code.AnalyticsTool
{
    public class EventServiceExample : MonoBehaviour
    {
        [SerializeField]
        private Button _button;
        
        [Space] [SerializeField]
        private EventService _service;
        
        private bool _isSubscribed;
        private int _index;
        private string[] _types;

        private void Awake()
        {
            _index = 0;
            _types = new[] {"inapp_purchased", "level_passed", "app_exit"};
        }

        private void Start()
        {
            Subscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            if (_isSubscribed)
                return;

            _isSubscribed = true;
            _button.onClick.AddListener(OnButtonClick);
        }
        
        private void Unsubscribe()
        {
            if (_isSubscribed == false)
                return;

            _isSubscribed = false;
            _button.onClick.RemoveListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            var randomData = GetRandomData();
            
            _service.TrackEvent(randomData.Item1, randomData.Item2);
        }

        private (string, string) GetRandomData()
        {
            var type = _types[Random.Range(0, _types.Length)];
            var data = Random.Range(0, 10000).ToString("D4");

            return (type, data);
        }
    }
}