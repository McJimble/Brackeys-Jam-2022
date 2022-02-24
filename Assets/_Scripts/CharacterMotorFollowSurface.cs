using UnityEngine;
using System.Collections.Generic;

// Forces an interactor to temporarily parent itself to this object
public class CharacterMotorFollowSurface : Interactable
{
    private Dictionary<IInteractor, Transform> originalParentStore = new Dictionary<IInteractor, Transform>();

    protected override void Awake()
    {
        base.Awake();
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
    }

    public void UnparentInteractor(IInteractor toUnparent)
    {
        if (!originalParentStore.ContainsKey(toUnparent)) return;

        toUnparent.InteractingTransform.parent = originalParentStore[toUnparent];
        originalParentStore.Remove(toUnparent);
    }

    public override bool FulfillsEnterRadiusParams(IInteractor interactor)
    {
        return interactor.CanInterchangeParents && this.CanBeInteracted;
    }

    public override bool FulfillsExitRadiusParams(IInteractor interactor)
    {
        return interactor.CanInterchangeParents && this.CanBeInteracted;
    }
}
