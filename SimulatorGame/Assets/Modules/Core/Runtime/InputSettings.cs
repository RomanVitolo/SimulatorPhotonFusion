using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "InputSettings", menuName = "Settings/Input Settings")]
    public class InputSettings : ScriptableObject
    {
        [Header("Movement Settings")]
        public float walkSpeed = 5f;
        public float sprintSpeed = 8f;
        public float jumpForce = 5f;
        public float gravity = -9.81f;

        [Header("Mouse Settings")]
        public float mouseSensitivity = 2f;
        public float verticalClamp = 80f;
    }
}
