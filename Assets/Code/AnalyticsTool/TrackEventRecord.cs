using System;
using UnityEngine;

namespace Code.AnalyticsTool
{
    [Serializable]
    public struct TrackEventRecord
    {
        [SerializeField]
        private string _eventType;
        [SerializeField]
        private string _eventData;

        public TrackEventRecord(string eventType, string eventData)
        {
            _eventType = eventType;
            _eventData = eventData;
        }

        public string EventType => _eventType;
        public string EventData => _eventData;
    }
}