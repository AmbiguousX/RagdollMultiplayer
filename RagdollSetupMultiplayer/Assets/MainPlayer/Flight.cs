using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flight : MonoBehaviour
{
    public float flyForce;
    public float powerUpStartTime;

    private Rigidbody rb;
    private MasterInputRef masterInputRef;

    public Animator animator;

    private ParticleSystem flyParticles;
    private RagDollController ragDollController;
    // Start is called before the first frame update
    void Start()
    {
        rb = transform.Find("PuppetMaster/PuppetHips").gameObject.GetComponent<Rigidbody>();
        ragDollController = transform.Find("PuppetMaster/PuppetHips").gameObject.GetComponent<RagDollController>();
        masterInputRef = transform.Find("MasterInputPlayer").gameObject.GetComponent<MasterInputRef>();
        animator = transform.Find("PlayerMesh").gameObject.GetComponent<Animator>();
        flyParticles = transform.Find("PuppetMaster/PuppetHips/FlyParticles").gameObject.GetComponent<ParticleSystem>();
        flyParticles.Play();

        
    }

    private void OnDisable()
    {
        ragDollController.isFlying = false;
        animator.SetBool("Flying", false);
        flyParticles.Stop();
        Destroy(this);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (masterInputRef.jump)
        {
            rb.AddForce(Vector3.up * flyForce * rb.mass);
            animator.SetBool("Flying", true);
            ragDollController.isFlying= true;
        }
        else
        {
            animator.SetBool("Flying", false);
            ragDollController.isFlying= false;
        }
    }

    private void Update()
    {
        
        CountDownTimer();
    }


    void CountDownTimer()
    {
        powerUpStartTime -= Time.deltaTime;
        if (powerUpStartTime < 0)
        {
            
            enabled = false;
            
        }
    }
}
