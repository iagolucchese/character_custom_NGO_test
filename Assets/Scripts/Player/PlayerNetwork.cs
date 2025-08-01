using Unity.Netcode;

namespace CharacterCustomNGO
{
    public class PlayerNetwork : NetworkBehaviour
    {
        public delegate void PlayerNetworkEvent(NetworkObject playerNO);
        public static event PlayerNetworkEvent OnPlayerSpawned;

        protected override void OnNetworkPostSpawn()
        {
            base.OnNetworkPostSpawn();
            OnPlayerSpawned?.Invoke(NetworkObject);
        }
    }
}