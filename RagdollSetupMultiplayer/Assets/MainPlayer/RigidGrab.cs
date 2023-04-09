using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidGrab : MonoBehaviour
{
    
    public bool isGrabbing;
    private FixedJoint joint;
    public MasterInputRef masterInputRef;

    public int breakStrength;
    
    //Can Always switch the FixedJoint to a ConfigurableJoint if need be

    
    void Start()
    {
        
        isGrabbing = false;
    }

    private void FixedUpdate()
    {
        if ((!masterInputRef.aim && isGrabbing) || masterInputRef.shoot)
        {
            Destroy(joint);
            isGrabbing = false;
        }

        
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (masterInputRef.aim && !isGrabbing && collision.gameObject.GetComponent<Rigidbody>() != null && collision.gameObject.tag != "mainPlayer"
            && collision.gameObject.tag != "sorry")
        {
            isGrabbing = true;
            joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = collision.rigidbody;

            
            //joint.xMotion = ConfigurableJointMotion.Locked;
            //joint.yMotion = ConfigurableJointMotion.Locked;
            //joint.zMotion = ConfigurableJointMotion.Locked;

        } 
        
    }

    
}
