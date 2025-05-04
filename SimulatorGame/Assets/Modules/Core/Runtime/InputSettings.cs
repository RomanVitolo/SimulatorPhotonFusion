using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "InputSettings", menuName = "Settings/Input Settings")]
    public class InputSettings : ScriptableObject
    {
        [Header("Movement Settings")]
        public float walkSpeed = 4f;
        public float sprintSpeed = 8f;
        public float jumpForce = 2.5f;
        public float gravity = -30f;

        [Header("Mouse Settings")]
        public float mouseSensitivity = 7f;
        public float verticalClamp = 80f;
    }
}
