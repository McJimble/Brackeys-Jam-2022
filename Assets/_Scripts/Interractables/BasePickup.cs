using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BasePickup : Interactable, IInteractor
{
    [SerializeField] private float distanceFromHolder = 0.5f;
    [SerializeField] private SphereCollider pickupRangeTrigger;
    [SerializeField] private Collider mainPhysicsCollider;
    public bool CanInterchangeParents => true;

    public bool CanInteract => interactingPlayer == null;
    public Transform InteractingTransform => transform;
    public Rigidbody InteractingRB => AttachedRB;

    public Collider InteractingCollider => mainPhysicsCollider;

    protected override void Awake()
    {
        base.Awake();
        automaticallyTrackPlayers = false;

        if (pickupRangeTrigger == null)
        {
            Debug.LogWarning("Pickup range trigger not found. Some pickup interactions may break.");
        }

        mainPhysicsCollider = (mainPhysicsCollider) ? mainPhysicsCollider : GetComponent<Collider>();
    }

    public override void HandlePlayerDeath(Player player)
    {
        if (interactingPlayer && interactingPlayer == player)
        {
            AttachedRB.velocity = interactingPlayer.AttachedMotor.AttachedRB.velocity;
            AttachedRB.detectCollisions = true;
            AttachedRB.useGravity = true;
            CurrentlyInteracting = false;
            CanBeInteracted = true;
            interactingPlayer = null;
        }
    }

    public override bool TryInteract(IInteractor interactor)
    {
        Player player = interactor as Player;
        if (!player) return false;

        // If player interacted and already is interacting, drop cube.
        // else, we are picking it up.
        if (interactingPlayer)
        {
            AttachedRB.velocity = interactingPlayer.AttachedMotor.AttachedRB.velocity;
            interactingPlayer = null;

            // If pickup still overlaps with player after dropping, still allow them to pick it up.
            int overlaps = Physics.OverlapSphereNonAlloc(transform.position, pickupRangeTrigger.radius, PhysicsUtils.NonAllocCollisionCasts, PhysicsUtils.Masks.player);
            if (overlaps > 0)
            {
                player.AddInRangeInteractableOverride(this);
                currentlyInteractingObjects.Add(player);
            }

            AttachedRB.detectCollisions = true;
            AttachedRB.useGravity = true;
            CurrentlyInteracting = false;
            CanBeInteracted = true;
        }
        else
        {
            interactingPlayer = player;
            AttachedRB.detectCollisions = false;
            AttachedRB.useGravity = false;
            AttachedRB.velocity = Vector3.zero;
            CurrentlyInteracting = true;
            CanBeInteracted = false;
            InvokeInteract(interactor);
        }

        return true;
    }

    protected override void Update()
    {
        if (interactingPlayer && CurrentlyInteracting)
        {
            transform.position = interactingPlayer.transform.position + (interactingPlayer.transform.forward * distanceFromHolder);
        }
    }
}
