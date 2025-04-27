using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerLookController : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Core.InputSettings inputSettings;

        private float verticalRotation = 0f;

        public override void Spawned()
        {
            if (!Object.HasInputAuthority)
            {
                enabled = false;
            }
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update() => HandleLook();

        private void HandleLook()
        {
            Vector2 mouseDelta = inputSettings.mouseSensitivity * Time.deltaTime * new Vector2(
                Mouse.current.delta.x.ReadValue(),
                Mouse.current.delta.y.ReadValue()
            );

            transform.Rotate(Vector3.up * mouseDelta.x);

            verticalRotation -= mouseDelta.y;
            verticalRotation = Mathf.Clamp(verticalRotation, -inputSettings.verticalClamp, inputSettings.verticalClamp);

            cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }
    }
}
