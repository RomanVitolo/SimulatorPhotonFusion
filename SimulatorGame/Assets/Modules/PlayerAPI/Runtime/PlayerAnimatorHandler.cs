using Fusion;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerAnimatorHandler : NetworkBehaviour
    {
        [SerializeField] private Transform modelRoot;

        private Animator animator;
        private PlayerController playerController;

        private float animationBlend;
        private const float blendSpeed = 10f;

        private void Awake()
        {
            animator = modelRoot.GetComponent<Animator>();
            playerController = GetComponent<PlayerController>();

            if (animator == null)
                Debug.LogError("Animator not found on Model.");
        }

        public override void Spawned() => enabled = Object.HasInputAuthority;

        public override void FixedUpdateNetwork() => UpdateAnimationStates();

        private void UpdateAnimationStates()
        {
            if (animator == null || playerController == null) return;

            Vector3 horizontalVelocity = new(playerController.MoveDirection.x, 0f, playerController.MoveDirection.z);
            float targetSpeed = horizontalVelocity.magnitude;

            animationBlend = Mathf.Lerp(animationBlend, targetSpeed, blendSpeed * Runner.DeltaTime);

            bool isGrounded = playerController.IsGrounded;
            bool isJumping = !isGrounded && playerController.MoveDirection.y > 0.1f;
            bool isFreeFalling = !isGrounded && playerController.MoveDirection.y < -0.1f;

            animator.SetFloat("Speed", animationBlend);
            animator.SetFloat("MotionSpeed", 1f);
            animator.SetBool("Grounded", isGrounded);
            animator.SetBool("Jump", isJumping);
            animator.SetBool("FreeFall", isFreeFalling);
        }
    }
}