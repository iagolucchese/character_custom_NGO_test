using ImportedScripts;
using UnityEngine;
using UnityEngine.Assertions;

namespace CharacterCustomNGO
{
    public class PlayerAnimations : MonoBehaviour
    {
        [Header("Inspector References")]
        [SerializeField] private InputMovement3D inputMovement;
        [SerializeField] private float rotateSpeed = 1f;
        //[SerializeField] private List<Animator> outfitAnimators;
        /*[Header("Animator Parameters")]
        [SerializeField] private Animator mainAnimator;
        [SerializeField, AnimatorParam("mainAnimator")] private int horizontalParam;
        [SerializeField, AnimatorParam("mainAnimator")] private int verticalParam;
        [SerializeField, AnimatorParam("mainAnimator")] private int walkParam;*/
        private Vector2 receivedInput;

        private Transform RotateTransform => transform;
        
        #region Unity Messages
        private void Awake()
        {
            //Assert.IsNotNull(outfitAnimators);
            Assert.IsNotNull(inputMovement);
        }

        private void Update()
        {
            UpdateAnimatorParameters();
        }
        #endregion
        
        #region Private Methods
        private void UpdateAnimatorParameters()
        {
            bool walking = inputMovement.IsMovementLocked == false && inputMovement.MovementMagnitude > 0f;
            if (walking)
                receivedInput = inputMovement.MoveInput;

            var rotateDirection = receivedInput.ToVector3XZ().normalized;
            var targetRotation = Quaternion.LookRotation(rotateDirection, Vector3.up);
            RotateTransform.rotation = Quaternion.Slerp(
                RotateTransform.rotation,
                targetRotation,
                Time.deltaTime * rotateSpeed);

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