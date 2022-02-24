using UnityEngine;

public interface IInteractor
{
    public bool CanInteract { get; }
    public bool CanInterchangeParents { get; }

    // The follow can be null

    public Transform InteractingTransform { get; }  
    public Rigidbody InteractingRB { get; }         
}
