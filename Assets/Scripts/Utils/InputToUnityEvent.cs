using System;
using CharacterCustomNGO.UI;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace CharacterCustomNGO.Utils
{
    public class InputToUnityEvent : MonoBehaviour
    {
        [SerializeField] private InputActionReference inputAction;
        [SerializeField] private bool blockedWhenScreenIsOpen;
        [SerializeField] private UnityEvent onAction;
        [ShowNonSerializedField] private bool isBlocked;
        
        private void OnEnable()
        {
            inputAction.action.Enable();
            inputAction.action.started += StartedCallback;

            ScreenManagerBase.OnScreenOpened += BlockAction;
            ScreenManagerBase.OnScreenClosed += UnlockAction;
        }

        private void OnDisable()
        {
            inputAction.action.started -= StartedCallback;
            ScreenManagerBase.OnScreenOpened -= BlockAction;
            ScreenManagerBase.OnScreenClosed -= UnlockAction;
        }

        private void StartedCallback(InputAction.CallbackContext callbackContext)
        {
            if (gameObject.activeInHierarchy == false) return;
            if (blockedWhenScreenIsOpen && isBlocked) return;
            onAction?.Invoke();
        }

        private void BlockAction() => isBlocked = true;
        private void UnlockAction() => isBlocked = false;
    }
}