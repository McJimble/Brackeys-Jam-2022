using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Corpse : MonoBehaviour, IInteractor
{
    [Tooltip("If this is less than zero, it will never be destroyed automatically.")]
    [SerializeField] private float autoDestroyAfterTime = 8f;

    public bool CanInteract => true;

    public bool CanInterchangeParents => true;

    public Transform InteractingTransform => transform;

    public Rigidbody InteractingRB => rb;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (autoDestroyAfterTime >= 0)
        {
            Destroy(gameObject, autoDestroyAfterTime);
        }
    }
}
