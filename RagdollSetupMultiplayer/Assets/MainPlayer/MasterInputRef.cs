using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MasterInputRef : MonoBehaviour
{
    public PlayerControls playerControls;
    

    public Vector2 move;
    public bool jump = false;
    public bool shoot;
    public bool aim;

    public bool slide;
    public bool punch;
    

    public bool pause;

    public bool speedBoost;


    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Enable();

        //playerControls.Player.Jump.started += onJump;
        //playerControls.Player.Jump.canceled += onJump;

        
    }

    // Update is called once per frame
    void Update()
    {
        //move = playerControls.Player.Move.ReadValue<Vector2>();
        //jump = playerControls.Player.Jump.IsPressed();
        //shoot = playerControls.Player.Shoot.IsPressed();
        //aim = playerControls.Player.Aim.IsPressed();

        //punch = playerControls.Player.Punch.IsPressed();
        //slide = playerControls.Player.Slide.IsPressed();


        pause = playerControls.Player.Pause.WasPerformedThisFrame();
        //aim = Input.GetMouseButton(0);
        
        

    }

    public void onJump(InputAction.CallbackContext context)
    {
        jump = context.ReadValueAsButton();
        
    }

    public void onMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void onAim(InputAction.CallbackContext context)
    {
        aim = context.ReadValueAsButton();
    }

    public void onSlide(InputAction.CallbackContext context)
    {
        slide = context.ReadValueAsButton();
        
    }

    public void onPunch(InputAction.CallbackContext context)
    {
        punch = context.ReadValueAsButton();
        
    }

    public void onShoot(InputAction.CallbackContext context)
    {
        shoot = context.ReadValueAsButton();
    }

    public void onSpeedBoost(InputAction.CallbackContext context)
    {
        speedBoost = context.ReadValueAsButton();
    }


}
