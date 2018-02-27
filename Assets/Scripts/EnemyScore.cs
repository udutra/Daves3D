using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class EnemyScore : NetworkBehaviour
{
    public int score;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("MultiBullet"))
        {
            MultiGameController.Instance.AddScore(score);
            PlayerNetworkSetup.Instance.CmdDestroy(transform.root.gameObject);
            PlayerNetworkSetup.Instance.CmdDestroy(other.gameObject);
        }
    }
}
