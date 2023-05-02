using Unity.Netcode;
using UnityEngine;

public class ConnectionApprovalHandler : MonoBehaviour
{
    [SerializeField] int _maxPlayer = 1;
    
    void Start()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
    }

    void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        Debug.Log("Connect Approval");
        response.Approved = true;
        response.CreatePlayerObject = true;
        response.PlayerPrefabHash = null;
        if (NetworkManager.Singleton.ConnectedClients.Count >= _maxPlayer)
        {
            response.Approved = false;
            Debug.Log("Server Full");
        }

        response.Pending = false;
    }
}
