using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected bool automaticallyTrackPlayers = true;
    [SerializeField] protected bool requiresKeyPress = true;
    [SerializeField] protected Outline outlineEffect;
    [Space]
    [SerializeField] protected UnityEvent onEnterRadius;
    [SerializeField] protected UnityEvent onExitRadius;
    [SerializeField] protected UnityEvent onInterract;  // Call this yourself based on needs of the inheriting class! (not called in base.Interact())

    protected Player interactingPlayer = null;
    protected bool canInterract = true;

    protected Collider genericTriggerBounds;

    protected HashSet<IInteractor> currentlyInteractingObjects = new HashSet<IInteractor>();

    public bool RequiredKeyPress { get => requiresKeyPress; }
    public bool CurrentlyInteracting { get; protected set; }
    public bool CanBeInteracted { get; protected set; }
    public Rigidbody AttachedRB { get; protected set; } // Can be null!


    protected virtual void Awake()
    {
        CurrentlyInteracting = false;
        CanBeInteracted = true;
        AttachedRB = GetComponent<Rigidbody>();
        genericTriggerBounds = GetComponent<Collider>();
        outlineEffect = (outlineEffect) ? outlineEffect : GetComponent<Outline>();

        currentlyInteractingObjects.Clear();
    }

    public virtual bool FulfillsInteractionsParams(IInteractor interactor)
    {
        // This is disgusting but:
        // Fulfills interaction if: interactor can interact, this can be interacted with, and this is not interacting right now.
        return (interactor.CanInteract && this.CanBeInteracted && !this.CurrentlyInteracting);
    }

    public virtual bool TryInteract(IInteractor interactor)
    {
        if (!FulfillsInteractionsParams(interactor)) return false;
        // Had other stuff here before so this is stupid right now.
        return true;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        // If an interractor exists and can interract, invoke event.
        IInteractor otherInteractor = other as IInteractor;
        if (otherInteractor != null && otherInteractor.CanInteract) return;

        currentlyInteractingObjects.Add(otherInteractor);
        onEnterRadius.Invoke();

        if (!automaticallyTrackPlayers) return;

        // Check for player reference; may need it for player specific stuff.
        Player player;
        if (other.TryGetComponent<Player>(out player))
        {
            interactingPlayer = player;
        }
        
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        // If other interactor exists in our table that we build on enter, then remove it now.
        IInteractor otherInteractor = other as IInteractor;
        if (otherInteractor != null && !currentlyInteractingObjects.Contains(otherInteractor)) return;

        currentlyInteractingObjects.Remove(otherInteractor);
        onExitRadius.Invoke();

        if (!automaticallyTrackPlayers) return;

        Player player;
        if (other.TryGetComponent<Player>(out player) && player == interactingPlayer)
        {
            interactingPlayer = null;
        }
    }

    public virtual void ToggleInteractAvailableEffect(bool enable)
    {
        if (outlineEffect)
        {
            outlineEffect.enabled = enable;
        }
    }

    public void DebugInterract()
    {
        Debug.Log("Interacted with: " + gameObject.name);
    }
}
