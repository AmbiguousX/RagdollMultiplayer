using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using RootMotion.Dynamics;


public class RagDollController : MonoBehaviour
{
    public LayerMask GroundLayer;
    
    public bool isMultiplayer;
    public LayerMask ClimbLayer;
    private float hangTime = 0;
    RaycastHit frontHit;
    RaycastHit backHit;
    public bool isHanging;
    private bool canHangAgain = true;
    private float hangAgainTimer;
    private Vector3 landPoint;

    public MasterInputRef masterInputRef;
    private PuppetMaster puppetMaster;


    //master "isActive" for player switching
    public float isActive = 1;

    private Rigidbody _rb;
    private Transform cameraTransform;

    //Can implement ground bound if I want the ground detectionpoint to be something other than the hips
    //public GameObject groundBody;

    private SpringJoint springJoint;
    //InputManagerStuff


    private Vector2 move;
    private bool jumpPressed;
    private bool aim;

    public float jointRotationOffset = 180;
    public float jumpGroundCheckDistance = 3;

    public bool rotaNorm = false;

    //For the normal vector move
    private Vector3 lookDir;
    private Vector3 moveLook;

    private float targetAngle;

    private bool isSpinning;
    float spinAngle = 0;

    public float flightAngle;
    public bool isFlying;




    private float horizontal;
    private float vertical;
    private float jumpStrength;
    public bool isJumping;
    public bool jumpGrounded;

    


    private ConfigurableJoint joint;
    private float collisionImpulse;

    public Transform headPosition;

    


    //Joe Binns MotorManager

    private float _speedFactor = 1f;
    private float _maxAccelForceFactor = 1f;
    private Vector3 _m_GoalVel = Vector3.zero;
    [Header("Motor Manager")]
    [SerializeField] public float _maxSpeed = 8f;
    [SerializeField] private float _acceleration = 200f;
    [SerializeField] private float _maxAccelForce = 150f;
    [SerializeField] private float _leanFactor = 0.25f;
    public float jumpScale = 50;
    [SerializeField] private AnimationCurve _accelerationFactorFromDot;
    [SerializeField] private AnimationCurve _maxAccelerationForceFactorFromDot;
    public Vector3 _moveForceScale = new Vector3(1f, 0f, 1f);
    //public Oscillator _squashAndStretchOcillator;



    private Quaternion _lastTargetRot;

    public float angularDisplacementMagnitude;
    private Vector3 _rotAxis;

    /// <summary>
    /// Get the rigidbody.
    /// </summary>


    

    void Start()
    {

        puppetMaster = transform.parent.gameObject.GetComponent<PuppetMaster>();
        _rb = GetComponent<Rigidbody>();
        isJumping = false;
        isSpinning = false;
        isHanging = false;

        //Instantiated in the startfunction...the main Camera's attributes by default
        cameraTransform = Camera.main.transform;
        
        joint = GetComponent<ConfigurableJoint>();

        jumpGrounded = true;
        
        
    }

    private void Update()
    {
        jumpPressed = masterInputRef.jump;
        aim = masterInputRef.aim;
        move = masterInputRef.move;

        
    }

    /// <summary>
    /// Hold the rigid body above any underneath surface within range using a spring force.
    /// </summary>
    void FixedUpdate()
    {

        

            inputManager();

            jointRotation();

        

            CharacterMove();

        if (isHanging)
        {
            hangTime += Time.deltaTime;

            if(hangTime > 3)
            {
                _rb.isKinematic= false;
                hangTime= 0;
                isHanging= false;
                canHangAgain = false;
                StartCoroutine(HangAgainTimer());
                //canHangAgain = false;
            }
        }

        Debug.Log(hangTime);

        if (EdgeClimb() && masterInputRef.aim && isMultiplayer && !isFlying && canHangAgain && !jumpGrounded && !GetComponentInChildren<FixedJoint>())
        {
            
            
            

            _rb.useGravity = false;
            _rb.velocity = Vector3.zero;
            isHanging = true;
            //_rb.position = new Vector3(landPoint.x, )

            Vector3 hitPointDistance = (landPoint - headPosition.position);

            if (hitPointDistance.magnitude > .5)
            {
                _rb.AddForce(hitPointDistance.normalized * 2000, ForceMode.Acceleration);
            }
            else 
            {
                _rb.isKinematic = true;

                
            }

            hitPointDistance.x = 0;
            hitPointDistance.y = 0;

            joint.SetTargetRotationLocal(Quaternion.LookRotation(hitPointDistance.normalized), Quaternion.Euler(0, 0 + jointRotationOffset, 0));
        }
        else
        {
            if (isHanging && masterInputRef.jump && !masterInputRef.aim && !jumpGrounded && _rb.isKinematic == true)
            {
                _rb.useGravity = true;
                _rb.isKinematic = false;
                hangTime = 0;


                //canHangAgain= false;
                //StartCoroutine(HangAgainTimer());

                //if (!gameObject.transform.root.gameObject.TryGetComponent<Big>(out Big large))
                //{
                //    _rb.AddForce(Vector3.up * 200, ForceMode.VelocityChange);
                //}
                //else
                //{
                    _rb.AddForce(Vector3.up * 75, ForceMode.VelocityChange);
                //}
                
                isHanging = false;
            }
        }



        //if (platformHit)
        //{
        //    SetPlatform(hit);
        //}





        //if (aim && isActive == 1)
        //{
        //    calculateAim();


        //}







    }

    IEnumerator HangAgainTimer()
    {
        yield return new WaitForSeconds(3);
        canHangAgain = true;
    }








    public void CharacterMove()
    {
        Vector3 m_UnitGoal = ((cameraTransform.forward * vertical + cameraTransform.right * horizontal)).normalized;
        m_UnitGoal.y = 0;



        Vector3 unitVel = _m_GoalVel.normalized;


        float velDot = Vector3.Dot(m_UnitGoal, unitVel);


        float accel = _acceleration * _accelerationFactorFromDot.Evaluate(velDot);

        Vector3 goalVel = m_UnitGoal * _maxSpeed * _speedFactor;
        //Vector3 otherVel = Vector3.zero;
        //Rigidbody hitBody = rayHit.rigidbody;

        _m_GoalVel = Vector3.MoveTowards(_m_GoalVel, goalVel, accel * Time.fixedDeltaTime);

        Vector3 neededAccel = (_m_GoalVel - _rb.velocity) / Time.fixedDeltaTime;
        float maxAccel = _maxAccelForce * _maxAccelerationForceFactorFromDot.Evaluate(velDot) * _maxAccelForceFactor;
        neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);
        _rb.AddForceAtPosition(Vector3.Scale(neededAccel * _rb.mass, _moveForceScale), transform.position + new Vector3(0f, transform.localScale.y * _leanFactor, 0f)); // Using AddForceAtPosition in order to both move the player and cause the play to lean in the direction of input.

        
    }



    void inputManager()
    {
        horizontal = move.x * isActive;
        vertical = move.y * isActive;
        jumping();
        
        //else jumpStrength = jumpScale * 0;
    }


    void calculateAim()
    {
        //Enable this script to hit desired target...this isn't working with the crosshair type stuff because the character faces the target.
        //lookat videos to see how GTA does shooting like this

        /* Vector3 relPos = (target.position - transform.position).normalized;
         Quaternion targetAim = Quaternion.LookRotation(relPos);
         Quaternion angles = Quaternion.Euler(0, targetAim.eulerAngles.y, 0);
         transform.rotation = Quaternion.Slerp(transform.rotation, angles, 3 * Time.deltaTime);
         localEquilibriumRotation = targetAim;
        localEquilibriumRotation = targetAim;
        */


        if (isActive == 1)
        {
            //localEquilibriumRotation = Quaternion.LookRotation(-cameraTransform.forward);
            joint.SetTargetRotationLocal(Quaternion.LookRotation(cameraTransform.forward), Quaternion.Euler(0, 0 + jointRotationOffset, 0));

        }
        

    }

    

    

    public void jumping()
    {
        
        //int layerMask = 1 << 12;
        //layerMask = ~layerMask;
        jumpGrounded = Physics.Raycast(transform.position, -transform.up, jumpGroundCheckDistance, GroundLayer);
        if (jumpGrounded && jumpPressed && !isJumping)
        {
            _rb.AddForce(transform.up * jumpScale, ForceMode.VelocityChange);
            isJumping = true;
            //jumpStrength = jumpScale * isActive;
        }
        else if (jumpGrounded && isJumping && !jumpPressed)
        {
            
            isJumping = false;
        }
    }

    public void jointRotation()
    {
        Vector3 direction = new Vector3(horizontal, 0, vertical);
        if (direction.magnitude > 0)
        {
            targetAngle = Mathf.Atan2(horizontal, vertical) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            
            if (masterInputRef.slide)
            {
                joint.SetTargetRotationLocal(Quaternion.Euler(-60, targetAngle, 0), Quaternion.Euler(0, 0 + jointRotationOffset, 0));
            }
            else
            {
                joint.SetTargetRotationLocal(Quaternion.Euler(0, targetAngle, 0), Quaternion.Euler(0, 0 + jointRotationOffset, 0));
            }

            if (masterInputRef.jump && transform.root.gameObject.TryGetComponent(out Flight flight))
            {
                //float flightAngle = Mathf.Atan2(horizontal, 0) * Mathf.Rad2Deg;
                //Debug.Log(flightAngle);

                joint.SetTargetRotationLocal(Quaternion.Euler(ExtensionMethods.remap(masterInputRef.move.y, 1, -1, -45, -135), targetAngle, 0), Quaternion.Euler(0, 0 + jointRotationOffset, 0));
            }

            
        }

        if (masterInputRef.punch)
        {
            RotationAttack();
        }


        //else
        //{
        //    joint.SetTargetRotationLocal(joint.targetRotation, Quaternion.Euler(0, 180, 0));
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isMultiplayer)
        {


            if (collision.gameObject.tag == "BossFoot")
            {
                puppetMaster.Kill();
                JointDrive jointXDrive = joint.angularXDrive;
                JointDrive jointYZDrive = joint.angularYZDrive;
                jointXDrive.positionSpring = 0;
                jointYZDrive.positionSpring = 0;
                Invoke(nameof(puppetRevive), 2);
            }
        }

    }

    public void puppetRevive()
    {
        puppetMaster.Resurrect();
        JointDrive jointXDrive = joint.angularXDrive;
        JointDrive jointYZDrive = joint.angularYZDrive;
        jointXDrive.positionSpring = 2000;
        jointYZDrive.positionSpring = 2000;
    }

    void RotationAttack()
    {

        //spinAngle += 980 * Time.deltaTime;

        //if (spinAngle > 980)
        //{
        //    spinAngle = 0;
        //}


        //joint.SetTargetRotationLocal(Quaternion.Euler(0, transform.eulerAngles.y + spinAngle, 0), Quaternion.Euler(0, 0 + transform.eulerAngles.y, 0));
        //isSpinning = true;

        //Invoke("StopSpin", 3);
        joint.SetTargetRotationLocal(Quaternion.Euler(75, targetAngle, 0), Quaternion.Euler(0, 0 + jointRotationOffset, 0));
    }

    void StopSpin()
    {
        isSpinning = false;

        
    }


    bool EdgeClimb()
    {
        

        

        Debug.DrawLine(headPosition.position, headPosition.position + -headPosition.forward * 2f);
        Debug.DrawLine(headPosition.position, headPosition.position + headPosition.forward * 2f);



        if (!jumpGrounded)
        {
            if (Physics.Raycast(headPosition.position, headPosition.forward, out frontHit, 5f, ClimbLayer))
            {
                //Debug.Log("ClimbInFront");
                //if (Physics.Raycast(firstHit.point + (-headPosition.forward * playerRadius) + (Vector3.up * .6f * playerHeight), Vector3.down, out var secondHit, playerHeight))
                //{
                //    landPoint = secondHit.point;
                //    Debug.Log("PlaceToLand");

                //    return true;
                //}

                

                //if (frontHit.collider == null)
                //{
                //    landPoint = backHit.point;
                //    Debug.Log(backHit.point);
                //}
                
                    landPoint = frontHit.point;
                    //Debug.Log(frontHit.point);
                if(frontHit.collider.gameObject.layer != LayerMask.GetMask("mainPlayer") || frontHit.collider.gameObject.layer != LayerMask.GetMask("Blue")
                    || frontHit.collider.gameObject.layer != LayerMask.GetMask("Red") || frontHit.collider.gameObject.layer != LayerMask.GetMask("White"))
                {
                    return true;
                }
                else
                {
                    return false;
                }

                
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(headPosition.position + -headPosition.forward * .5f, 1f);
    }

    



}