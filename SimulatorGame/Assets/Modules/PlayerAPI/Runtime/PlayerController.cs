using UnityEngine;

namespace PlayerAPI
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator animator;

        private CharacterController controller;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float sprintSpeed = 6f;
        [SerializeField] private float speedChangeRate = 10f;
        private float currentSpeed;
        private Vector2 inputMove;
        private bool isSprinting;

        [Header("Jump & Gravity")]
        [SerializeField] private float jumpHeight = 1.2f;
        [SerializeField] private float gravity = -30f;
        [SerializeField] private float fallMultiplier = 2.5f;
        [SerializeField] private float jumpTimeout = 0.2f;
        [SerializeField] private float fallTimeout = 0.1f;
        private float jumpTimeoutDelta;
        private float fallTimeoutDelta;
        private float verticalVelocity;
        private readonly float terminalVelocity = 53f;
        private bool jumping;

        [Header("Ground Check")]
        [SerializeField] private LayerMask groundLayers;
        [SerializeField] private float groundOffset = -0.14f;
        [SerializeField] private float groundRadius = 0.25f;
        private bool grounded;

        // Animator IDs
        private int animIDSpeed;
        private int animIDMotionSpeed;
        private int animIDJump;
        private int animIDFreeFall;
        private int animIDGrounded;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();

            if (animator == null)
                animator = GetComponentInChildren<Animator>();

            AssignAnimationIDs();
        }

        private void AssignAnimationIDs()
        {
            animIDSpeed = Animator.StringToHash("Speed");
            animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            animIDJump = Animator.StringToHash("Jump");
            animIDFreeFall = Animator.StringToHash("FreeFall");
            animIDGrounded = Animator.StringToHash("Grounded");
        }

        public void ManualUpdate()
        {
            GroundedCheck();
            ApplyGravity();
            Move();
        }

        public void SetInput(Vector2 moveInput)
        {
            inputMove = moveInput;
        }

        public void SetSprinting(bool value)
        {
            isSprinting = value;
        }

        public void Jump()
        {
            jumping = true;
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + groundOffset, transform.position.z);
            grounded = Physics.CheckSphere(spherePosition, groundRadius, groundLayers, QueryTriggerInteraction.Ignore);
        }

        private void Move()
        {
            Vector3 inputDir = new Vector3(inputMove.x, 0f, inputMove.y).normalized;
            Vector3 moveDir = transform.TransformDirection(inputDir);

            float targetSpeed = inputMove == Vector2.zero ? 0f : (isSprinting ? sprintSpeed : moveSpeed);

            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, speedChangeRate * Time.deltaTime);

            Vector3 velocity = moveDir * currentSpeed + Vector3.up * verticalVelocity;
            controller.Move(velocity * Time.deltaTime);

            UpdateAnimation(inputMove.magnitude);
        }

        private void ApplyGravity()
        {
            if (grounded)
            {
                fallTimeoutDelta = fallTimeout;

                if (verticalVelocity < 0f)
                    verticalVelocity = -2f;

                if (jumping && jumpTimeoutDelta <= 0f)
                {
                    verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                    jumping = false;
                }

                if (jumpTimeoutDelta > 0f)
                    jumpTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                jumpTimeoutDelta = jumpTimeout;

                if (fallTimeoutDelta > 0f)
                {
                    fallTimeoutDelta -= Time.deltaTime;
                }

                float gravityScale = verticalVelocity < 0 ? fallMultiplier : 1f;
                verticalVelocity += gravity * gravityScale * Time.deltaTime;
                verticalVelocity = Mathf.Clamp(verticalVelocity, -terminalVelocity, float.MaxValue);
            }
        }

        private void UpdateAnimation(float inputMagnitude)
        {
            if (animator == null) return;

            animator.SetFloat(animIDSpeed, currentSpeed);
            animator.SetFloat(animIDMotionSpeed, Mathf.Clamp01(inputMagnitude));
            animator.SetBool(animIDGrounded, grounded);
            animator.SetBool(animIDJump, jumping);
            animator.SetBool(animIDFreeFall, !grounded && fallTimeoutDelta <= 0f);
        }

        // Para depuración del ground check
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = grounded ? new Color(0, 1, 0, 0.3f) : new Color(1, 0, 0, 0.3f);
            Vector3 spherePos = new Vector3(transform.position.x, transform.position.y + groundOffset, transform.position.z);
            Gizmos.DrawSphere(spherePos, groundRadius);
        }

        // Exponer estados (opcional)
        public bool IsGrounded => grounded;
        public float CurrentSpeed => currentSpeed;
        public bool IsJumping => jumping;
    }
}