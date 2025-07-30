using System;
using CharacterCustomNGO.UI;
using ImportedScripts;
using NaughtyAttributes;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace CharacterCustomNGO
{
    public class InputMovement3D : NetworkBehaviour
    {
        [Header("Movement Parameters")]
        [SerializeField, Min(0f)] private float moveSpeed = 2f;
        [Header("References")]
        [SerializeField] private InputActionReference movementAction;
        [SerializeField] private new Rigidbody rigidbody;
        [Header("Debug")]
        [SerializeField, ReadOnly] private Vector2 moveInput;
        [SerializeField, ReadOnly] private float movementMagnitude;
        [SerializeField, ReadOnly] private int movementLocks;

        public int MovementLocks
        {
            get => movementLocks;
            set => movementLocks = value < 0 ? 0 : value;
        }
        public bool IsMovementLocked => MovementLocks > 0;
        public Vector2 MoveInput => moveInput;
        public float MovementMagnitude => movementMagnitude;

        #region Unity Messages
        private void Awake()
        {
            Assert.IsNotNull(movementAction);
            Assert.IsNotNull(movementAction.action);
            Assert.IsNotNull(rigidbody);
            movementAction.action.Enable();
            moveInput = Vector2.zero;
            movementMagnitude = 0f;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            MovementLocks = 0;
            ScreenManagerBase.OnScreenOpened += AddMovementLock;
            ScreenManagerBase.OnScreenClosed += RemoveMovementLock;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            ScreenManagerBase.OnScreenOpened -= AddMovementLock;
            ScreenManagerBase.OnScreenClosed -= RemoveMovementLock;
        }

        private void FixedUpdate()
        {
            PlayerMovementLoop();
        }

        private void Reset()
        {
            rigidbody = GetComponentInChildren<Rigidbody>();
        }
        #endregion

        #region Private Methods
        private void AddMovementLock() => MovementLocks++;
        private void RemoveMovementLock() => MovementLocks--;
        
        private void PlayerMovementLoop()
        {
            moveInput = movementAction.action.ReadValue<Vector2>();
            movementMagnitude = moveInput.magnitude;
            if (movementMagnitude <= 0f) return;
            if (IsMovementLocked) return;

            Vector3 moveVector = moveInput.ToVector3XZ();
            Vector2 movementTargetPosition = rigidbody.position + (moveVector * (moveSpeed * Time.fixedDeltaTime));
            rigidbody.MovePosition(movementTargetPosition);
        }
        #endregion
    }
}