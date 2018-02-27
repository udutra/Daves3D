using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Linq;

public class MultiGameController : NetworkBehaviour
{
    public GameObject player;
    public GameObject door;
    public GameObject portal;
    public Material materialOpenDoor;

    public int Lifes = 3;
    public bool removeLife = false;
    public Texture2D davesLife;
    public Texture2D openDoorTexture;
    public Texture2D gunTexture;
    public Texture2D jackpackTexture;

    public bool openDoor = false;

    public bool gameOver = false;

    private string fmt = "00000";
    public int score = 0;
    public Font font;

    public static int currentStage = 1;

    public Texture2D jackPackBar_1;
    public Texture2D jackPackBar_2;

    public GameObject ScoresPanel;

    public AudioSource audioS;

    public AudioClip audioClipDie;

    private static MultiGameController instance = null;
    public static MultiGameController Instance { get { return instance; } }

    public bool isOptionPanel = false;

    void Start()
    {
        currentStage = 1;
        instance = this;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (GameObject.FindGameObjectWithTag("Door"))
        {
            door = GameObject.FindGameObjectWithTag("Door");
            portal = door.transform.GetChild(1).gameObject;
            portal.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            isOptionPanel = !isOptionPanel;
    }

    private void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetActiveScene().name.Equals("Multi_fase_1"))
        {
            gameOver = false;
            currentStage = 1;

        } else if(!SceneManager.GetActiveScene().name.Equals("Multi_GameOver"))
        {
            openDoor = false;
            currentStage++;
        }

        if (SceneManager.GetActiveScene().name.Equals("Menu"))
            Destroy(gameObject);

        if (GameObject.FindGameObjectWithTag("Door"))
        {
            door = GameObject.FindGameObjectWithTag("Door");
            portal = door.transform.GetChild(1).gameObject;
            portal.SetActive(false);
        }

        if (SceneManager.GetActiveScene().name.Equals("Multi_GameOver"))
        {
            gameOver = true;
            //gameObject.transform.Find("PlayerCamera").gameObject.SetActive(false);
            if(GameObject.Find("MainCamera"))
                GameObject.Find("MainCamera").SetActive(false);

            resetPlayer();
        }      
    }

    public void resetPlayer()
    {
        GetComponent<PlayerController>().hasJetPack = false;
        GetComponent<PlayerController>().hasPistol = false;
        GetComponent<PlayerController>().jetPackPrefab.SetActive(false);
        GetComponent<PlayerController>().pistolPrefab.SetActive(false);
        GetComponent<CharacterController>().enabled = false;
        GetComponent<PlayerController>().enabled = false;
        GetComponent<MultiGameController>().enabled = false;
        GetComponent<PlayerNetworkSetup>().timerGameOver = 600f;
    }

    [Command]
    public void CmdLoadGameOverScene()
    {
        NetworkManager.singleton.ServerChangeScene("Multi_GameOver");
    }

    void OnGUI()
    {
        GUI.skin.font = font;
        GUI.skin.label.fontSize = 40;

        GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));

        if (!gameOver)
        {
            GUI.Label(new Rect(15, 15, 300, 50), score.ToString(fmt));

            GUI.Label(new Rect((Screen.width / 2) - (150 / 2), 15, 300, 50), "Level : " + currentStage);

            if (player.GetComponent<PlayerController>().hasPistol)
                GUI.Label(new Rect(15, Screen.height - 50, 300, 50), gunTexture);

            if (openDoor)
                GUI.Label(new Rect(Screen.width - 271, Screen.height - 45, 400, 100), openDoorTexture);

            if (player.GetComponent<PlayerController>().hasJetPack)
            {
                GUI.Label(new Rect((Screen.width / 2) - (150 / 2), Screen.height - 50, 300, 50), jackpackTexture);
                GUI.DrawTexture(new Rect((Screen.width / 2) - (340 / 2), Screen.height - 20, 300 / 50 * player.GetComponent<PlayerController>().maxJetpack, 15), jackPackBar_2);
            }

            if (isOptionPanel)
            {
                GUI.Box(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 150, 150, 150), "Menu");

                if (GUI.Button(new Rect(Screen.width / 2 - 40, Screen.height / 2 - 120, 130, 50), "Disconnect"))
                {
                    resetPlayer();
                    CmdRemovePlayerOnList(NetworkManagerHUD.Instance.playerName);
                    PlayerNetworkSetup.Instance.CmdDeletePlayer(gameObject);
                    NetworkManagerHUD.Instance.manager.StopHost();
                    Destroy(NetworkManagerHUD.Instance.gameObject);
                }                    
            }              
        }

        GUI.EndGroup();
    }

    [Command]
    public void CmdRemovePlayerOnList(string playerName)
    {
        RemovePlayerOnList(playerName);
    }

    public Player GetPlayer(string playerName)
    {
        Player player = (from item in NetworkManagerHUD.Instance.playerList
                         where item.playerName == playerName
                         select item).FirstOrDefault();

        return player;
    }

    public void UpdatePlayer(Player p)
    {
        var obj = NetworkManagerHUD.Instance.playerList.FirstOrDefault(x => x.index == p.index);
        if (obj != null) obj = p;
    }

    public void RemovePlayerOnList(string playerName)
    {
        Debug.Log(playerName);
        NetworkManagerHUD.Instance.playerList.RemoveAll(x => x.playerName == playerName);
    }

    [Command]
    public void CmdAddScore(string playerName, int score)
    {
        Player player = GetPlayer(playerName);

        player.score += score;

        UpdatePlayer(player);
    }

    public void AddScore(int scr)
    {
        score += scr;
        CmdAddScore(NetworkManagerHUD.Instance.playerName, scr);
    }

    public void RemoveLife()
    {
        audioS.clip = audioClipDie;
        audioS.Play();
        GetComponent<PlayerSync>().StartPosition();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("PickUp"))
        {
            AddScore(other.gameObject.GetComponent<ScoreMulti>().points);
        }
        if (other.gameObject.tag.Equals("Cup"))
        {
            AddScore(other.gameObject.GetComponent<ScoreMulti>().points);
            PlayerNetworkSetup.Instance.CmdOpenDoor();
        }
        if (openDoor && other.gameObject.tag.Equals("Door"))
        {
            CmdLoadGameOverScene();
        }
        if (other.gameObject.tag.Equals("GroundDie"))
        {
            RemoveLife();
        }
    }

}
