using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class AnimationEvents : MonoBehaviour {
    public List<EventData> eventDatas;

    public void TriggerAnimationEvent(string eventName) {
        EventData eventData = eventDatas.Where(_event => (_event.EventName == eventName)).FirstOrDefault();
        eventData.Event?.Invoke();
    }


    [System.Serializable]
    public class EventData {
        public UnityEvent Event;
        public string EventName;
    }
}