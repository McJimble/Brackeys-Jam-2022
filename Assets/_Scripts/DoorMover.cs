using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class DoorMover : Interactable
{
    public enum DoorState
    {
        open, closed
    }
    
    [SerializeField] private bool openOnStart = false;
    [SerializeField] private Transform doorToMove;
    [SerializeField] private float moveAmount = .5f;
    [SerializeField] private AnimationCurve openDoorCurve;
    [SerializeField] private AnimationCurve closeDoorCurve;
    DoorState doorState = DoorState.closed;
    Vector3 originalDoorPostion;
    Vector3 endPosition;
    bool movedPlayer = false;
  

    private void Start()
    {
        if (openOnStart)
        {
            StartCoroutine(OpenLerpDoor());
        }
        originalDoorPostion = doorToMove.position;
        endPosition = doorToMove.position + (doorToMove.up * moveAmount);
    }




    public IEnumerator OpenLerpDoor()
    {
        doorState = DoorState.open;
        float timeElapsed = 0;
        float openDoorTime = openDoorCurve.keys[openDoorCurve.length - 1].time;
        while (timeElapsed <= openDoorTime)
        {
            doorToMove.position = Vector3.Lerp(doorToMove.position, endPosition, openDoorCurve.Evaluate(timeElapsed));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        doorToMove.position = endPosition;
       
    }

    public IEnumerator CloseLerpDoor()
    {
       
        float timeElapsed = 0;
        float closeDoorTime = closeDoorCurve.keys[closeDoorCurve.length - 1].time;
        while (timeElapsed <= closeDoorTime)
        {
            doorToMove.position = Vector3.Lerp(endPosition, originalDoorPostion, closeDoorCurve.Evaluate(timeElapsed));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        doorToMove.position = originalDoorPostion;
        doorState = DoorState.closed;
    }

   

    public void TriggerDoor()
    {
      
        if (doorState == DoorState.open)
        {
            StartCoroutine(CloseLerpDoor());
        }
        else
        {
            StartCoroutine(OpenLerpDoor());
        }
    }

    public void MovePlayerAtStart(Player player)
    {
        if(movedPlayer == false)
        {
            StartCoroutine(MovePlayer(player));
            movedPlayer = true;
        }
    }

    private IEnumerator MovePlayer(Player player)
    {
        float timeElapsed = 0f;
        yield return new WaitForSeconds(.75f);
        while (timeElapsed < .5f)
        {

            player.transform.position = Vector3.Lerp(player.SpawnPoint.position, player.ShovePoint.position, timeElapsed / .5f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }


}
