using Fusion;
using UnityEngine;

namespace Network
{
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkAuthorityHandler : NetworkBehaviour
    {
        [Header("Components to Enable If Has Input Authority")]
        [SerializeField] private UnityEngine.Behaviour[] componentsToEnable;

        [Header("GameObjects to Enable If Has Input Authority (Optional)")]
        [SerializeField] private GameObject[] objectsToEnable;

        public override void Spawned()
        {
            bool hasAuthority = HasInputAuthority;

            foreach (var comp in componentsToEnable)
            {
                if (comp != null)
                    comp.enabled = hasAuthority;
            }

            foreach (var go in objectsToEnable)
            {
                if (go != null)
                    go.SetActive(hasAuthority);
            }
        }
    }
}
