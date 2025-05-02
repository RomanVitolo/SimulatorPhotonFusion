using Fusion;
using UnityEngine;

namespace Player
{
    public class FirstPersonLook : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform cameraHolder;
        [SerializeField] private Core.InputSettings inputSettings;

        private float verticalLookRotation = 0f;

        public override void Spawned()
        {
            if (!Object.HasInputAuthority)
            {
                cameraHolder.gameObject.SetActive(false);
                enabled = false;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (!Object.HasInputAuthority) return;

            HandleLook();
        }

        private void HandleLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * inputSettings.mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * inputSettings.mouseSensitivity;

            transform.rotation *= Quaternion.Euler(0f, mouseX, 0f);

            verticalLookRotation -= mouseY;
            verticalLookRotation = Mathf.Clamp(
                verticalLookRotation,
                -inputSettings.verticalClamp,
                inputSettings.verticalClamp
            );

            cameraHolder.localEulerAngles = new Vector3(verticalLookRotation, 0f, 0f);
        }
    }
}
