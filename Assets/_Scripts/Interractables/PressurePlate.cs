using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : Interactable
{
    [Header("Visual Setting")]
    [SerializeField] private AnimationCurve pushInCurve;
    [SerializeField] private AnimationCurve pullOutCurve;
    [SerializeField] private Transform pushButtonTransform;
    [SerializeField] private float moveAmount = 0.5f;

    Vector3 pushButtonOriginalPositon;
    Vector3 endPostion;
    bool isReleaseAnimRunning = false;

    private void Start()
    {
        pushButtonOriginalPositon = pushButtonTransform.position;
        endPostion = pushButtonTransform.position + (pushButtonTransform.up * -moveAmount);
    }

    public override bool TryInterract(IInteractor interactor)
    {
        if (!base.TryInterract(interactor)) return false;
        

        return true;
    }

  

    public void PressurePlatePushAnim()
    {
        if (isReleaseAnimRunning) return;
        StartCoroutine(PushAnim());
    }

    public void PressurePlateReleaseAnim()
    {
        if (pushButtonTransform.position == pushButtonOriginalPositon) return;
        isReleaseAnimRunning = true;
        StartCoroutine(ReleaseAnim());
        
    }

    private IEnumerator PushAnim()
    {
        CurrentlyInteracting = true;
       
        float pushInTime = pushInCurve.keys[pushInCurve.length - 1].time;
        float animTime = 0;

        while (animTime <= pushInTime)
        {
            pushButtonTransform.position = Vector3.Lerp(pushButtonOriginalPositon, endPostion, pushInCurve.Evaluate(animTime));
            animTime += Time.fixedDeltaTime;
            yield return null;
        }
        onInterract.Invoke();

    }

   

    private IEnumerator ReleaseAnim()
    {
        
        float animTime = 0;
        float pushOutTime = pushInCurve.keys[pushInCurve.length - 1].time;

        while (animTime <= pushOutTime)
        {
            pushButtonTransform.position = Vector3.Lerp(endPostion, pushButtonOriginalPositon, pullOutCurve.Evaluate(animTime));
            animTime += Time.fixedDeltaTime;
            yield return null;
        }
        CurrentlyInteracting = false;
        isReleaseAnimRunning = false;
    }




}