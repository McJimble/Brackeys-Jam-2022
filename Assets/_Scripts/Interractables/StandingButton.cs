using UnityEngine;
using System.Collections;

public class StandingButton : Interactable
{
    [Header("Visual Settings")]
    [SerializeField] private AnimationCurve pushInCurve;
    [SerializeField] private AnimationCurve pullOutCurve;
    [SerializeField] private Transform pushButtonTransform;
    [SerializeField] private float moveAmountY = 0.05f;
    
    public override void TryInterract(IInteractor interactor)
    {
        base.TryInterract(interactor);
        StartCoroutine(ButtonPushAnimation());
    }

    private IEnumerator ButtonPushAnimation()
    {
        Vector3 buttonOriginalPosition = pushButtonTransform.position;
        Vector3 endPosition = pushButtonTransform.position + (pushButtonTransform.up * -moveAmountY);
        float pushInTime = pushInCurve.keys[pushInCurve.length - 1].time;
        float pullOutTime = pushInCurve.keys[pushInCurve.length - 1].time;
        float animTime = 0;

        // Push in/out along respective curves.
        while (animTime <= pushInTime)
        {
            pushButtonTransform.position = Vector3.Lerp(buttonOriginalPosition, endPosition, pushInCurve.Evaluate(animTime));
            animTime += Time.deltaTime;
            yield return null;
        }

        // Interract when pushed all the way in.
        onInterract.Invoke();

        // Pull out now.
        animTime = 0;
        while (animTime <= pullOutTime)
        {
            pushButtonTransform.position = Vector3.Lerp(endPosition, buttonOriginalPosition, pullOutCurve.Evaluate(animTime));
            animTime += Time.deltaTime;
            yield return null;
        }

    }

}
