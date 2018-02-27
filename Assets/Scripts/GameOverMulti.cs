using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameOverMulti : NetworkBehaviour
{
    [SyncVar]
    public float timer = 10f;

    public Text text;

    private void Start()
    {
        if (isServer)
            PlayerNetworkSetup.Instance.CmdAddScorePanel();
    }

    private void Update()
    {
        if (isServer && timer >= 0)
            timer -= Time.deltaTime;

        text.text = ((int)timer).ToString();

        if (timer < 0 && isServer)
            NetworkManager.singleton.ServerChangeScene("LobbyMatch");
    }

}
