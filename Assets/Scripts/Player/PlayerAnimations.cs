using ImportedScripts;
using UnityEngine;
using UnityEngine.Assertions;

namespace CharacterCustomNGO
{
    public class PlayerAnimations : MonoBehaviour
    {
        [Header("Inspector References")]
        [SerializeField] private InputMovement3D inputMovement;
        [SerializeField] private float rotateSpeed = 9f;
        //[SerializeField] private List<Animator> outfitAnimators;
        /*[Header("Animator Parameters")]
        [SerializeField] private Animator mainAnimator;
        [SerializeField, AnimatorParam("mainAnimator")] private int horizontalParam;
        [SerializeField, AnimatorParam("mainAnimator")] private int verticalParam;
        [SerializeField, AnimatorParam("mainAnimator")] private int walkParam;*/
        //private Vector2 receivedInput;

        private Transform RotateTransform => transform;
        
        #region Unity Messages
        private void Awake()
        {
            //Assert.IsNotNull(outfitAnimators);
            Assert.IsNotNull(inputMovement);
        }

        private void Update()
        {
            //bool walking = inputMovement.IsMovementLocked == false && inputMovement.MovementMagnitude > 0f;
            /*if (inputMovement.IsMoving)
                receivedInput = inputMovement.MoveInput;*/

            RotateModel();
            UpdateAnimatorParameters();
        }
        #endregion
        
        #region Private Methods
        private void RotateModel()
        {
            if (inputMovement.IsMoving == false) return;
            
            Vector3 rotateDirection = inputMovement.LastMoveDirection;
            rotateDirection.y = 0f;
            rotateDirection.Normalize();
            Quaternion targetRotation = rotateDirection.sqrMagnitude <= 0 
                ? Quaternion.identity
                : Quaternion.LookRotation(rotateDirection, Vector3.up);
            RotateTransform.rotation = Quaternion.Slerp(
                RotateTransform.rotation,
                targetRotation,
                Time.deltaTime * rotateSpeed);
        }
        
        private void UpdateAnimatorParameters()
        {
            Vector2 receivedInput = new(inputMovement.LastMoveDirection.x, inputMovement.LastMoveDirection.z);

            /*mainAnimator.SafeSetParameter(walkParam, walking);
            mainAnimator.SafeSetParameter(horizontalParam, receivedInput.x);
            mainAnimator.SafeSetParameter(verticalParam, receivedInput.y);*/

            /*outfitAnimators.ForEach(animator => animator.SafeSetParameter(walkParam, walking));
            outfitAnimators.ForEach(animator => animator.SafeSetParameter(horizontalParam, receivedInput.x));
            outfitAnimators.ForEach(animator => animator.SafeSetParameter(verticalParam, receivedInput.y));*/
        }
        #endregion
    }
}