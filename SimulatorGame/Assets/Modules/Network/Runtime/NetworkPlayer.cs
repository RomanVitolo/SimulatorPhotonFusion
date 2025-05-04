using Fusion;
using UnityEngine;

namespace Network
{
    [RequireComponent(typeof(NetworkMecanimAnimator))]
    public class NetworkPlayer : NetworkBehaviour
    {
        private NetworkMecanimAnimator netAnimator;
        private Animator animator;

        public override void Spawned()
        {
            netAnimator = GetComponent<NetworkMecanimAnimator>();
            animator = GetComponent<Animator>();

            if (netAnimator.Animator == null)
            {
                netAnimator.Animator = animator;
            }
        }
    }
}
