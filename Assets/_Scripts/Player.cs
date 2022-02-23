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
    private bool faceMovementDirection = true;

    [SerializeField] private List<Interactable> inRangeInteractables;
    [SerializeField] private BasePickup heldPickup;
    private BehaviourDistanceComparerer distanceComparerer;

    bool IInteractor.CanInteract { get => true; }

    public CharacterInputs CharacterInputs { get => characterInputs; }
    public Transform SpawnPoint { get => spawnPoint; }
    public Transform ShovePoint { get => shoveToPoint;  }

    public CharacterMotor AttachedMotor { get => characterMotor; }
    public bool FaceMovementDirection { get => faceMovementDirection; set => faceMovementDirection = value; }
   
    private void Awake()
    {
        distanceComparerer = new BehaviourDistanceComparerer(transform);
        inRangeInteractables = new List<Interactable>();

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

        if (faceMovementDirection && movementInput != Vector2.zero)
        {
            transform.LookAt(transform.position + SetMovementFromInput().normalized, Vector3.up);
        }
    }

    private void FixedUpdate()
    {
        characterMotor.SetMoveVelocity(SetMovementFromInput());
    }

    private void OnTriggerEnter(Collider other)
    {
        Interactable interactableObj;
        if (other.TryGetComponent(out interactableObj)
            && interactableObj.RequiredKeyPress
            && interactableObj.FulfillsInteractionsParams(this)
            && !inRangeInteractables.Contains(interactableObj))
        {
            inRangeInteractables.Add(interactableObj);
            inRangeInteractables.Sort(distanceComparerer);
            inRangeInteractables[0].ToggleInteractAvailableEffect(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Interactable interactableObj;
        if (other.TryGetComponent(out interactableObj))
        {
            inRangeInteractables.Remove(interactableObj);
            inRangeInteractables.Sort(distanceComparerer);
            interactableObj.ToggleInteractAvailableEffect(false);
        }
    }

    private void AttemptInterract()
    {
        // We treat pickups differently since they have to be dropped at some point.
        if (heldPickup && heldPickup.TryInteract(this))
        {
            heldPickup = null;
        }

        for (int i = 0; i < inRangeInteractables.Count; i++)
        {
            if (inRangeInteractables[i].TryInteract(this) && inRangeInteractables[i] != heldPickup)
            {
                BasePickup possiblePickup = inRangeInteractables[i] as BasePickup;
                if (possiblePickup && heldPickup == null)
                    heldPickup = possiblePickup;

                break;
            }
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
