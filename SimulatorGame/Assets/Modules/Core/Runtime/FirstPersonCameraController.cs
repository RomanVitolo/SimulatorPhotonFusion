using UnityEngine;
using Fusion;

namespace Core
{
    public class FirstPersonCameraController : MonoBehaviour
    {
        [SerializeField] private Transform cameraTarget;
        [SerializeField] private float sensitivity = 1.5f;
        [SerializeField] private float pitchClamp = 80f;

        private float yaw;
        private float pitch;

        private NetworkObject networkObject;

        public void SetCameraTarget(Transform target)
        {
            cameraTarget = target;
        }

        private void Awake()
        {
            networkObject = GetComponent<NetworkObject>();
        }

        private void Update()
        {
            if (networkObject == null || !networkObject.HasInputAuthority)
                return;

            if (cameraTarget == null) return;

            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

            yaw += mouseX;
            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, -pitchClamp, pitchClamp);

            cameraTarget.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        }
    }
}


