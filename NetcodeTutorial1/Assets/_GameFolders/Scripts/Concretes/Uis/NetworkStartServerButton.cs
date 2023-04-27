using Unity.Netcode;

public class NetworkStartServerButton : BaseNetworkStartButton
{
    void Start()
    {
        _text.SetText("Start Server");
    }

    protected override void HandleOnButtonClicked()
    {
        NetworkManager.Singleton.StartServer();
        base.HandleOnButtonClicked();
    }
}