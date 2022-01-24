using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Code.AnalyticsTool
{
    public class EventService : MonoBehaviour
    {
        [SerializeField]
        private string _url;
        [SerializeField] [Min(0)]
        private float _cooldownBeforeSend = 5;
    
        private const string PlayerPrefsKey = "AnalyticEvents";
    
        private Queue<TrackEventRecord> _eventRecords;
        private WaitForSeconds _cooldownWaiter;
        private int _sentRecordsCount;

        private void Start()
        {
            _cooldownWaiter = new WaitForSeconds(_cooldownBeforeSend);
            _sentRecordsCount = 0;
        
            LoadEvents();
            StartCoroutine(ContinuesRequesting());
        }

        private void OnDestroy()
        {
            SaveEvents();
        }

        public void TrackEvent(string type, string data)
        {
            var record = new TrackEventRecord(type, data);
        
            Debug.Log($"enqueue : {record.EventType} , {record.EventData}");
            _eventRecords.Enqueue(record);
        }

        private void LoadEvents()
        {
            if (PlayerPrefs.HasKey(PlayerPrefsKey) == false)
                _eventRecords = new Queue<TrackEventRecord>();
            else
            {
                var wrapper = JsonUtility.FromJson<TrackEventRecordsWrapper>(PlayerPrefs.GetString(PlayerPrefsKey));
                _eventRecords = new Queue<TrackEventRecord>(wrapper.Records);
            }
        }

        private void SaveEvents()
        {
            var wrapper = PrepareWrapper();

            var json = JsonUtility.ToJson(wrapper);
            PlayerPrefs.SetString(PlayerPrefsKey, json);
        }

        private IEnumerator ContinuesRequesting()
        {
            while (true)
            {
                yield return _cooldownWaiter;
            
                if(_eventRecords.Count > 0)
                    yield return SendRequest();
            }
        }

        private IEnumerator SendRequest()
        {
            var wwwForm = new WWWForm();
            var request = UnityWebRequest.Post(_url, wwwForm);

            var wrapper = PrepareWrapper();
            _sentRecordsCount = wrapper.Records.Length;
        
            var json = JsonUtility.ToJson(wrapper);
            var bytesJson = Encoding.UTF8.GetBytes(json);
            var uploadHandler = new UploadHandlerRaw(bytesJson);

            request.uploadHandler = uploadHandler;
        
            request.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");

            yield return request.SendWebRequest();

            Debug.Log($"data count : {wrapper.Records.Length}, response : {request.responseCode}, {request.result}");

            if (request.responseCode == 200)
            {
                Debug.Log($" {_sentRecordsCount} records are successfully sent");

                for (int i = 0; i < _sentRecordsCount; i++)
                    _eventRecords.Dequeue();
            }
        }

        private TrackEventRecordsWrapper PrepareWrapper()
        {
            var wrapper = new TrackEventRecordsWrapper();

            if (_eventRecords == null)
                _eventRecords = new Queue<TrackEventRecord>();

            wrapper.Records = _eventRecords.ToArray();
            return wrapper;
        }
    }
}
