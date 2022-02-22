using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMover : MonoBehaviour
{
    [SerializeField] private bool openOnStart = false;
    [SerializeField] private Transform doorToMove;
    [SerializeField] private float moveAmount = .5f;
    [SerializeField] private AnimationCurve openDoorCurve;
    [SerializeField] private AnimationCurve closeDoorCurve;

    Vector3 originalDoorPostion;
    Vector3 endPosition;
    

    private void Start()
    {
        if (openOnStart)
        {
            StartCoroutine(OpenLerpDoor());
        }
        originalDoorPostion = doorToMove.position;
        endPosition = doorToMove.position + (doorToMove.up * moveAmount);
    }

    private void OnTriggerEnter(Collider other)
    {
        
        Player player;
        if (other.TryGetComponent<Player>(out player))
        {
            
            StartCoroutine(CloseLerpDoor());
        }
    }

    // could go back to trigger for future usefullness

    public IEnumerator OpenLerpDoor()
    {
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
    }

}
