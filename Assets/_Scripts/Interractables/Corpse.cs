using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Corpse : MonoBehaviour, IInteractor
{
    [Tooltip("If this is less than zero, it will never be destroyed automatically.")]
    [SerializeField] private float autoDestroyAfterTime = 8f;
    [SerializeField] private Collider mainPhysicsCollider;

    public bool CanInteract => true;

    public bool CanInterchangeParents => !doomed;

    public Transform InteractingTransform => transform;

    public Rigidbody InteractingRB => rb;

    public Collider InteractingCollider => mainPhysicsCollider;

    private HashSet<Interactable> interactingWith = new HashSet<Interactable>();
    private Rigidbody rb;
    private bool doomed = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainPhysicsCollider = (mainPhysicsCollider) ? mainPhysicsCollider : GetComponent<Collider>();
    }

    private void Start()
    {
        if (autoDestroyAfterTime >= 0)
        {
            Destroy(gameObject, autoDestroyAfterTime);
        }
    }

    private void OnDestroy()
    {
        doomed = true;
        foreach (var interactable in interactingWith)
        {
            interactable.SpecialOnTriggerExit(this);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Interactable interactable;
        if (other.TryGetComponent(out interactable) && !interactingWith.Contains(interactable))
        {
            interactingWith.Add(interactable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Interactable interactable;
        if (other.TryGetComponent(out interactable) && interactingWith.Contains(interactable))
        {
            interactingWith.Remove(interactable);
        }
    }
}
