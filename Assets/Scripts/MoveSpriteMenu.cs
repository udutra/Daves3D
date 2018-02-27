using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MoveSpriteMenu : MonoBehaviour {

    public Sprite[] sprites;
    public Image background;
    public float timer = 0f;
    public int i = 1;

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            background.sprite = sprites[i];
            i++;
            if (i > 2)
                i = 0;

            timer = 0.1f;
        }
    }
}
