using UnityEngine;

/// <summary>
/// Rotates an object constantly at a random speed and direction in euler angles/sec.
/// </summary>
public class RotateRandomly : MonoBehaviour
{
    [Header("Pre-Runtime")]
    [SerializeField] private bool beginRotateOnStart = false;
    [Min(0f)]
    [SerializeField] private float maxRotationSpeed;

    [Header("Runtime")]
    [SerializeField] private Vector3 rotationDirection;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private bool rotating = false;

    public Vector3 RotationDirection { get => rotationDirection; set => rotationDirection = value.normalized; }
    public float MaxRotationSpeed { get => maxRotationSpeed; set => maxRotationSpeed = value; }
    public float RotationSpeed { get => rotationSpeed; set => rotationSpeed = System.Math.Clamp(rotationSpeed, 0, value); }
    public bool Rotating { get => rotating; set => rotating = value; }

    private void Start()
    {
        if (!beginRotateOnStart) return;

        GenerateNewRotation();
    }

    private void Update()
    {
        if (rotating)
        {
            transform.Rotate(rotationDirection.x * rotationSpeed * Time.deltaTime,
                            rotationDirection.y * rotationSpeed * Time.deltaTime,
                            rotationDirection.z * rotationSpeed * Time.deltaTime, 
                            Space.World);
        }
    }

    public void GenerateNewRotation()
    {
        rotationDirection = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
        rotationDirection.Normalize();

        rotationSpeed = (Random.Range(0, maxRotationSpeed));
    }

    public void SetIsRotating(bool enable)
    {
        rotating = enable;
        if (enable) GenerateNewRotation();
    }
}
