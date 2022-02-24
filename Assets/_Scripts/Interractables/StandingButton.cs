using UnityEngine;
using System.Collections;

public class StandingButton : Interactable
{
    
    [Header("Visual Settings")]
    [SerializeField] private AnimationCurve pushInCurve;
    [SerializeField] private AnimationCurve pullOutCurve;
    [SerializeField] private Transform pushButtonTransform;
    [SerializeField] private float moveAmountY = 0.05f;
    
    public override bool TryInteract(IInteractor interactor)
    {
        if (!base.TryInteract(interactor))
        {
            Debug.Log("Could not interact with " + gameObject.name);
            return false;
        }
            CurrentlyInteracting = true;
        StopAllCoroutines();
        StartCoroutine(ButtonPushAnimation(interactor));
        return true;
    }

    private IEnumerator ButtonPushAnimation(IInteractor pushInteractor)
    {
        
        Vector3 buttonOriginalPosition = pushButtonTransform.position;
        Vector3 endPosition = pushButtonTransform.position + (pushButtonTransform.up * -moveAmountY);
        float pushInTime = pushInCurve.keys[pushInCurve.length - 1].time;
        float pullOutTime = pullOutCurve.keys[pullOutCurve.length - 1].time;
        float animTime = 0;

        // Push in/out along respective curves.
        while (animTime <= pushInTime)
        {
            pushButtonTransform.position = Vector3.Lerp(buttonOriginalPosition, endPosition, pushInCurve.Evaluate(animTime));
            animTime += Time.deltaTime;
            yield return null;
        }

        // Interract when pushed all the way in.
        InvokeInteract(pushInteractor);

        // Pull out now.
        animTime = 0;
        while (animTime <= pullOutTime)
        {
            pushButtonTransform.position = Vector3.Lerp(endPosition, buttonOriginalPosition, pullOutCurve.Evaluate(animTime));
            animTime += Time.deltaTime;
            yield return null;
        }
        CurrentlyInteracting = false;
    }

}
