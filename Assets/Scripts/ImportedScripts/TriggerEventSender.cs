using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace ImportedScripts
{
    /// <summary>
    /// Class for using a trigger event in a different component.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class TriggerEventSender : MonoBehaviour, IFill
    {
        public delegate void ColliderEvent(Collider other);
        public event ColliderEvent OnEnter;
        public event ColliderEvent OnStay;
        public event ColliderEvent OnExit;

        [SerializeField] private new Collider collider;
        [Tooltip("Leave empty to avoid checking for tag.")]
        [SerializeField] private string checkVsTag = string.Empty;
        [SerializeField] private bool triggerOncePerSession;
        [Header("Delayed Trigger")]
        [SerializeField] private bool useDelay = false;
        [SerializeField, Min(0f)] private float triggerDelay = 1f;
        [SerializeField, Min(0f)] private float delayDrainSpeed = 1f;
        public UnityEvent onEnterNoDelay;
        public UnityEvent onEnter;
        public UnityEvent onStay;
        public UnityEvent onExit;
        [Header("Debug")]
        [SerializeField] private bool showGizmos;
        [SerializeField] private Color gizmoColor = new Color (1,0,1,0.5f);
        private bool triggeredThisSession;
        private int objectsInsideTrigger = 0;
        private float delayCounter;
        private bool triggeredDelayedEnter;

        public Bounds TriggerBounds => collider.bounds;
        public bool TriggerEnabled
        {
            get => collider.enabled;
            set => collider.enabled = value;
        }
        public int ObjectsInsideTrigger
        {
            get => objectsInsideTrigger;
            set => objectsInsideTrigger = value < 0 ? 0 : value;
        }
        public bool HasAnythingInsideTrigger => ObjectsInsideTrigger > 0;
        public bool DelayCompleted => delayCounter >= triggerDelay;
        public float NormalizedFill => triggerDelay == 0f ? 0f : delayCounter / triggerDelay;
#if !UNITY_2021_1_OR_NEWER
        public float SizeMultiplier => 1f;
        public float RotationAngle => -1f;
#endif
        
        #region Unity Messages
        private void Awake()
        {
            Assert.IsNotNull(collider);
        }

        private void Update()
        {
            TriggerDelayLoop();
        }

        private void OnValidate()
        {
            Assert.IsNotNull(collider);
        }

        private void Reset()
        {
            collider = GetComponent<Collider>();
        }
        
        private void OnDrawGizmos()
        {
            if (!showGizmos) return;

            Gizmos.color = gizmoColor;
            Gizmos.DrawCube(TriggerBounds.center, TriggerBounds.size);
        }
        #endregion

        #region Trigger Callbacks
        private void OnTriggerEnter(Collider other)
        {
            if (triggerOncePerSession && triggeredThisSession) return;
            if (string.IsNullOrEmpty(checkVsTag) == false && other.CompareTag(checkVsTag) == false) return;

            ObjectsInsideTrigger++;
            
            onEnterNoDelay?.Invoke();
            if (!useDelay)
            {
                OnEnter?.Invoke(other);
                onEnter?.Invoke();
                triggeredThisSession = true;
            }
        }
        
        private void OnTriggerStay(Collider other)
        {
            if (string.IsNullOrEmpty(checkVsTag) == false && other.CompareTag(checkVsTag) == false) return;

            if (!useDelay)
            {
                OnStay?.Invoke(other);
                onStay?.Invoke();
            }
            else if (DelayCompleted)
            {
                if (triggeredDelayedEnter == false)
                {
                    OnEnter?.Invoke(other);
                    onEnter?.Invoke();
                    triggeredThisSession = true;
                    triggeredDelayedEnter = true;
                }
                OnStay?.Invoke(other);
                onStay?.Invoke();
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (string.IsNullOrEmpty(checkVsTag) == false && other.CompareTag(checkVsTag) == false) return;

            ObjectsInsideTrigger--;
            triggeredDelayedEnter = false;
            
            OnExit?.Invoke(other);
            onExit?.Invoke();
        }
        #endregion

        #region Trigger Delay
        private void TriggerDelayLoop()
        {
            if (useDelay == false)
            {
                delayCounter = 0f;
                return;
            }

            if (HasAnythingInsideTrigger)
            {
                delayCounter += Time.deltaTime;
            }
            else
                delayCounter -= Time.deltaTime * delayDrainSpeed;

            delayCounter = Mathf.Clamp(delayCounter, 0f, triggerDelay);
        }
        
        public void ResetDelay() => delayCounter = 0f;
        #endregion
        
        #region Public Methods
        public void ResetObjectsOnTrigger() => ObjectsInsideTrigger = 0;
        
        public void EnableCollision() => TriggerEnabled = true;
        public void DisableCollision() => TriggerEnabled = false;
        #endregion
    }
}
