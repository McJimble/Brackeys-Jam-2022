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

    private WaitForFixedUpdate waitForFixed = new WaitForFixedUpdate();

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
        StopAllCoroutines();
        StartCoroutine(PushAnim());
    }

    public void PressurePlateReleaseAnim()
    {
        if (pushButtonTransform.position == pushButtonOriginalPositon) return;
        StopAllCoroutines();
        StartCoroutine(ReleaseAnim());
    }

    private IEnumerator PushAnim()
    {
        CurrentlyInteracting = true;
       
        float pushInTime = pushInCurve.keys[pushInCurve.length - 1].time;
        float animTime = 0;

        while (animTime <= pushInTime || Vector3.Distance(pushButtonTransform.position, endPostion) > 0.01f)
        {
            pushButtonTransform.position = Vector3.Lerp(pushButtonTransform.position, endPostion, pushInCurve.Evaluate(animTime));
            animTime += Time.fixedDeltaTime;
            yield return waitForFixed;
        }
        onInterract.Invoke();
    }

    private IEnumerator ReleaseAnim()
    {
        float animTime = 0;
        float pushOutTime = pullOutCurve.keys[pullOutCurve.length - 1].time;

        while (animTime <= pushOutTime)
        {
            pushButtonTransform.position = Vector3.Lerp(pushButtonTransform.position, pushButtonOriginalPositon, pullOutCurve.Evaluate(animTime));
            animTime += Time.fixedDeltaTime;
            yield return waitForFixed;
        }
        CurrentlyInteracting = false;
    }
}