using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    public float runSpeed = 10;
    public float maxJetpack = 50f;

    public float gravity = -12;
    public float jumpHeight = 1;

    public float timerBullet = 0;

    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;

    public float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;
    float currentSpeed;
    float velocityY;

    public Animator animator;
    CharacterController controller;

    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    public bool hasPistol = false;
    public bool hasJetPack = false;

    //public Vector3 startPoint;

    public GameObject pistolPrefab;
    public GameObject jetPackPrefab;

    public AudioSource audioS;
    public AudioSource audioJetPack;
    public AudioClip audioClipPickUp;
    public AudioClip audioClipCup;
    public AudioClip audioClipPistol;
    public AudioClip audioClipJet;
    public AudioClip audioClipGun;
    public AudioClip audioClipDoor;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        DontDestroyOnLoad(gameObject);

        //startPoint = GameObject.FindGameObjectWithTag("StartPoint").transform.position;
        //GetComponent<PlayerSync>().lastPos = startPoint;
        //GetComponent<PlayerSync>().playerTransform.position = startPoint;
        //transform.position = startPoint;
    }

    void Awake()
    {
        //transform.position = startPoint;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
            
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        velocityY += Time.deltaTime * gravity;
        Move(input);
        Animating(input);

        if(timerBullet >= 0)
            timerBullet -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button1))
            Jump();

        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Joystick1Button0))
            Shoot();

        if (Input.GetMouseButton(1) || Input.GetButton("CircleButton"))
            JetPack();

        if (Input.GetMouseButtonDown(1) && hasJetPack || Input.GetKeyDown(KeyCode.Joystick1Button2) && hasJetPack)
        {
            audioJetPack.Play();
        }
        if (Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.Joystick1Button2))
        {
            audioJetPack.Stop();
        }
    }

    void Move(Vector2 dir)
    {
        Vector2 inputDir = dir.normalized;

        //Smooth Rotation
        if (inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }

        float targetSpeed = runSpeed * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        // transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);

        Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;
        controller.Move(velocity * Time.deltaTime);

        if (controller.isGrounded)
        {
            animator.SetBool("IsJetPack", false);
            velocityY = 0;
        }
    }

    void Animating(Vector2 dir)
    {
        // Create a boolean that is true if either of the input axes is non-zero.
        //bool walking = h != 0f || v != 0f;
        bool walking = dir.x != 0f || dir.y != 0f;

        // Tell the animator whether or not the player is walking.
        animator.SetBool("IsRunning", walking);
    }

    void Jump()
    {
        if (controller.isGrounded)
        {
            CmdSetAnimTrigger("IsJumping");
            //animator.SetTrigger("IsJumping");
            float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
            velocityY = jumpVelocity;
        }
    }
    
    void Shoot()
    {
        if (controller.isGrounded && hasPistol && timerBullet < 0)
        {
            CmdSetAnimTrigger("IsShoot");
            CmdFire();
            timerBullet = 0.5f;
            audioS.clip = audioClipGun;
            audioS.Play();
        }
    }

    void JetPack()
    {
        if (hasJetPack)
        {
            animator.SetBool("IsJetPack", true);
            float jumpVelocity = Mathf.Sqrt(-1 * gravity * 2);
            velocityY = jumpVelocity;

            maxJetpack -= Time.deltaTime * 5f;

            if (maxJetpack < 0)
                hasJetPack = false;
        }
    }

    //Set Trigger Network Animation
    [Command]
    public void CmdSetAnimTrigger(string triggerName)
    {
        if (!isServer)
        {
            animator.SetTrigger(triggerName);
        }
        RpcSetAnimTrigger(triggerName);        
    }

    [ClientRpc]
    public void RpcSetAnimTrigger(string triggerName)
    {
        if(animator != null)
            animator.SetTrigger(triggerName);
    }

    [Command]
    void CmdFire()
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 20;

        // Spawn the bullet on the Clients
        NetworkServer.Spawn(bullet);

        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("PickUp"))
        {
            audioS.clip = audioClipPickUp;
            audioS.Play();
            CmdSetDestroy(other.gameObject);
        }
        
        if (other.gameObject.tag.Equals("Cup"))
        {
            audioS.clip = audioClipCup;
            audioS.Play();
            CmdSetDestroy(other.gameObject);
        }

        if (other.gameObject.tag.Equals("Pistol") && !hasPistol)
        {
            audioS.clip = audioClipPistol;
            audioS.Play();
            hasPistol = true;
            CmdSetDestroy(other.gameObject);
            CmdActiveItens(1);
        }
        if (other.gameObject.tag.Equals("JetPack") && !hasJetPack)
        {
            audioS.clip = audioClipJet;
            audioS.Play();
            hasJetPack = true;
            CmdSetDestroy(other.gameObject);
            CmdActiveItens(2);
            maxJetpack = 50f;
        }
        if (other.gameObject.tag.Equals("Door"))
        {
            GameObject door = GameObject.FindGameObjectWithTag("Door");
            GameObject portal = door.transform.GetChild(1).gameObject;
            if (portal.activeSelf == true)
            {
                audioS.clip = audioClipDoor;
                audioS.Play();
            }
        }
    }

    [Command]
    void CmdActiveItens(int item)
    {
        ActiveItens(item);
        RpcActiveItens(item);
    }

    [ClientRpc]
    void RpcActiveItens(int item)
    {
        ActiveItens(item);
    }

    void ActiveItens(int item)
    {
        if (item == 1)
            pistolPrefab.SetActive(true);

        if (item == 2)
            jetPackPrefab.SetActive(true);
    }

    [Command]
    public void CmdSetDestroy(GameObject gameObject)
    {
        if (!isServer)
            Destroy(gameObject);

        RpcSetActive(gameObject);
    }

    [ClientRpc]
    public void RpcSetActive(GameObject gameObject)
    {
        Destroy(gameObject);
    }
}
