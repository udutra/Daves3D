using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {

    public AudioSource audioS;

    public float timer = 10f;
    public Text text;

    private void Update()
    {
        if (timer >= 0)
            timer -= Time.deltaTime;

        text.text = ((int)timer).ToString();

        if (timer < 0)
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
}
