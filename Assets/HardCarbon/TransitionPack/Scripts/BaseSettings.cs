using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HardCarbon.TransitionPack
{
    public class BaseSettings : MonoBehaviour
    {
        public enum TransitionTime { GameTime, UnscaledTime }

        [Tooltip("Run transition in on startup")]
        public bool autoRun = false;
        [Tooltip("Delay before running transition")]
        public float delay = 0;
        [Tooltip("Duration before transition is finished")]
        public float duration = .3f;
    }

    [System.Serializable]
    public class TransitionInSettings
    {
        public enum EndAction { Disable, Destroy, None }
        [Tooltip("Action made to the object after the transition")]
        public EndAction endAction = EndAction.Disable;
        public BaseSettings.TransitionTime timeUpdateMethod = BaseSettings.TransitionTime.GameTime;
        public Texture2D overlay;
        public Color color = Color.white;
        public Texture2D mask;
        public bool invertMask;
        [Range(0, 1)]
        public float softness = 0;

        [Space(10)]
        public EventCalls events;
    }

    [System.Serializable]
    public class TransitionOutSettings
    {
        public enum EndAction { Disable, Destroy, None }
        [Tooltip("Action made to the object after the transition")]
        public EndAction endAction = EndAction.Disable;
        public BaseSettings.TransitionTime timeUpdateMethod = BaseSettings.TransitionTime.GameTime;
        public Texture2D overlay;
        public Color color = Color.white;
        public Texture2D mask;
        public bool invertMask;
        [Range(0, 1)]
        public float softness = 0;

        [Space(10)]
        public EventCalls events;
        [Space(10)]
        public bool changeScene;
        public string sceneName;
    }

    [System.Serializable]
    public class EventCalls
    {
        public UnityEvent OnStart;
        public UnityEvent OnUpdate;
        public UnityEvent OnFinish;
    }
}