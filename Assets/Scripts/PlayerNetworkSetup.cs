using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerNetworkSetup : NetworkBehaviour {

    [SerializeField] Camera playerCam;
    [SerializeField] AudioListener playerAudio;

    private static PlayerNetworkSetup instance;
    public static PlayerNetworkSetup Instance { get { return instance; } }

    public Material hatBlue;
    public Material hatRed;
    public Material hatGreen;
    public Material hatBlack;

    public TextMesh textPlayerName;
    public GameObject goPlayerName;

    [SyncVar]
    public float timerGameOver = 600f;

    public bool loadMultiPlayer = false;

    public GameObject ScoresPanel;

    [SyncVar]
    public bool gameStart = false;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (isLocalPlayer)
            instance = this;
      
        //GAP
        if(isLocalPlayer)
            foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
                if(obj.GetComponent<PlayerNetworkSetup>().gameStart)
                    CmdDeletePlayer(gameObject);
    }

    private void Awake()
    {
        if (isLocalPlayer)
            instance = this;
    }

    private void FixedUpdate()
    {
        if (loadMultiPlayer && isServer && timerGameOver >= 0)
            timerGameOver -= Time.deltaTime;

        if (timerGameOver < 0 && isServer)
            NetworkManager.singleton.ServerChangeScene("Multi_GameOver");
    }

    [Command]
    public void CmdGameStart(string playerName)
    {
        if (playerName.Equals(NetworkManagerHUD.Instance.playerName))
        {
            if (!gameStart)
                CmdAddPlayer(NetworkManagerHUD.Instance.playerName);
        }

        RpcGameStart(playerName, gameStart);
    }

    [ClientRpc]
    public void RpcGameStart(string playerName, bool isReady)
    {
        if (playerName.Equals(NetworkManagerHUD.Instance.playerName))
        {
            //GAP
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("PlayerLobby"))
            {
                if (obj.transform.Find("PlayerName").GetComponent<InputField>().text.Equals(playerName))
                    return;
            }

            if (!isReady)
                CmdAddPlayer(NetworkManagerHUD.Instance.playerName);
        }
    }

    [Command]
    public void CmdSendText(string playerName, string txt)
    {
        LobbyController.Instance.CmdAddText(playerName, txt);
    }

    [Command]
    public void CmdAddPlayer(string playerName)
    {      
        LobbyController.Instance.CmdAddPlayer(playerName);
    }

    [Command]
    public void CmdReadyGame(string playerName)
    {
        int i = 0;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("PlayerLobby"))
        {
            Color color = go.GetComponent<LobbyMenu>().colorButton.GetComponent<Image>().color;
            GameObject.FindGameObjectsWithTag("Player")[i].transform.Find("Sphere").GetComponent<Renderer>().material = LobbyController.Instance.getMaterialByColor(color);
            i++;
        }
        RpcSetHatColor();
        LobbyController.Instance.CmdSetReadyGame(playerName);
        LobbyController.Instance.CmdReadyGame();
    }

    [ClientRpc]
    public void RpcSetHatColor()
    {
        int i = 0;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("PlayerLobby"))
        {
            Color color = go.GetComponent<LobbyMenu>().colorButton.GetComponent<Image>().color;
            GameObject.FindGameObjectsWithTag("Player")[i].transform.Find("Sphere").GetComponent<Renderer>().material = LobbyController.Instance.getMaterialByColor(color);
            i++;
        }
    }

    [Command]
    public void CmdChangeColor(string playerName)
    {
        LobbyController.Instance.CmdChangeColor(playerName);
    }

    private void OnLevelWasLoaded(int level)
    {
        if (!SceneManager.GetActiveScene().name.Equals("LobbyMatch") && isLocalPlayer)
        {
            if (GameObject.Find("Scene Camera"))
                GameObject.Find("Scene Camera").SetActive(false);

            GetComponent<CharacterController>().enabled = true;
            GetComponent<PlayerController>().enabled = true;
            GetComponent<MultiGameController>().enabled = true;
            playerCam.enabled = true;
            playerAudio.enabled = true;
        }

        if (!SceneManager.GetActiveScene().name.Equals("LobbyMatch"))
            GetComponent<PlayerSync>().enabled = true;

        if (SceneManager.GetActiveScene().name.Equals("LobbyMatch"))
        {
            playerCam.enabled = false;
            playerAudio.enabled = false;
            gameStart = false;
            if(MultiGameController.Instance != null)
            {
                MultiGameController.currentStage = 1;
                MultiGameController.Instance.score = 0;
                MultiGameController.Instance.gameOver = false;
            }
                
            GetComponent<PlayerSync>().enabled = false;
            GetComponent<PlayerSync>().Reset();           
            GetComponent<PlayerSync>().ready = false;

            loadMultiPlayer = false;

            if (isServer)
                NetworkManagerHUD.Instance.playerList.Clear();

            NetworkServer.SpawnObjects();

            timerGameOver = 600f;

            //if(isLocalPlayer)
            //CmdGameStart(NetworkManagerHUD.Instance.playerName);
        }

        if (SceneManager.GetActiveScene().name.Equals("Multi_fase_1"))
            loadMultiPlayer = true;

        if (SceneManager.GetActiveScene().name.Equals("Multi_GameOver"))
            loadMultiPlayer = false;
    }

    [Command]
    public void CmdDeletePlayer(GameObject player)
    {
        RpcDeletePlayer(player);
        Destroy(player);      
    }

    [ClientRpc]
    public void RpcDeletePlayer(GameObject player)
    {
        Destroy(player);
    }

    [Command]
    public void CmdBridgeStage(int option)
    {
        Bridge.Instance.CmdBridgeStage(option);
    }

    [Command]
    public void CmdButton(bool trigger, int idButton, GameObject gameObject)
    {
        //ButtonSwitch.Instance.CmdButton(trigger, idButton);
        gameObject.GetComponent<ButtonSwitch>().CmdButton(trigger, idButton);
    }

    [Command]
    public void CmdFire(GameObject obj)
    {
        //EnemyMultiplayer.Instance.CmdFire();
        obj.GetComponent<EnemyMultiplayer>().CmdFire();
    }

    [Command]
    public void CmdDestroy(GameObject obj)
    {
        Destroy(obj);
        RpcDestroy(obj);
    }

    [ClientRpc]
    public void RpcDestroy(GameObject obj)
    {
        Destroy(obj);
    }

    [Command]
    public void CmdAddScorePanel()
    {
        string fmt = "00000";
        foreach (Player p in NetworkManagerHUD.Instance.playerList)
        {
            GameObject childObject = Instantiate(ScoresPanel) as GameObject;
            childObject.transform.Find("Score").GetComponent<Text>().text = p.score.ToString(fmt);
            childObject.transform.Find("Level").GetComponent<Text>().text = MultiGameController.currentStage.ToString();
            childObject.transform.Find("Name").GetComponent<Text>().text = p.playerName;

            NetworkServer.Spawn(childObject);
        }
    }

    [ClientRpc]
    public void RpcAddScorePanel(Player p)
    {
        string fmt = "00000";
        GameObject.FindGameObjectsWithTag("ScoresPanel")[p.index].transform.Find("Score").GetComponent<Text>().text = p.score.ToString(fmt);
        GameObject.FindGameObjectsWithTag("ScoresPanel")[p.index].transform.Find("Level").GetComponent<Text>().text = MultiGameController.currentStage.ToString();
        GameObject.FindGameObjectsWithTag("ScoresPanel")[p.index].transform.Find("Name").GetComponent<Text>().text = p.playerName;
    }

    [Command]
    public void CmdRefreshScorePanel()
    {
        foreach (Player p in NetworkManagerHUD.Instance.playerList)
            RpcAddScorePanel(p);
    }

    [Command]
    public void CmdOpenDoor()
    {
        OpenDoor();
        RpcOpenDoor();
    }

    [ClientRpc]
    public void RpcOpenDoor()
    {
        OpenDoor();
    }

    public void OpenDoor()
    {
        MultiGameController.Instance.portal.SetActive(true);
        MultiGameController.Instance.openDoor = true;
    }

    private void OnGUI()
    {
        if(loadMultiPlayer && isLocalPlayer)
            GUI.Label(new Rect(Screen.width - 100, 15, 100, 100), ((int)timerGameOver).ToString());
    }
}
