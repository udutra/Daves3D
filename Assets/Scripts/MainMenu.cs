using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public void LoadStartGame()
    {
        SceneManager.LoadScene("Single_fase_1", LoadSceneMode.Single);
    }

    public void LoadMultiPlayer()
    {
        SceneManager.LoadScene("LobbyMenu", LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
