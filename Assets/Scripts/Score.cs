using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour
{
    public int points = 10;

    void Update()
    {
        transform.Rotate(new Vector3(0, 0, 45) * Time.deltaTime);
    }
}
