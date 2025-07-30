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
        //[SerializeField, ReadOnly] private Vector2 moveInput;
        [SerializeField, ReadOnly] private float movementMagnitude;
        [SerializeField, ReadOnly] private int movementLocks;
        
        public NetworkVariable<Vector2> moveInputNetwork = new();

        public int MovementLocks
        {
            get => movementLocks;
            set => movementLocks = value < 0 ? 0 : value;
        }
        public bool IsMovementLocked => MovementLocks > 0;
        public Vector2 MoveInput => moveInputNetwork.Value;//moveInput;
        public float MovementMagnitude => movementMagnitude;

        #region Unity Messages
        private void Awake()
        {
            Assert.IsNotNull(movementAction);
            Assert.IsNotNull(movementAction.action);
            Assert.IsNotNull(rigidbody);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            movementAction.action.Enable();
            //moveInput = Vector2.zero;
            moveInputNetwork.Value = Vector2.zero;
            movementMagnitude = 0f;
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
            if (!IsOwner) return;
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
            //moveInput = movementAction.action.ReadValue<Vector2>();
            moveInputNetwork.Value = movementAction.action.ReadValue<Vector2>();
            movementMagnitude = moveInputNetwork.Value.magnitude;
            if (movementMagnitude <= 0f) return;
            if (IsMovementLocked) return;

            Vector3 moveVector = moveInputNetwork.Value.ToVector3XZ();
            Vector3 movementTargetPosition = rigidbody.position + (moveVector * (moveSpeed * Time.fixedDeltaTime));
            rigidbody.MovePosition(movementTargetPosition);
        }
        #endregion
    }
}