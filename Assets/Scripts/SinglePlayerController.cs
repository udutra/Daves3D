using UnityEngine;
using System.Collections;

public class SinglePlayerController : MonoBehaviour {

    public float runSpeed = 10;
    public float maxJetpack = 50f;

    public float gravity = -12;
    public float jumpHeight = 1;

    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;

    public float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;
    float currentSpeed;
    float velocityY;

    public float timerBullet = 0;

    Animator animator;
    CharacterController controller;

    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    public bool hasPistol = false;
    public bool hasJetPack = false;

    public Vector3 startPoint;

    public GameObject pistolPrefab;
    public GameObject jetPackPrefab;

    public AudioSource audioS;
    public AudioSource audioJetPack;
    public AudioClip audioClipPickUp;
    public AudioClip audioClipCup;
    public AudioClip audioClipDoor;
    public AudioClip audioClipGun;
    public AudioClip audioClipPistol;
    public AudioClip audioClipJet;


    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        startPoint = GameObject.FindGameObjectWithTag("StartPoint").transform.position;
        transform.position = startPoint;
    }

    void OnLevelWasLoaded(int level)
    {
        if (GameObject.FindGameObjectWithTag("StartPoint"))
        {
            startPoint = GameObject.FindGameObjectWithTag("StartPoint").transform.position;
            transform.position = startPoint;
        }
    }

    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        velocityY += Time.deltaTime * gravity;
        Move(input);
        Animating(input);

        if (timerBullet >= 0)
            timerBullet -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button1))
            Jump();

        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Joystick1Button0))
            Shoot();

        if (Input.GetMouseButton(1) || Input.GetButton("CircleButton"))
        {
            JetPack();
        }
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

        if (inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }

        float targetSpeed = runSpeed * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

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
        bool walking = dir.x != 0f || dir.y != 0f;
        animator.SetBool("IsRunning", walking);
    }

    void Jump()
    {
        if (controller.isGrounded)
        {
            animator.SetTrigger("IsJumping");
            float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
            velocityY = jumpVelocity;
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
            {
                hasJetPack = false;
                audioJetPack.Stop();
            }
        }
    }

    void Shoot()
    {
        if (controller.isGrounded && hasPistol && timerBullet < 0)
        {
            animator.SetTrigger("IsShoot");
            Fire();
            timerBullet = 0.5f;
            audioS.clip = audioClipGun;
            audioS.Play();
        }
    }

    void Fire()
    {
        var bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 20;
        Destroy(bullet, 2.0f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Pistol"))
        {
            hasPistol = true;
            Destroy(other.gameObject);
            pistolPrefab.SetActive(true);          
            audioS.clip = audioClipPistol;
            audioS.Play();
            
        }
        if (other.gameObject.tag.Equals("JetPack"))
        {
            hasJetPack = true;
            Destroy(other.gameObject);
            jetPackPrefab.SetActive(true);
            audioS.clip = audioClipJet;
            audioS.Play();
            maxJetpack = 50f;
        }

        if (other.gameObject.tag.Equals("PickUp"))
        {
            audioS.clip = audioClipPickUp;
            audioS.Play();
        }

        if (other.gameObject.tag.Equals("Cup"))
        {
            audioS.clip = audioClipCup;
            audioS.Play();
        }

        if (other.gameObject.tag.Equals("Door"))
        {
            GameObject door = GameObject.FindGameObjectWithTag("Door");
            GameObject portal = door.transform.GetChild(1).gameObject;
            if(portal.activeSelf == true)
            {
                audioS.clip = audioClipDoor;
                audioS.Play();
            }
        }
    }
}
