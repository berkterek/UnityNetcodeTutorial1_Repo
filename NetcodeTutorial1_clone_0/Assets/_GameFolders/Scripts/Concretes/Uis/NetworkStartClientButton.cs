using Unity.Netcode;

public class NetworkStartClientButton : BaseNetworkStartButton
{
    void Start()
    {
        _text.SetText("Start Client");
    }

    protected override void HandleOnButtonClicked()
    {
        NetworkManager.Singleton.StartClient();
    }
}