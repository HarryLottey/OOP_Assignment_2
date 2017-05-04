using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class EnemyT1 : Enemy
{
    [Header("EnemyTier1")]
   // public Rigidbody2D rigi;

    Vector2 knockback;

    [HideInInspector]
    public GameObject playerRef;

    //TODO: Apply HP GUI

    public override void Start()
    {
        base.Start();  
        playerRef = GameObject.FindGameObjectWithTag("Player");  
    }

    public override void FixedUpdate() // Detect Ray collision and apply knockback & reduce hp
    {
        base.FixedUpdate();
        playerRef = GameObject.FindGameObjectWithTag("Player");
        Player reference = playerRef.GetComponent<Player>();
        damage = 50f;
        reference.knockbackForce = 30f;

        // really bad knockback, applies to both enemies because I didnt get around to setting it up for specific enemies
         knockback = new Vector2(transform.position.x - reference.transform.position.x,0).normalized; 
        if(reference.enemyHit == true) 
        {
            health -= reference.dmg;
            rigi.AddForce(knockback * reference.knockbackForce);
        }
    }

    public override void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.tag == ("Player"))
        {
            print(other.transform.tag);
            Player stats = other.gameObject.GetComponent<Player>();
            stats.hitPoints -= damage; // minus the players hp by the damage specified on this script

        }
    }

    public override void Update()
    {
        base.Update();

        if (playerInRange == true) // when the player is close enough, the force mode is changed to impulse and the enemy launches towards you.
        {
            fMode = ForceMode2D.Impulse;
            speed = 100; // lowered to compensate for this forcemode
            updateRate = 0.5f; // lowered so that there is enough time for you to leave the ability activate range.
        }
        else
        {
            fMode = ForceMode2D.Force;
            speed = 600;
            updateRate = 2;
        }

        if(health <= 0)
        {
            Death();
        }
    }
}

