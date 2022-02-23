using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BasePickup : Interactable, IInteractor
{
    [SerializeField] private float distanceFromHolder = 0.5f;
    public bool CanInteract => interactingPlayer != null;

    protected override void Awake()
    {
        base.Awake();
        automaticallyTrackPlayers = false;
    }

    public override bool TryInteract(IInteractor interactor)
    {
        Player player = interactor as Player;
        if (!player) return false;

        if (interactingPlayer)
        {
            interactingPlayer = null;
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
            CurrentlyInteracting = true;
            CanBeInteracted = false;
            onInterract.Invoke();
        }

        return true;
    }

    protected virtual void Update()
    {
        if (interactingPlayer && CurrentlyInteracting)
        {
            transform.position = interactingPlayer.transform.position + (interactingPlayer.transform.forward * distanceFromHolder);
        }
    }
}
