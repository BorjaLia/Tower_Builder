using UnityEngine;

public class GameRigController : MonoBehaviour
{
    [Tooltip("How fast the rig follows the tower height")]
    [SerializeField] private float smoothSpeed = 2.0f;

    [Tooltip("Base Y offset to keep the tower visible")]
    [SerializeField] private float yOffset = 0.0f;

    private float targetY;
    private Vector3 initialPosition;

    private void Awake()
    {
        initialPosition = transform.position;
        targetY = initialPosition.y;
    }

    private void Start()
    {
        GameplayManager.OnHeightUpdated += UpdateTargetHeight;
    }

    private void OnDestroy()
    {
        GameplayManager.OnHeightUpdated -= UpdateTargetHeight;
    }

    private void UpdateTargetHeight(float newHeight)
    {
        targetY = newHeight + yOffset;
    }

    private void LateUpdate()
    {
        float currentY = Mathf.Lerp(transform.position.y, targetY, Time.deltaTime * smoothSpeed);
        transform.position = new Vector3(initialPosition.x, currentY, initialPosition.z);
    }
}