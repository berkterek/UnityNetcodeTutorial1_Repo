using System.Collections;
using Unity.BossRoom.Infrastructure;
using Unity.Netcode;
using UnityEngine;

public class FoodSpawnerController : MonoBehaviour
{
    [SerializeField] GameObject _prefab;

    const int MAX_PREFAB_COUNT = 50;

    void Start()
    {
        NetworkManager.Singleton.OnServerStarted += HandleOnServerStared;
    }

    void HandleOnServerStared()
    {
        //avoid for resource spend
        NetworkManager.Singleton.OnServerStarted -= HandleOnServerStared;
        NetworkObjectPool.Singleton.InitializePool();

        for (int i = 0; i < 30; i++)
        {
            SpawnFood();
        }

        StartCoroutine(SpawnOverTimeAsync());
    }

    void SpawnFood()
    {
        NetworkObject networkObject =
            NetworkObjectPool.Singleton.GetNetworkObject(_prefab, GetRandomMapPosition(), Quaternion.identity);
        
        networkObject.GetComponent<FoodController>().Prefab = _prefab;

        if (!networkObject.IsSpawned)
        {
            //Spawn means => When scene changed object with destroy
            networkObject.Spawn(true);
        }
    }

    Vector3 GetRandomMapPosition()
    {
        return new Vector3(Random.Range(-17f, 17f), Random.Range(-9f, 9f), 0f);
    }

    IEnumerator SpawnOverTimeAsync()
    {
        var wait = new WaitForSeconds(2f);
        while (NetworkManager.Singleton.ConnectedClients.Count > 0)
        {
            yield return wait;
            if (NetworkObjectPool.Singleton.GetCurrentPrefabCount(_prefab) < MAX_PREFAB_COUNT)
            {
                SpawnFood();    
            }
        }
    }
}