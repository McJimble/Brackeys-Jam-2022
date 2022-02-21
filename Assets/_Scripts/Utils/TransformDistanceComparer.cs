using System.Collections.Generic;
using UnityEngine;

public class BehaviourDistanceComparerer : IComparer<Behaviour>
{
    private Transform trackedTransform;

    public BehaviourDistanceComparerer(Transform trackedTransform)
    {
        this.trackedTransform = trackedTransform;
    }

    public int Compare(Behaviour x, Behaviour y)
    {
        float xDist = Vector3.SqrMagnitude(x.transform.position - trackedTransform.position);
        float yDist = Vector3.SqrMagnitude(y.transform.position - trackedTransform.position);
        return (xDist.CompareTo(yDist));
    }
}
