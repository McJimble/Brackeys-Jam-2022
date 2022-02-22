using UnityEngine;

public class Elevator : Interactable
{
    [System.Serializable]
    public class TimedMoveInfo
    {
        public float delayTime;             // Wait this long before actually moving the platform.
        public Transform moveToTransform;   // Position we will be moving to.
        public AnimationCurve moveToCurve;  // Animation curve we follow for the movement (including time)

        private float? animLength;
        public float AnimLength
        {
            get
            {
                if (animLength == null)
                    animLength = moveToCurve.keys[moveToCurve.length - 1].time;

                return (float)animLength;
            }
        }
    }

    [SerializeField] private Transform elevatorTransform;       // This object will be moved by this script.
    [Space]
    [SerializeField] private bool followPath;                   // If true, follows its movePathInfos indefinitely
    [SerializeField] private bool canOverrideCurrentMovement;   // If true, can change it's current destination mid-animation.
    [Min(0)]
    [SerializeField] private int pathIndex;                     // Next path index to move to.
    [SerializeField] private TimedMoveInfo[] movePathInfos;     // Movement we will be following by default.

    [Header("Runtime")]
    private TimedMoveInfo currentTimedInfo;
    private Vector3 startMovePosition;
    [SerializeField] private float moveTimeElapsed = 0;
    [SerializeField] private float delayTimeRemaining = 0;

    // Set to false to make elevator stop following its normal preset path.
    public bool FollowPath { get => followPath; set => followPath = value; }
    public bool CanOverrideCurrentMovement { get => canOverrideCurrentMovement; set => canOverrideCurrentMovement = value; }

    public bool CanMoveNext(TimedMoveInfo moveInfo)
    {
        if (currentTimedInfo != null && !canOverrideCurrentMovement)
        {
            Debug.LogWarning("Cannot move this platform while it is already moving.");
            return false;
        }
        // If set to position it's already moving to, do nothing
        else if (currentTimedInfo == moveInfo)
        {
            Debug.LogWarning("Trying to move platform to position it is already moving to");
            return false;
        }

        return true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (movePathInfos == null) return;
        for (int i = 0; i < movePathInfos.Length; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(movePathInfos[i].moveToTransform.position, Vector3.one * 0.5f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(movePathInfos[i].moveToTransform.position, movePathInfos[(i + 1) % movePathInfos.Length].moveToTransform.position);
        }
    }
#endif

    private void Start()
    {
        // Incase useer specifies one that is too large/small.
        pathIndex = System.Math.Clamp(pathIndex, 0, movePathInfos.Length - 1);
    }

    private void Update()
    {
        if (currentTimedInfo != null)
        {
            // Waiting on path delay to finish
            if (delayTimeRemaining >= 0)
            {
                delayTimeRemaining -= Time.deltaTime;
                startMovePosition = elevatorTransform.position;
            }
            // Start moving after delay time is done; start from wherever the elevator was when delay finished.
            else if (moveTimeElapsed <= currentTimedInfo.AnimLength)
            {
                elevatorTransform.position = Vector3.Lerp(startMovePosition,
                                                  currentTimedInfo.moveToTransform.position,
                                                  currentTimedInfo.moveToCurve.Evaluate(moveTimeElapsed));

                moveTimeElapsed += Time.deltaTime;
            }
            else
                currentTimedInfo = null;
        }
        else
        {
            // If following path, go to next one.
            if (followPath)
            {
                pathIndex = (pathIndex + 1) % movePathInfos.Length;
                SetNextMoveTo(movePathInfos[pathIndex]);
            }
            // If not folowing path, stop moving by just nulling the current path info.
            else
                currentTimedInfo = null;
        }
    }

    public void SetNextMoveTo(TimedMoveInfo moveInfo, bool includeDelay = true)
    {
        if (!CanMoveNext(moveInfo)) return;

        currentTimedInfo = moveInfo;
        delayTimeRemaining = (includeDelay) ? moveInfo.delayTime : 0f;
        moveTimeElapsed = 0;
    }

    public void SetNextMoveTo(int index)
    {
        index = System.Math.Clamp(index, 0, movePathInfos.Length - 1);
        SetNextMoveTo(movePathInfos[index]);
    }

    public void SetNextMoveToInstant(int index)
    {
        index = System.Math.Clamp(index, 0, movePathInfos.Length - 1);
        SetNextMoveTo(movePathInfos[index], false);
    }

    public void MoveToNextInPath(bool isDelayed)
    {
        int nextIndexToTry = (pathIndex + 1) % movePathInfos.Length;
        if (!CanMoveNext(movePathInfos[nextIndexToTry])) return;

        pathIndex = nextIndexToTry;
        SetNextMoveTo(movePathInfos[pathIndex], isDelayed);
    }
}
