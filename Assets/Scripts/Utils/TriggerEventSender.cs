using UnityEngine;
using UnityEngine.Events;

namespace CharacterCustomNGO.Utils
{
    [RequireComponent(typeof(Collider))]
    public class TriggerEventSender : MonoBehaviour
    {
        public UnityEvent onEnter;
        public UnityEvent onStay;
        public UnityEvent onExit;

        private void OnTriggerEnter(Collider other)
        {
            onEnter?.Invoke();
        }

        private void OnTriggerStay(Collider other)
        {
            onStay?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            onExit?.Invoke();
        }
    }
}