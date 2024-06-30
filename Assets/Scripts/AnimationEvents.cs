using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class AnimationEvents : MonoBehaviour {
    public List<EventData> eventDatas;

    public void TriggerAnimationEvent(EventType eventType) {
        EventData eventData = eventDatas.Where(_event => (_event.EventType == eventType)).FirstOrDefault();
        eventData.Event?.Invoke();
    }


    [System.Serializable]
    public class EventData {
        public UnityEvent Event;
        public EventType EventType;
    }

    public enum EventType {
        LeanOnMiddle,
        LeanOnFloor,
    }
}