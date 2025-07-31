using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace CharacterCustomNGO
{
    public class PlayerGetCameraToFollow : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            GetCameraToFollow();
        }

        private void GetCameraToFollow()
        {
            var mainCam = Camera.main;
            if (!mainCam.TryGetComponent<CinemachineBrain>(out var brain)) return;
            
            if (brain.ActiveVirtualCamera is CinemachineCamera vcam) 
                vcam.Target = new CameraTarget{TrackingTarget = transform, LookAtTarget = transform};
        }
    }
}