using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class Enemy : MonoBehaviour
{
    [Header("Enemy AI")]
    // what to seek
    public Transform target;
    // Path update rate
    public float updateRate = 2f;
    private Seeker seeker;
    public Rigidbody2D rigi;
    // calculated path
    public Path path;
    // speed per second
    public ForceMode2D fMode;
    public float speed = 200f;
    [HideInInspector]
    public bool pathIsEnded = false;
    public float nextWayPointDistance = 3;

    [Header("Attack")] // Things used for the attack
    public bool playerInRange = false;
    public float attackRadius;
    public Transform radiusPos;
    public LayerMask pLayerMask;
    public GameObject player;

    [Header("EnemyStats")]
    public float health;
    public float jumpforce;
    public float damage;
    // The waypoint we are currently moving towards
    private int currentWayPoint = 0;
    private bool searchingForPlayer = false;

    public virtual void Start()
    {
        seeker = GetComponent<Seeker>();
        rigi = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player"); // referencing

        if (target == null) // if there is no target, and not finding a target, start finding a target
        {
            if (!searchingForPlayer)
            {
                searchingForPlayer = true;
                StartCoroutine(SearchForPlayer());
            }
            return;
        }
        seeker.StartPath(transform.position, target.position, OnPathComplete);
        StartCoroutine(UpdatePath());
    }

    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }

    IEnumerator SearchForPlayer()
    {
        GameObject sResult = GameObject.FindGameObjectWithTag("Player");
        if (sResult == null)
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(SearchForPlayer());
        }
        else
        {
            searchingForPlayer = false;
            target = sResult.transform;
            StartCoroutine(UpdatePath());
            yield return false;
        }
    }

    IEnumerator UpdatePath() 
    {
        if (target == null)
        {
            yield return false;
        }
        seeker.StartPath(transform.position, target.position, OnPathComplete);
        yield return new WaitForSeconds(1f / updateRate);
        StartCoroutine(UpdatePath());
    }

    public virtual void Update()
    {
        //  A radius to attack or activate ability when player enters the range
        if (Physics2D.OverlapCircle(radiusPos.position, attackRadius, pLayerMask))
        {
            print("in Range");
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }
    }

    public virtual void FixedUpdate()
    {
        //TODO: Always look at player (homing projectiles)

        if (target == null)
        {
            return;
        }

        if (path == null)
        {
            return;
        }

        if (currentWayPoint >= path.vectorPath.Count)
        {
            if (pathIsEnded)
            {
                return;
            }
            //  Debug.Log("End of path reached");
            pathIsEnded = true;
            return;
        }

        pathIsEnded = false;

        // Dirrection to the next waypoint
        Vector3 dir = (path.vectorPath[currentWayPoint] - transform.position).normalized;
        dir *= speed * Time.fixedDeltaTime;

        // Movement for AI
        rigi.AddForce(dir, fMode);
        float dist = Vector3.Distance(transform.position, path.vectorPath[currentWayPoint]);
        if (dist < nextWayPointDistance)
        {
            currentWayPoint++;
            return;
        }
    }

    public virtual void Death()
    {   
        gameObject.SetActive(false);
    }

    private void OnDrawGizmos() // visualise attack range
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(radiusPos.position, attackRadius);
    }


    public virtual void OnCollisionEnter2D(Collision2D other)
    {
        if(other.transform.tag == ("Player"))
        {
            print(other.transform.tag);
            Player stats = other.gameObject.GetComponent<Player>();
            stats.hitPoints -= damage;
            
        }


    }




}  
