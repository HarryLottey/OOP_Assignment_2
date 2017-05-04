using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    Animator anim;
    public AudioClip[] fireSounds;
    AudioSource bulletAudio;
    public float audioCoolDownTime = 0.09f;
    public float coolDownTime = 0.5f;
    public float startingTime = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
        bulletAudio = GetComponent<AudioSource>();
        startingTime = Time.time;
    }

    void Update()
    {
        PlayerFire var = GetComponent<PlayerFire>();

        if (Input.GetKey(KeyCode.Z) && var.facingLeft == true && anim.GetBool("Ground") == true || Input.GetKey(KeyCode.Z) && var.facingRight == true && anim.GetBool("Ground") == true) // only play sounds after the player has moved and only if the play is on the ground
        {
            //Check if we have reached the cool down timer
            if (Time.time > startingTime + audioCoolDownTime)
            {
                // reset timer
                startingTime = Time.time;
                Invoke("playAudio", 0);
            }
        }
    }

    void playAudio()
    {
        bulletAudio.PlayOneShot(fireSounds[Random.Range(0, fireSounds.Length)]);
    }

    IEnumerator gunCoolDown() // When cooldownTime is reached half fire for a few seconds
    {
     yield return new WaitForSeconds(coolDownTime);
    }

}


   

   

  

