using Fusion;
using UnityEngine;

namespace Network
{
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkPlayer : NetworkBehaviour
    {
        private Vector3 targetPosition;
        private Quaternion targetRotation;

        public override void Spawned()
        {
            Debug.Log($"[NetworkPlayer] Spawned with authority: {Object.InputAuthority}");
        }

        public override void FixedUpdateNetwork()
        {
            if (HasInputAuthority) return;

            ApplyMovement();
        }

        public void SyncMovement(Vector3 position) => targetPosition = position;

        public void SyncRotation(Quaternion rotation) => targetRotation = rotation;

        private void ApplyMovement()
        {
            transform.SetPositionAndRotation(
                Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f),
                Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f));
        }
    }
}
