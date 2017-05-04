using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class EnemyT2 : Enemy
{
    public Sprite[] sprites;
    Vector2 abliity;
    Vector2 knockback;
    [HideInInspector]
    public GameObject playerRef;
    SpriteRenderer spriteRend;
    
    public override void Start()
    {
        base.Start();
        Resources.Load<Sprite>("EnemyT2_Ability");
        Resources.Load<Sprite>("EnemyT2");
        spriteRend = gameObject.GetComponent<SpriteRenderer>();
        playerRef = GameObject.FindGameObjectWithTag("Player");
    }
    
   public override void Update()
    {
        base.Update();
        Player reference = playerRef.GetComponent<Player>();
        if (playerInRange == true)
        {
            spriteRend.sprite = sprites[1]; // Change sprite
            abliity = new Vector2(transform.position.x - reference.transform.position.x, 0).normalized; 
            reference.rigi.AddForce(abliity * reference.knockbackForce * 10f); // Pulls you towards the enemy as the ability activates
            StartCoroutine(AbilityWait());
        }
        else
        {
            spriteRend.sprite = sprites[0]; // revert to defualt sprite
        }

        if (health <= 0)
        {
            Death();
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        playerRef = GameObject.FindGameObjectWithTag("Player");
        Player reference = playerRef.GetComponent<Player>();
        knockback = new Vector2(transform.position.x - reference.transform.position.x, 0).normalized;

        if (reference.enemyHit == true) // knockback and damage based on the players damage
        {
            health -= reference.dmg;
            rigi.AddForce(knockback * reference.knockbackForce);
        }
    }

    public override void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.tag == ("Player")) // apply damage to the player on collision
        {
            print(other.transform.tag);
            Player stats = other.gameObject.GetComponent<Player>();
            stats.hitPoints -= damage;
        }
    }


    IEnumerator AbilityWait() // make sure the ability range decreases to 0 so the ability does not stay active forever once activated
    {
        print("abilityActivated");
        
        yield return new WaitForSeconds(1.5f);
        attackRadius = 0f;
        yield return new WaitForSeconds(1.5f);
        attackRadius = 3.5f; // defualt radius
        
        
    }
}
