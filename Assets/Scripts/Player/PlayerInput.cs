using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public event Action OnLeftMouseClick, OnRightMouseClick, OnFly, OnInventoryButtonPressed, OnRightMouseUp;
    public event Action<int> OnHotbarSelection;
    public bool RunningPressed { get; private set; }
    public Vector3 MovementInput { get; private set; }
    public Vector2 MousePosition { get; private set; }
    public bool IsJumping { get; private set; }
    public bool inventoryOpen = false;
    void Update()
    {
        if(!inventoryOpen){
            GetMouseClick();
            GetMousePosition();
            GetMovementInput();
            GetJumpInput();
            GetRunInput();
            GetFlyInput();
        } else {
            MovementInput = new Vector3(0,0,0);
            MousePosition = new Vector2(0,0);
        }
        GetInventoryButtonPressed();
        
    }

    private void GetFlyInput()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            OnFly?.Invoke();
        }
    }

    private void GetRunInput()
    {
        RunningPressed = Input.GetKey(KeyCode.LeftShift);
    }

    private void GetJumpInput()
    {
        IsJumping = Input.GetButton("Jump");
    }
    private void GetMovementInput()
    {
        MovementInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    }

    private void GetMousePosition()
    {
        MousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    private void GetMouseClick()
    {
        if (Input.GetMouseButton(0))
        {
            OnLeftMouseClick?.Invoke();
        }
        if (Input.GetMouseButtonDown(1))
        {
            OnRightMouseClick?.Invoke();
        }
        if (Input.GetMouseButtonUp(0)){
            OnRightMouseUp?.Invoke();
        }
    }
    private void GetInventoryButtonPressed(){
        if(Input.GetKeyDown(KeyCode.E)){
            OnInventoryButtonPressed.Invoke();
        }
    }
}
