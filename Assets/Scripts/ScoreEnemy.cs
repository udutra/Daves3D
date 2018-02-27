using UnityEngine;
using System.Collections;

public class ScoreEnemy : MonoBehaviour {

    public int score;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Bullet"))
        {
            SingleGameController.Instance.AddScore(score);
            Destroy(transform.root.gameObject);
            Destroy(other.gameObject);
            //MultiGameController.Instance.AddScore(score);
            //PlayerNetworkSetup.Instance.CmdDestroy(transform.root.gameObject);
            //PlayerNetworkSetup.Instance.CmdDestroy(other.gameObject);
        }
    }
}
