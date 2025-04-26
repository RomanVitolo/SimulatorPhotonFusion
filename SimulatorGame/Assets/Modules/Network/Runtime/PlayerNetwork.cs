using Fusion;
using UnityEngine;
using PlayerAPI;
using InputSystem;
using Fusion.Sockets;

namespace Network
{
    [RequireComponent(typeof(NetworkObject))]
    public class PlayerNetwork : NetworkBehaviour
    {
        [SerializeField] private Transform fpsView;
        [SerializeField] private Animator animator;
        private PlayerController playerController;
        private IPlayerInput playerInput;

        [Networked] public float NetSpeed { get; set; }
        [Networked] public float NetMotionSpeed { get; set; }
        [Networked] public bool NetJump { get; set; }
        [Networked] public bool NetFreeFall { get; set; }
        [Networked] public bool NetGrounded { get; set; }

        public override void Spawned()
        {
            playerController = GetComponent<PlayerController>();

            if (Object.HasInputAuthority)
            {
                if (fpsView != null) fpsView.gameObject.SetActive(true);

                var fpsController = GetComponent<Core.FirstPersonCameraController>();
                fpsController.SetCameraTarget(fpsView);

                playerInput = new LocalPlayerInput();
            }
            else 
            {
                if (fpsView != null) fpsView.gameObject.SetActive(false);
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (Object.HasInputAuthority)
            {
                Vector2 move = playerInput.GetMovement();
                playerController.SetInput(move);

                if (playerInput.JumpPressed())
                    playerController.Jump();

                bool isSprinting = playerInput.SprintHeld();
                playerController.SetSprinting(isSprinting);

                playerController.ManualUpdate();

                // → Actualizamos las variables de red con los valores actuales
                NetSpeed = playerController.CurrentSpeed;
                //NetMotionSpeed = playerController.CurrentMotionSpeed;
                NetGrounded = playerController.IsGrounded;
                NetJump = playerController.IsJumping;
                //NetFreeFall = playerController.IsFreeFalling;
            }
            else
            {
                // → Aplicamos esas variables al Animator
                if (animator != null)
                {
                    if (Mathf.Abs(animator.GetFloat("Speed") - NetSpeed) > 0.01f)
                        animator.SetFloat("Speed", NetSpeed);
                    animator.SetFloat("MotionSpeed", NetMotionSpeed);
                    animator.SetBool("Grounded", NetGrounded);
                    animator.SetBool("Jump", NetJump);
                    animator.SetBool("FreeFall", NetFreeFall);
                }
            }
        }
    }
}
