using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IInteractor
{
    [SerializeField] float movementSpeed = 10f;
    [SerializeField] public Vector2 movementInput;
    [SerializeField] Transform spawnPoint;
    [SerializeField] Transform shoveToPoint;
    [SerializeField] AudioClip jumpSFX;

    private AudioSource audioSource;
    private CharacterInputs characterInputs;
    private CharacterMotor characterMotor;
    private Transform cameraObject;
    private bool faceMovementDirection = true;

    public static event System.Action<Player> OnPlayerDeath;

    [SerializeField] private ParticleSystem deathParticle;
    [SerializeField] private List<Interactable> inRangeInteractables;
    [SerializeField] private BasePickup heldPickup;
    private BehaviourDistanceComparerer distanceComparerer;

    // ---- IInteractor stuff ----
    bool IInteractor.CanInteract { get => true; }
    Transform IInteractor.InteractingTransform { get => transform; }
    Rigidbody IInteractor.InteractingRB { get => AttachedMotor.AttachedRB; }
    bool IInteractor.CanInterchangeParents => true;
    // ----------------------------
    public CharacterInputs CharacterInputs { get => characterInputs; }
    public Transform SpawnPoint { get => spawnPoint; }
    public Transform ShovePoint { get => shoveToPoint; }
    public ParticleSystem DeathParticle { get => deathParticle; }

    public CharacterMotor AttachedMotor { get => characterMotor; }
    public bool FaceMovementDirection { get => faceMovementDirection; set => faceMovementDirection = value; }


    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        audioSource = GetComponent<AudioSource>();
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
        characterMotor.AttachedRB.velocity = Vector3.zero;
        movementInput = Vector2.zero;
    }

    private void Update()
    {
        if (characterInputs.Player.Actions.triggered)
        {
            if (characterMotor.StartJump())
            {
                audioSource.clip = jumpSFX;
                audioSource.Play();
            }

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
            && interactableObj.FulfillsInitialInteractionsParams(this)
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

    public bool AddInRangeInteractableOverride(Interactable toAdd)
    {
        if (inRangeInteractables.Contains(toAdd)) return false;

        inRangeInteractables.Add(toAdd);
        inRangeInteractables.Sort(distanceComparerer);
        inRangeInteractables[0].ToggleInteractAvailableEffect(true);

        return true;
    }

    public bool RemoveInRangeInteractableOverride(Interactable toRem)
    {
        if (!inRangeInteractables.Remove(toRem)) return false;

        inRangeInteractables.Sort(distanceComparerer);
        toRem.ToggleInteractAvailableEffect(false);

        return true;
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

    public void KillPlayer()
    {
        DeathParticle.transform.position = transform.position;
        DeathParticle.Play();
        ExplosionSFX.Instance.PlayExplosion();
        CinemachineShake.Instance.ShakeCamera(10f, .7f);
        LevelManager.Instance.RespawnPlayer(this);

        OnPlayerDeath?.Invoke(this);
    }
}
