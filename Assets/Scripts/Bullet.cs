using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            if(other.gameObject.GetComponent<MultiGameController>())
                other.gameObject.GetComponent<MultiGameController>().RemoveLife();
            if(other.gameObject.GetComponent<SingleGameController>())
                other.gameObject.GetComponent<SingleGameController>().RemoveLife();
        }
            
    }
}
