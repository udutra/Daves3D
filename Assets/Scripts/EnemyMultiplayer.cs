using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class EnemyMultiplayer : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public Animator animator;

    //private static EnemyMultiplayer instance = null;
    //public static EnemyMultiplayer Instance { get { return instance; } }

    private float timer = 1f;

    //public int score = 10;

    

    private void Start()
    {
        //instance = this;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
            animator.SetBool("Atack", true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
            animator.SetBool("Atack", false);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            PlayerNetworkSetup.Instance.CmdFire(gameObject);
        }          
    }

    [Command]
    public void CmdFire()
    {
        if (timer >= 0)
            timer -= Time.deltaTime;

        if (timer < 0f)
        {
            

            // Create the Bullet from the Bullet Prefab
            var bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);

            // Add velocity to the bullet
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 30;

            // Spawn the bullet on the Clients
            NetworkServer.Spawn(bullet);

            // Destroy the bullet after 2 seconds
            Destroy(bullet, 2.0f);

            timer = 1f;
        }
    }
}
