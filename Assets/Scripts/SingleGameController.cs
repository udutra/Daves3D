using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SingleGameController : MonoBehaviour {

    public GameObject player;
    public GameObject door;
    public GameObject portal;
  
    public int Lifes = 3;
    public bool removeLife = false;
    public Texture2D davesLife;
    public Texture2D openDoorTexture;
    public Texture2D gunTexture;
    public Texture2D jackpackTexture;

    public Texture2D jackPackBar_1;
    public Texture2D jackPackBar_2;

    public bool openDoor = false;

    private string fmt = "00000";
    public int score = 0;
    public Font font;

    public static int currentStage;

    public Text scoreGameOver;
    public Text levelGameOver;

    private static SingleGameController instance = null;

    public AudioSource audioS;

    public AudioClip audioClipDie;
    public AudioClip audioClipgameOver;

    public static SingleGameController Instance
    {
        get { return instance; }
    }

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

    private void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetActiveScene().name.Equals("Menu"))
            Destroy(gameObject);

        if (GameObject.FindGameObjectWithTag("Door"))
        {
            door = GameObject.FindGameObjectWithTag("Door");
            portal = door.transform.GetChild(1).gameObject;
            portal.SetActive(false);
        }

        if (SceneManager.GetActiveScene().name.Equals("Single_GameOver"))
        {
            scoreGameOver = GameObject.Find("Score").GetComponent<Text>();
            levelGameOver = GameObject.Find("Level").GetComponent<Text>();

            scoreGameOver.text = score.ToString(fmt);
            levelGameOver.text = currentStage.ToString();

            gameObject.SetActive(false);
        }
        else
        {
            openDoor = false;
            currentStage++;
        }
    }

    public void OpenDoor()
    {
        portal.SetActive(true);
        openDoor = true;
    }

    void OnGUI()
    {
        int space = 20;
        GUI.skin.font = font;
        GUI.skin.label.fontSize = 40;

        GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));

        if (Lifes > 0)
        {
            GUI.Label(new Rect(15, 15, 300, 50), score.ToString(fmt));
            for (int i = 0; i < Lifes; i++)
            {
                GUI.Label(new Rect(Screen.width - 200 + (30 * i) + space, 15, 64, 32), davesLife);
                space += 20;
            }
            GUI.Label(new Rect((Screen.width / 2) - (150 / 2), 15, 300, 50), "Level : " + currentStage);

            if (player.GetComponent<SinglePlayerController>().hasPistol)
                GUI.Label(new Rect(15, Screen.height - 50, 300, 50), gunTexture);

            if (openDoor)
                GUI.Label(new Rect(Screen.width - 271, Screen.height - 45, 400, 100), openDoorTexture);

            if (player.GetComponent<SinglePlayerController>().hasJetPack)
            {
                GUI.Label(new Rect((Screen.width / 2) - (150 / 2), Screen.height - 50, 300, 50), jackpackTexture);

                GUI.DrawTexture(new Rect((Screen.width / 2) - (340 / 2), Screen.height - 20, 300/50* player.GetComponent<SinglePlayerController>().maxJetpack, 15), jackPackBar_2);
                //GUI.DrawTexture(new Rect((Screen.width / 2) - (150 / 2), Screen.height - 45, 300, 50), jackPackBar_1);
            }
                
        }

        GUI.EndGroup();
    }

    public void AddScore(int scr)
    {
        score += scr;
    }

    public void RemoveLife()
    {
        if (!removeLife)
        {
            if (Lifes > 1)
            {
                audioS.clip = audioClipDie;
                audioS.Play();
            }
            else
            {
                audioS.clip = audioClipgameOver;
                audioS.Play();
            }
           
            removeLife = true;
            Lifes--;
            player.transform.position = player.GetComponent<SinglePlayerController>().startPoint;

            

            if (Lifes <= 0)
                SceneManager.LoadScene("Single_GameOver", LoadSceneMode.Single);
            removeLife = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("PickUp"))
        {
            AddScoreAndDestroy(other.gameObject);
        }
        if (other.gameObject.tag.Equals("Cup"))
        {
            AddScoreAndDestroy(other.gameObject);
            OpenDoor();
        }
        if (openDoor && other.gameObject.tag.Equals("Door"))
        {
            if(currentStage != 3)
                SceneManager.LoadScene("Single_fase_" + (currentStage + 1), LoadSceneMode.Single);
            else
                SceneManager.LoadScene("Single_GameOver", LoadSceneMode.Single);
        }
        if (other.gameObject.tag.Equals("GroundDie"))
        {
            RemoveLife();
        }   
    }

    void AddScoreAndDestroy(GameObject obj)
    {
        AddScore(obj.GetComponent<Score>().points);
        Destroy(obj);
    }
}