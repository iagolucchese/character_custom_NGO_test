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
        private const float MinMoveDelta = 0.00001f;
        
        [Header("Movement Parameters")]
        [SerializeField, Min(0f)] private float moveSpeed = 4f;
        [Header("References")]
        [SerializeField, Required] private InputActionReference movementAction;
        [SerializeField, Required] private Rigidbody rigidbodyRef;
        [Header("Debug")]
        [SerializeField, ReadOnly] private Vector2 moveInput;
        [SerializeField, ReadOnly] private float inputMagnitude;
        [SerializeField, ReadOnly] private int movementLocks;
        [SerializeField, ReadOnly] private Vector3 lastMoveDirection;
        [SerializeField, ReadOnly] private float lastMoveMagnitude;
        [SerializeField, ReadOnly] private Vector3 lastPosition;

        public int MovementLocks
        {
            get => movementLocks;
            set => movementLocks = value < 0 ? 0 : value;
        }
        public bool IsMovementLocked => MovementLocks > 0;
        public Vector2 MoveInput => moveInput;
        public Vector3 LastMoveDirection => lastMoveDirection;
        public bool IsMoving => !IsMovementLocked && lastMoveMagnitude > MinMoveDelta;

        #region Unity Messages
        private void Awake()
        {
            Assert.IsNotNull(movementAction);
            Assert.IsNotNull(movementAction.action);
            Assert.IsNotNull(rigidbodyRef);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (!IsOwner) return;
            
            movementAction.action.Enable();
            //moveInputNetwork.Value = Vector2.zero;
            lastMoveDirection = Vector3.zero;
            lastMoveMagnitude = inputMagnitude = 0f;
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
            if (IsOwner)
                PlayerMovementLoop();
            CalculateMovementDelta();
        }

        private void Reset()
        {
            rigidbodyRef = GetComponentInChildren<Rigidbody>();
        }
        #endregion

        #region Private Methods
        private void AddMovementLock() => MovementLocks++;
        private void RemoveMovementLock() => MovementLocks--;
        
        private void PlayerMovementLoop()
        {
            moveInput = movementAction.action.ReadValue<Vector2>();
            //moveInputNetwork.Value = movementAction.action.ReadValue<Vector2>();
            inputMagnitude = moveInput.magnitude;
            if (inputMagnitude <= 0f) return;
            if (IsMovementLocked) return;

            Vector3 moveVector = moveInput.ToVector3XZ();
            Vector3 movementTargetPosition = rigidbodyRef.position + (moveVector * (moveSpeed * Time.fixedDeltaTime));
            rigidbodyRef.MovePosition(movementTargetPosition);
        }

        private void CalculateMovementDelta()
        {
            Vector3 currentPosition = rigidbodyRef.position;
            Vector3 moveDelta = currentPosition - lastPosition;
            lastMoveMagnitude = moveDelta.magnitude;
            if (lastMoveMagnitude > Mathf.Epsilon)
                lastMoveDirection = moveDelta.normalized;
            
            lastPosition = currentPosition;
        }
        #endregion
    }
}