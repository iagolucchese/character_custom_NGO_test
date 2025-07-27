using UnityEngine;
using UnityEngine.Events;

namespace CharacterCustomNGO.Utils
{
    [RequireComponent(typeof(Collider2D))]
    public class Trigger2DEventSender : MonoBehaviour
    {
        public UnityEvent onEnter;
        public UnityEvent onStay;
        public UnityEvent onExit;

        private void OnTriggerEnter2D(Collider2D other)
        {
            onEnter?.Invoke();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            onStay?.Invoke();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            onExit?.Invoke();
        }
    }
}