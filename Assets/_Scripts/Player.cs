using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour, IInteractor
{
    [SerializeField] float movementSpeed = 10f;
    [SerializeField] public Vector2 movementInput;
    [SerializeField] Transform spawnPoint;
    [SerializeField] Transform shoveToPoint;
    [SerializeField] AudioClip jumpSFX;
    [Space]
    [SerializeField] private GameObject walkingMesh;
    [SerializeField] private GameObject jumpingMesh;
    [SerializeField] private GameObject corpseOnDeath;
    [Space]
    private Animator animator;
    private int walkingParamHash = Animator.StringToHash("isWalking");
    private int groundedParamHash = Animator.StringToHash("isGrounded");

    private AudioSource audioSource;
    private CharacterInputs characterInputs;
    private CharacterMotor characterMotor;
    private Transform cameraObject;
    private float panicTimer = 0;
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

    public Collider InteractingCollider => AttachedMotor.Capsule;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
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

        AttachedMotor.onMotorGrounded += ResetAnimationOnGrounded;
    }
    private void OnDisable()
    {
        characterInputs.Disable();
        characterMotor.AttachedRB.velocity = Vector3.zero;
        movementInput = Vector2.zero;

        foreach (var inter in inRangeInteractables)
        {
            inter.SpecialOnTriggerExit(this);
        }
        inRangeInteractables.Clear();

    }

    private void Update()
    {
        if (characterInputs.Player.Actions.triggered)
        {

            if (characterMotor.StartJump())
            {
                audioSource.clip = jumpSFX;
                audioSource.Play();
                jumpingMesh.SetActive(true);
                walkingMesh.SetActive(false);
            }
        }

        CheckPanic();

        if (faceMovementDirection && movementInput != Vector2.zero)
        {
            transform.LookAt(transform.position + SetMovementFromInput().normalized, Vector3.up);
            animator.SetBool(walkingParamHash, true);
        }
        else
            animator.SetBool(walkingParamHash, false);

        animator.SetBool(groundedParamHash, AttachedMotor.IsGrounded);
    }

    private void ResetAnimationOnGrounded()
    {
        jumpingMesh.SetActive(false);
        walkingMesh.SetActive(true);
    }

    private void CheckPanic()
    {
        bool isRKeyHeld = characterInputs.Player.Panic.ReadValue<float>() > 0.1f;

        if (isRKeyHeld)
        {
            panicTimer += Time.deltaTime;
            CinemachineShake.Instance.ShakeCamera(2f, .1f);
            if (panicTimer > 1.5f)
            {
                KillPlayer();
                panicTimer = 0;
            }
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

        if (LevelManager.Instance.LevelNumber == 7)
            moveDirection *= -1;

        return moveDirection * movementSpeed;
    }

    public void ClearMovementInput()
    {
        movementInput = Vector2.zero;
    }

    public void KillPlayer()
    {
        transform.parent = null;
        inRangeInteractables.Remove(heldPickup);
        heldPickup = null;
        DeathParticle.transform.position = transform.position;
        DeathParticle.Play();
        ExplosionSFX.Instance.PlayExplosion();
        CinemachineShake.Instance.ShakeCamera(10f, .7f);

        GameObject corpse = Instantiate(corpseOnDeath, walkingMesh.transform.position, walkingMesh.transform.rotation, null);
        Corpse corpseComp;
        if (corpse.TryGetComponent(out corpseComp))
        {
            // Do extra stuff if necessary w/ corpse here.
            corpseComp.InteractingRB.velocity = AttachedMotor.AttachedRB.velocity;
        }

        // Shouldn't have to do this but SOMEONE made the UI scripts edit the deaths and timer
        // so I sorta have to do this.
        OnScreenUI ui = FindObjectOfType<OnScreenUI>();
        if (ui)
        {
            ui.IncreaseDeathCounter();
        }

        OnPlayerDeath?.Invoke(this);
        LevelManager.Instance.RespawnPlayer(this);
    }
}
