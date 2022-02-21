using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float movementSpeed = 10f;

    CharacterInputs characterInputs;
    CharacterMotor characterMotor;
    Transform cameraObject;

    public  Vector2 movementInput;




    private void Awake()
    {
        characterMotor = GetComponent<CharacterMotor>();
        cameraObject = Camera.main.transform;
       
    }

    private void OnEnable()
    {
        characterInputs = new CharacterInputs();
        characterInputs.Player.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
        
        characterInputs.Enable();
    }
    private void OnDisable()
    {
        characterInputs.Disable();
    }

    private void Update()
    {
        if (characterInputs.Player.Actions.triggered)
        {
            characterMotor.StartJump();
        }
    }

    private void FixedUpdate()
    {
        characterMotor.SetMoveVelocity(SetMovementFromInput());
       
    }

    public Vector3 SetMovementFromInput()
    {
        Vector3 moveDirection = cameraObject.forward * movementInput.y;
        moveDirection = moveDirection + cameraObject.right * movementInput.x;
        moveDirection.Normalize();
        moveDirection.y = 0;
        return moveDirection * movementSpeed;
    }
}
