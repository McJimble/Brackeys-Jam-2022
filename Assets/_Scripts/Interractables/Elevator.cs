using UnityEngine;

public class Elevator : Interactable
{
    [SerializeField] private AnimationCurve elevatorUpCurve;
    [SerializeField] private AnimationCurve elevatorDownCurve;

    [SerializeField] private int startPathPosition;
    [SerializeField] private Transform[] pathTransforms;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        for (int i = 0; i < pathTransforms.Length; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(pathTransforms[i].position, Vector3.one * 0.5f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pathTransforms[i].position, pathTransforms[(i + 1) % pathTransforms.Length].position);
        }
    }
#endif


}
