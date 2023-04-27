using Unity.BossRoom.Infrastructure;
using Unity.Netcode;
using UnityEngine;

public class FoodController : NetworkBehaviour
{
    public GameObject Prefab { get; set; }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        //this means this code only work on server prepared to clients cheating
        if (!NetworkManager.Singleton.IsServer) return;

        if (other.TryGetComponent(out PlayerLengthHandler playerLengthHandler))
        {
            playerLengthHandler.AddLength();
        }
        else if (other.TryGetComponent(out TailController tailController))
        {
            tailController.NetworkOwner.GetComponent<PlayerLengthHandler>().AddLength();
        }

        NetworkObjectPool.Singleton.ReturnNetworkObject(NetworkObject, Prefab);
        NetworkObject.Despawn(false);
    }
}