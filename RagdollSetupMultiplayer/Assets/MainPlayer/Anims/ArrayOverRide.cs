using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayOverRide : MonoBehaviour
{

    public AnimationClip[] danceAnimationClip = new AnimationClip[3];


    private Animator animator;
    protected AnimatorOverrideController animatorOverrideController;

    private bool restart = true;

    public void Start()
    {
        animator = GetComponent<Animator>();

        {


            animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = animatorOverrideController;


        }
    }

    public void animPick()
    {



        //animator.SetFloat("danceTimer", 10);

        if (restart)
        {
            animatorOverrideController["Ymca Dance"] = danceAnimationClip[Random.Range(0, danceAnimationClip.Length)];
            animator.SetBool("isDancing", true);
            //Invoke("backToIdle", 5);

            StartCoroutine(backToIdle());

            restart = false;
        }




    }

    IEnumerator backToIdle()
    {
        yield return new WaitForSeconds(5);
        animator.SetBool("isDancing", false);
        restart = true;

    }
}



