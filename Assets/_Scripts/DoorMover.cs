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
    [SerializeField] private AudioClip openDoorSFX;
    [SerializeField] private AudioClip closeDoorSFX;
    AudioSource audioSource;

    DoorState doorState = DoorState.closed;
    Vector3 originalDoorPostion;
    Vector3 endPosition;
    Vector3 gizmoSize = new Vector3(2f, .5f, 2f);
    bool movedPlayer = false;

    
       

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (openOnStart)
        {
            StartCoroutine(OpenLerpDoor());
        }
        originalDoorPostion = doorToMove.position;
        endPosition = doorToMove.position + (doorToMove.up * moveAmount);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, gizmoSize);

    }


    public IEnumerator OpenLerpDoor()
    {
        doorState = DoorState.open;
        float timeElapsed = 0;
        float openDoorTime = openDoorCurve.keys[openDoorCurve.length - 1].time;
        audioSource.clip = openDoorSFX;
        audioSource.Play();
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
        doorState = DoorState.closed;
        float timeElapsed = 0;
        float closeDoorTime = closeDoorCurve.keys[closeDoorCurve.length - 1].time;
        audioSource.clip = closeDoorSFX;
        audioSource.Play();
        while (timeElapsed <= closeDoorTime)
        {
            doorToMove.position = Vector3.Lerp(endPosition, originalDoorPostion, closeDoorCurve.Evaluate(timeElapsed));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        doorToMove.position = originalDoorPostion;
       
    }

   public void OpenDoor()
    {
        if (doorState == DoorState.open) return;
        StartCoroutine(OpenLerpDoor());
    }

    public void CloseDoor()
    {
        if (doorState == DoorState.closed) return;
        StartCoroutine(CloseLerpDoor());
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
        player.CharacterInputs.Disable();
        float timeElapsed = 0f;
        yield return new WaitForSeconds(.75f);
        while (timeElapsed < .5f)
        {

            player.transform.position = Vector3.Lerp(player.SpawnPoint.position, player.ShovePoint.position, timeElapsed / .5f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        player.CharacterInputs.Enable();
    }

    public void BreakRenderQueue(Player player)
    {
        player.GetComponent<MeshRenderer>().material.renderQueue = 3002;
    }
    public void RenderQueueFix(Player player)
    {
       player.GetComponent<MeshRenderer>().material.renderQueue = -1;
    }

}
