using Unity.Netcode;

public class NetworkStartHostButton : BaseNetworkStartButton
{
    void Start()
    {
        _text.SetText("Start Host");
    }

    protected override void HandleOnButtonClicked()
    {
        NetworkManager.Singleton.StartHost();
    }
}