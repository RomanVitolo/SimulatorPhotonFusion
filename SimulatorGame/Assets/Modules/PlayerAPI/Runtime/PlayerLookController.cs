using Fusion;
using Network;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(NetworkTransform))]
    public class PlayerLookController : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Core.InputSettings inputSettings;

        private NetworkPlayer networkPlayer;

        private float verticalRotation = 0f;

        private void Awake() => networkPlayer = GetComponent<NetworkPlayer>();

        public override void FixedUpdateNetwork() => HandleLook();

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

            SyncRotation();
        }

        private void SyncRotation() => networkPlayer?.SyncRotation(transform.rotation);
    }
}
