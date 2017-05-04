using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Values")]
    public float jumpForce = 600;
    public float speed = 20;
    [Header("Health")]
    public float hitPoints = 100f;
    private Animator anim;
    public Rigidbody2D rigi;

    [Header("DamageDealing")]
    public float dmg;
    public float knockbackForce;


    [Header("Detect Collision For Ground")]
    public LayerMask whatIsGround; // A layer masked to indicate groud
    public Transform groundCheck;
    public Transform ceilingCheck;
    private float groundRadius = 0.2f;
    private float ceilingRadius = 9.1f;
    private bool grounded;
    private bool ceilingCol;

    [Header("Ray")]
    public Transform rayOrigin;
    private LayerMask enemyLayerMask = 1 << 9;
    public float rayDistance;
    public bool enemyHit;
    public float rateOfFire = 0.32f;
    public float startingTime = 0f;



    void Awake()
    {
        // Set up all our references
        groundCheck = transform.Find("GroundCheck");
        anim = GetComponent<Animator>();
        rigi = GetComponent<Rigidbody2D>();
        anim.SetBool("Ground", grounded);
    }

    void Start()
    {
        StartCoroutine(AnimInitialise());
        startingTime = Time.time;
    }

    void Update()
    {
        Death();

        // Movement
        float hAxis = Input.GetAxis("Horizontal");
        Vector2 movement = new Vector2(hAxis, 0) * speed * Time.deltaTime;
        rigi.velocity = new Vector2(movement.x, rigi.velocity.y);

        // Speed variable for animator is being set by the player movement
        anim = GetComponent<Animator>();
        float move = Input.GetAxis("Horizontal");
        anim.SetFloat("Speed", move);

        if (grounded == true) // Jump Fall speed > than jump elevate speed
        {
            rigi.gravityScale = 3;
        }
        else
        {
            StartCoroutine(JumpFall());
        }

        // Jump values
        if (grounded && Input.GetButtonDown("Jump"))
        {
            rigi.AddForce(new Vector2(0, jumpForce));
            anim.SetBool("Ground", false);
            grounded = false;
            anim.SetBool("Ground", grounded);
        }

        // Rays
        PlayerFire var = GetComponent<PlayerFire>();
        if (enemyHit == true && !var.firing) // Make sure enemyHit is set to false when not firing
        {
            enemyHit = false;
        }

        if (Input.GetKey(KeyCode.Z) && var.facingRight == true && grounded == true && Time.time > startingTime + rateOfFire)
        {
            startingTime = Time.time;
            Invoke("RayRight", 0f);
        } // if Facing right and fire is pressed, while on the ground, ray right is invoked.
        else
        {
            CancelInvoke("RayRight");
        }

        if (Input.GetKey(KeyCode.Z) && var.facingLeft == true && grounded == true && Time.time > startingTime + rateOfFire)
        {
            startingTime = Time.time;
            Invoke("RayLeft", 0);
        } // if Facing left and fire is pressed, while on the ground, ray left is invoked.
        else
        {
            CancelInvoke("RayLeft");
        }
    }

    void FixedUpdate()
    {
        // Performing groundcheck using physcics 2D
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        anim.SetBool("Ground", grounded);
    }
    IEnumerator AnimInitialise() // Fixes an error message with the animator
    {
        yield return new WaitUntil(() => anim.isInitialized);
        anim = GetComponent<Animator>();
    }

    void RayRight() 
    {
        PlayerFire var = GetComponent<PlayerFire>();
        Vector2 forward = rayOrigin.transform.TransformDirection(Vector2.right) * rayDistance; // RayRight Drawn
        Debug.DrawRay(rayOrigin.transform.position, forward, Color.green);

        if (Physics2D.Raycast(rayOrigin.transform.position, rayOrigin.TransformDirection(Vector2.right), rayDistance, enemyLayerMask) && var.firing == true)
        {
            enemyHit = true; 
        }
        else
        {
            enemyHit = false;
        }
    }

    void RayLeft()
    {
        Vector2 backward = rayOrigin.transform.TransformDirection(Vector2.left) * rayDistance; // RayLeft Drawn
        Debug.DrawRay(rayOrigin.transform.position, backward, Color.green);

        if (Physics2D.Raycast(rayOrigin.transform.position, rayOrigin.transform.TransformDirection(Vector2.left), rayDistance, enemyLayerMask))
        {
            enemyHit = true;
        }
        else
        {
            enemyHit = false;
        }
    }

    IEnumerator JumpFall() // when falling the gravity increases, making the fall faster
    {
        yield return new WaitForSeconds(0.2f);
        rigi.gravityScale = 4;
    }

    void Death()
    {
        if (hitPoints <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
