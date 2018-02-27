using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    Animator animator;

    public bool fireReady = true;
    private float timer = 1f;

   
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            animator.SetBool("Atack", true);
            Shoot();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            animator.SetBool("Atack", false);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (!fireReady)
            timer -= Time.deltaTime;

        if (timer < 0f)
            fireReady = true;

        if (fireReady)
        {
            var bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 30;
            Destroy(bullet, 2.0f);
           
            fireReady = false;
            timer = 1f;
        }
    }
}
