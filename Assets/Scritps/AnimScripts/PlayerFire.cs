using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class PlayerFire : MonoBehaviour
{
    public float fireAnimDelay;
    Animator anim;
    public BoxCollider2D hitBox;
    Vector2 xOffset;
    Vector2 xOffsetLeft;
    Vector2 xOffsetFiringLeft;
    Vector2 xOffsetFiringRight;
    Vector2 gcTransformXRight;
    Vector2 gcTransformXLeft;
    public Transform groundCheck;
    public Transform ceilingCheck;
    public bool facingRight = false;
    public bool facingLeft = false;
    public bool firing;
    bool idle;
    private float ceilingX;
    private float groundX;
    
    void Awake()
    {
        // Hitbox Offset Values
        xOffset = new Vector2(-0.315f, 0); 
        xOffsetLeft = new Vector2(0.315f, 0);
        xOffsetFiringLeft = new Vector2(0.9f, 0);
        xOffsetFiringRight = new Vector2(-0.9f, 0);
        gcTransformXRight = new Vector2(-0.3f,-0.6f);
        gcTransformXLeft = new Vector2(0.3f, -0.6f);

        ceilingX = ceilingCheck.transform.position.x;
        groundX = groundCheck.transform.position.x;

        // Default Offset
        hitBox.offset = xOffset;
     //   groundCheck.transform.position = gcTransformXRight;
        anim = GetComponent<Animator>();
        anim.GetFloat("Speed");
    }
    
    void FlipCollisions(bool isFlipped) // Make the ground and ceiling check follow the player correctly
    {
        Vector3 ceilingPos = ceilingCheck.transform.localPosition;
        Vector3 groundPos = groundCheck.transform.localPosition;
        if (isFlipped)
        {
            ceilingPos.x = -ceilingX;
            groundPos.x = -groundX;
        }
        else
        {
            ceilingPos.x = ceilingX;
            groundPos.x = groundX;
        }
        ceilingCheck.transform.localPosition = ceilingPos;
        groundCheck.transform.localPosition = groundPos;
    }

    void Update()
    {
        
        // Set Bools based on what direction you are facing.
        
        if (Input.GetKey(KeyCode.RightArrow))
        {
            facingRight = true;
            facingLeft = false;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            facingLeft = true;
            facingRight = false;
        }

        if (Input.GetKey(KeyCode.D)) // alternate
        {
            facingRight = true;
            facingLeft = false;
        }

        if (Input.GetKey(KeyCode.A)) // alternate
        {
            facingLeft = true;
            facingRight = false;
        }

        if(facingRight == true) // anim
        {
            anim.SetBool("FacingRight", true);
            anim.SetBool("FacingLeft", false);
        }

        if (facingLeft == true) // anim
        {
            anim.SetBool("FacingRight", false);
            anim.SetBool("FacingLeft", true);
        }

        FlipCollisions(facingLeft);

        // Check if firing

        if (Input.GetKey(KeyCode.Z) && facingLeft == true && facingRight == true) // only fire after the player has moved
        {
            firing = true;
            anim.SetBool("Firing", true);
        }
        else
        {
            firing = false;
            anim.SetBool("Firing", false);
        }

        // Animation stuff

        // Hitbox & public transform adjustments, because of dodgey sprite animation.


        if (firing == true && facingLeft == true)
        {

            hitBox.offset = xOffsetFiringLeft; // 0.9 // Not Updating?
        }

        if (firing == true && facingRight == true)
        {

            hitBox.offset = xOffsetFiringRight; // -0.9 // Not Updating?
        }

        if (facingLeft == true && firing == false)
        {
            hitBox.offset = xOffsetLeft;
            
        }

        if (facingRight == true && firing == false)
        {
            hitBox.offset = xOffset;
           
        }

        if (anim.GetFloat("Speed") == 0.0f) // if speed is 0 you are idle
        {
            idle = true; // this is useless at the moment because idle animation has not been made
        }

        if (anim.GetFloat("Speed") == 0.0f && idle == true) // if speed is 0 you are idle
        {
            
        }

        if (Input.GetKey(KeyCode.Z))
        {
            anim.SetBool("Firing", true);
            firing = true;
        }
        else
        {
            firing = false;
            anim.SetBool("Firing", false);
        }
    }
}
