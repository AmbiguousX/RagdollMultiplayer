using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActiveRagAnim : MonoBehaviour
{
    private Animator anim;
    public MasterInputRef masterInputRef;


    public RagDollController ragController;
    public float topSpeed;
    public float acceletation;
    float blendFactor;






    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();



        acceletation = 1;

    }

    // Update is called once per frame
    private void FixedUpdate()
    {


    }

    void Update()
    {



        blendFactor = acceletation / topSpeed;
        ragController._maxSpeed = acceletation;

        Vector2 move = new Vector2(masterInputRef.move.x, masterInputRef.move.y);
        if (move.magnitude > .1 && ragController._maxSpeed < topSpeed)
        {
            //anim.SetBool("walking", true);

            acceletation += topSpeed * Time.deltaTime;
            Mathf.Clamp(acceletation, acceletation, topSpeed);



            anim.SetFloat("moveSpeed", blendFactor);
        }
        if (move.magnitude < .1 && ragController._maxSpeed > 0)
        {
            //anim.SetBool("walking", false);

            acceletation -= topSpeed * Time.deltaTime;

            anim.SetFloat("moveSpeed", blendFactor);
            
        }

        //if (masterInputRef.danceMenu)
        //{
        //    anim.SetBool("isDancing", true);
        //}
        //else
        //{
        //    anim.SetBool("isDancing", false);
        //}

        //if (masterInputRef.aim)
        //{
        //    anim.SetLayerWeight(1, 1);
        //    //anim.SetBool("isAiming", true);
        //}
        //else
        //{
        //    anim.SetLayerWeight(1, 0);
        //    //anim.SetBool("isAiming", false);
        //}



        if (masterInputRef.jump && ragController.jumpGrounded && !ragController.isJumping)
        {
            anim.SetBool("isJumping", true);
            anim.SetBool("isGrounded", false);


        }

        if (!ragController.jumpGrounded)
        {
            

            anim.SetBool("isJumping", false);
            anim.SetBool("isFalling", true);
            anim.SetFloat("moveSpeed", 0);

            anim.SetBool("isGrounded", false);



        }

        if (ragController.jumpGrounded && !masterInputRef.jump)
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isGrounded", true);
            anim.SetBool("isFalling", false);
            anim.SetFloat("moveSpeed", blendFactor);

        }

 

        if (masterInputRef.slide)
        {
            anim.SetLayerWeight(2, 1);
        }

        else
        {
            anim.SetLayerWeight(2, 0);
        }

        if (masterInputRef.punch)
        {
            anim.SetBool("isKicking", true);
        }
        else
        {
            anim.SetBool("isKicking", false);
        }


        if (!ragController.jumpGrounded && ragController.gameObject.GetComponent<Rigidbody>().velocity.y < -10)
        {
            anim.SetBool("FreeFall", true);
        }
        else
        {
            anim.SetBool("FreeFall", false);
        }

        if (!ragController.jumpGrounded && ragController.isHanging)
        {
            anim.SetBool("isHanging", true);
        }
        else
        {
            anim.SetBool("isHanging", false);
        }


    }


}
