using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IInteractor
{
    [SerializeField] float movementSpeed = 10f;
    [SerializeField] public Vector2 movementInput;
    [SerializeField] Transform spawnPoint;
    [SerializeField] Transform shoveToPoint;

    private CharacterInputs characterInputs;
    private CharacterMotor characterMotor;
    private Transform cameraObject;
    private bool interractAction = false;

    [SerializeField] private List<Interactable> inRangeInteractables;
    private BehaviourDistanceComparerer distanceComparerer;

    bool IInteractor.CanInteract { get => characterInputs.Player.Interact.triggered; }

    public CharacterInputs CharacterInputs { get => characterInputs; }
    public Transform SpawnPoint { get => spawnPoint; }
    public Transform ShovePoint { get => shoveToPoint;  }

    public CharacterMotor AttachedMotor { get => characterMotor; }
   
    private void Awake()
    {
        
        distanceComparerer = new BehaviourDistanceComparerer(transform);
        inRangeInteractables.Clear();

        characterMotor = GetComponent<CharacterMotor>();
        cameraObject = Camera.main.transform;
    }

    private void OnEnable()
    {
        characterInputs = new CharacterInputs();
        characterInputs.Player.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
        characterInputs.Player.Interact.performed += i => AttemptInterract();
        
        characterInputs.Enable();
    }
    private void OnDisable()
    {
        
        characterInputs.Disable();
        characterMotor.SetMoveVelocity(Vector3.zero);
        movementInput = Vector2.zero;
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

    private void OnTriggerEnter(Collider other)
    {
        Interactable interactableObj;
        if (other.TryGetComponent(out interactableObj) && interactableObj.RequiredKeyPress && !inRangeInteractables.Contains(interactableObj))
        {
            inRangeInteractables.Add(interactableObj);
            inRangeInteractables.Sort(distanceComparerer);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Interactable interactableObj;
        if (other.TryGetComponent(out interactableObj))
        {
            inRangeInteractables.Remove(interactableObj);
            inRangeInteractables.Sort(distanceComparerer);
        }
    }

    private void AttemptInterract()
    {
        if (inRangeInteractables.Count <= 0) return;
        foreach (var interactble in inRangeInteractables)
        {
            interactble.TryInterract(this);
        }
    }

    public Vector3 SetMovementFromInput()
    {
        Vector3 moveDirection = cameraObject.forward * movementInput.y;
        moveDirection = moveDirection + cameraObject.right * movementInput.x;
        moveDirection.Normalize();
        moveDirection.y = 0;
        return moveDirection * movementSpeed;
    }
    public void ClearMovementInput()
    {
        movementInput = Vector2.zero;
    }
}
