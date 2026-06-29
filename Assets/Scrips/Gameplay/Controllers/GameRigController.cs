using UnityEngine;
// Warning: 'using static UnityEngine.GraphicsBuffer' no se usa.
using static UnityEngine.GraphicsBuffer;

public class GameRigController : MonoBehaviour
{
    [Tooltip("How fast the rig follows the tower sideways")]
    // Warning: smoothSpeed es Vector2 pero solo se usa .x; smoothSpeed.y queda muerto (la Y usa smoothSpeedY). Unificar a un float.
    [SerializeField] private Vector2 smoothSpeed = new Vector2(2.0f,2.0f);

    [Tooltip("How fast the rig follows the tower")]
    [SerializeField] private float smoothSpeedY = 2.0f;

    [Tooltip("Base Y offset to keep the tower visible")]
    [SerializeField] private float yOffset = 0.0f;

    [Tooltip("Allowed units for the game rig to not follow the tower horizontally")]
    [SerializeField] private float camDeadzone = 2.5f;

    private Vector2 target;
    private Vector3 initialPosition;

    private void Awake()
    {
        initialPosition = transform.position;
        target.x = initialPosition.x;
        target.y = initialPosition.y;
    }

    private void Start()
    {
        GameplayManager.OnTowerPosUpdated += UpdateTarget;
    }

    private void OnDestroy()
    {
        GameplayManager.OnTowerPosUpdated -= UpdateTarget;
    }

    private void UpdateTarget(Vector2 pos)
    {
        target.x = pos.x;
        target.y = pos.y + yOffset;
    }

    private void LateUpdate()
    {
        float currentRigX = transform.position.x;
        float desiredRigX = currentRigX;
        float offset = target.x - currentRigX;

        if (offset > camDeadzone)
        {
            desiredRigX = target.x - camDeadzone;
        }
        else if (offset < -camDeadzone)
        {
            desiredRigX = target.x + camDeadzone;
        }

        float smoothedX = Mathf.Lerp(currentRigX, desiredRigX, Time.deltaTime * smoothSpeed.x);
        float smoothedY = Mathf.Lerp(transform.position.y, target.y, Time.deltaTime * smoothSpeedY);

        transform.position = new Vector3(smoothedX, smoothedY, initialPosition.z);
    }
}