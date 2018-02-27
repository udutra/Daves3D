using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyMenu : NetworkBehaviour
{
    public GameObject readyButton;
    public GameObject colorButton;
    public GridLayoutGroup Grid;

    private void Start()
    {
        if (GameObject.FindGameObjectWithTag("LobbyPanel"))
        {
            Grid = GameObject.FindGameObjectWithTag("LobbyPanel").GetComponent<GridLayoutGroup>();
            gameObject.transform.SetParent(Grid.transform);
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            gameObject.transform.localPosition = Vector3.zero;
        }    
    }

    public void BackLobbyMenu()
    {
        NetworkManagerHUD.Instance.CustomStopServer();
    }

    public void ReadyGame()
    {
        if (gameObject.transform.Find("PlayerName").transform.FindChild("Text").GetComponent<Text>().text.Equals(NetworkManagerHUD.Instance.playerName))
            PlayerNetworkSetup.Instance.CmdReadyGame(NetworkManagerHUD.Instance.playerName);
    }

    public void ChangeColor()
    {
        if (gameObject.transform.Find("PlayerName").transform.FindChild("Text").GetComponent<Text>().text.Equals(NetworkManagerHUD.Instance.playerName))
            PlayerNetworkSetup.Instance.CmdChangeColor(NetworkManagerHUD.Instance.playerName);
    }

}
