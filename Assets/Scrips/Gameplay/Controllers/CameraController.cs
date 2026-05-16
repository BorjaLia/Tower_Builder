using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Tooltip("Smoothing")]
    [SerializeField] private float smoothSpeed = 2.0f;
    [Tooltip("Distance over the tallest block")]
    [SerializeField] private float yOffset = 2.0f;

    private float targetY;
    private Vector3 initialPosition;

    private void Awake()
    {
        transform.position = new Vector3(0,yOffset,-10);
    }

    private void Start()
    {
        initialPosition = transform.position;
        targetY = initialPosition.y;
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