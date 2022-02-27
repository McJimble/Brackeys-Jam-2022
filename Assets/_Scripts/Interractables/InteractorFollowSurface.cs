using UnityEngine;
using System.Collections.Generic;

// Forces an interactor to temporarily parent itself to this object
public class InteractorFollowSurface : Interactable
{
    private Dictionary<IInteractor, Transform> originalParentStore = new Dictionary<IInteractor, Transform>();

    protected override void Awake()
    {
        base.Awake();
        requiresKeyPress = false;
        onEnterRadius.AddListener(RefreshInteractorParenting);
        onExitRadiusNU += UnparentInteractor;
    }

    public void RefreshInteractorParenting()
    {
        foreach (var interactor in currentlyInteractingObjects)
        {
            if (originalParentStore.ContainsKey(interactor)) continue;

            originalParentStore.Add(interactor, interactor.InteractingTransform.parent);
            interactor.InteractingTransform.parent = this.transform;
        }

        currentlyInteractingObjects.RemoveWhere(n => n == null);
    }

    public void UnparentInteractor(IInteractor toUnparent)
    {
        if (!originalParentStore.ContainsKey(toUnparent) || !toUnparent.CanInterchangeParents) return;

        toUnparent.InteractingTransform.parent = originalParentStore[toUnparent];
        originalParentStore.Remove(toUnparent);
    }

    public override bool FulfillsEnterRadiusParams(IInteractor interactor)
    {
        if (interactor == null) return false;
        return interactor.CanInterchangeParents && this.CanBeInteracted;
    }

    public override bool FulfillsExitRadiusParams(IInteractor interactor)
    {
        if (interactor == null) return false;
        return interactor.CanInterchangeParents && this.CanBeInteracted;
    }
}
