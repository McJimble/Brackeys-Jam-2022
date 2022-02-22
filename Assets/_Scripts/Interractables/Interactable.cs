using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public abstract class Interactable : MonoBehaviour
{ 
    [SerializeField] protected bool requiresKeyPress = true;
    [SerializeField] protected UnityEvent onEnterRadius;
    [SerializeField] protected UnityEvent onExitRadius;
    [SerializeField] protected UnityEvent onInterract;  // Call this yourself based on needs of the inheriting class! (not called in base.Interact())

    protected Player interractingPlayer = null;
    protected bool canInterract = true;

    protected Collider genericTriggerBounds;

    public bool RequiredKeyPress { get => requiresKeyPress; }
    public bool CurrentlyInteracting { get; protected set; }
    public bool CanBeInteracted { get; protected set; }

    protected virtual void Awake()
    {
        CurrentlyInteracting = false;
        CanBeInteracted = true;
        genericTriggerBounds = GetComponent<Collider>();
    }

    public virtual bool TryInterract(IInteractor interactor)
    {
        if (!interactor.CanInteract || !this.CanBeInteracted || CurrentlyInteracting) return false;
        Debug.Log("Interracted with " + gameObject.name);
        return true;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        Player player;
        if (other.TryGetComponent<Player>(out player))
        {
            interractingPlayer = player;
            onEnterRadius.Invoke();
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        Player player;
        if (other.TryGetComponent<Player>(out player) && player == interractingPlayer)
        {
            onExitRadius.Invoke();
            interractingPlayer = null;
        }
    }

    public void DebugInterract()
    {
        Debug.Log("Interacted with: " + gameObject.name);
    }
}
