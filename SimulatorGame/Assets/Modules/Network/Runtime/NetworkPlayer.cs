using Fusion;
using UnityEngine;

namespace Network
{
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkPlayer : NetworkBehaviour
    {
        public override void Spawned()
        {
            Debug.Log($"[NetworkPlayer] Spawned with authority: {Object.InputAuthority}");
        }
    }
}
