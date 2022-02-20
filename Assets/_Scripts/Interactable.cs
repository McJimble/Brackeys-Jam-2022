using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public abstract class Interractable
{
    [SerializeField] private UnityEvent onEnterRadius;
    [SerializeField] private UnityEvent onInterract;

    //private Player interractingPlayer;

    protected virtual void Awake()
    {

    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        // If get component player, call on enter radius event.
    }
}
