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
        if (interractingPlayer)
        {
            LaunchPlayer(interractingPlayer);
        }
    }


    protected override void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<Player>() == interractingPlayer)
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
        interractingPlayer.CharacterInputs.Enable();
        interractingPlayer.AttachedMotor.onMotorGrounded -= ReEnableControl;
        interractingPlayer = null;
    }

    


}
