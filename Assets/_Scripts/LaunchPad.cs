using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPad : Interactable
{
    [Range(10,100)]
    [SerializeField] float launchForce = 10f;
    [SerializeField] Transform forceDirection;


    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (interactingPlayer)
        {
            LaunchPlayer(interactingPlayer);
        }
    }


    protected override void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<Player>() == interactingPlayer)
        {
           
        }
    }


    public void LaunchPlayer(Player player)
    {
        player.AttachedMotor.AttachedRB.AddForce(launchForce * forceDirection.up, ForceMode.Impulse);
        player.CharacterInputs.Disable();
        player.ClearMovementInput();
        player.AttachedMotor.onMotorGrounded += ReEnableControl;
    }

   



    private void ReEnableControl()
    {
        interactingPlayer.CharacterInputs.Enable();
        interactingPlayer.AttachedMotor.onMotorGrounded -= ReEnableControl;
        interactingPlayer = null;
    }

    


}
