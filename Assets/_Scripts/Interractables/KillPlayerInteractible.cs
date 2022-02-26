using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayerInteractible : Interactable
{
    protected override void Awake()
    {
        base.Awake();
        automaticallyTrackPlayers = true;   // Doesn't work if this is false.
    }

    public override bool TryInteract(IInteractor interactor)
    {
        if (!base.TryInteract(interactor)) return false;

        return true;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (interactingPlayer)
        {
            currentlyInteractingObjects.Remove(interactingPlayer);
            interactingPlayer.KillPlayer();
            interactingPlayer = null;
        }
    }

    
}
