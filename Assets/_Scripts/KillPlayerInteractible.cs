using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayerInteractible : Interactable
{
    public override bool TryInterract(IInteractor interactor)
    {
        if (!base.TryInterract(interactor)) return false;


        return true;
    }

  

    public IEnumerator RespawnPlayer(Player player)
    {
        float timeElapsed = 0;
        player.gameObject.SetActive(false);
        
        yield return new WaitForSeconds(1f);
        player.transform.position = player.SpawnPoint.position;

        player.gameObject.SetActive(true);
        player.AttachedMotor.AttachedRB.velocity = Vector3.zero;
        player.CharacterInputs.Disable();
       

        yield return new WaitForSeconds(.75f);
        while (timeElapsed < .5f)
        {
            
            player.transform.position = Vector3.Lerp(player.SpawnPoint.position, player.ShovePoint.position, timeElapsed / .5f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        player.transform.position = player.ShovePoint.position;
        player.CharacterInputs.Enable();
        
    }

    public void KillPlayer(Player player)
    {
        StartCoroutine(RespawnPlayer(player));
    }
}
