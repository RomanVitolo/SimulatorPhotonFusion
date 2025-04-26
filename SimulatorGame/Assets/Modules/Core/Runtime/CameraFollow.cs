using Unity.Cinemachine;
using UnityEngine;

namespace Core
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera cineCam;

        public void SetTarget(Transform target)
        {
            if (cineCam != null)
            {
                cineCam.Follow = target;
                cineCam.LookAt = target;

                cineCam.transform.SetPositionAndRotation(target.position, target.rotation);
            }
            else
            {
                Debug.LogWarning("CinemachineCamera no asignada.");
            }
        }
    }
}
