using UnityEngine;

public class CameraFollowTest : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private PlayerTestMovement movement;

    [Header("Vertical Camera")]
    [SerializeField] private float verticalOffset = 0.8f;
    [SerializeField] private float verticalSmoothTime = 0.25f;
    [SerializeField, Min(0f)] private float upperSoftZone = 1.85f;
    [SerializeField, Min(0f)] private float lowerSoftZone = 2.3f;
    [SerializeField, Min(0f)] private float upwardLookAhead = 0.08f;
    [SerializeField, Min(0f)] private float downwardLookAhead = 0.06f;
    [SerializeField, Min(0f)] private float maxLookAhead = 0.35f;
    [SerializeField, Min(0.01f)] private float upwardSmoothTime = 0.18f;
    [SerializeField, Min(0.01f)] private float downwardSmoothTime = 0.28f;
    [SerializeField, Min(0f)] private float safeViewMargin = 0.5f;

    [Header("Pixel stability — optional probe, not an accepted fix")]
    [SerializeField] private bool quantizeToWorldPixel;
    [SerializeField, Min(1f)] private float pixelsPerUnit = 32f;

    private float verticalVelocity;
    private float targetY;
    private float smoothedY;
    private Camera viewCamera;

    private void Awake()
    {
        viewCamera = GetComponent<Camera>();
        // Nếu đã kéo PlayerTest vào Target,
        // tự lấy PlayerTestMovement nếu Movement chưa được gán.
        if (target != null && movement == null)
            movement = target.GetComponent<PlayerTestMovement>();
    }

    private void Start()
    {
        if (target == null)
            return;

        targetY = smoothedY = target.position.y + verticalOffset;
        transform.position = new Vector3(target.position.x, smoothedY, transform.position.z);
    }

    private void LateUpdate()
    {
        Follow(Time.deltaTime);
    }

    private void Follow(float deltaTime)
    {
        if (target == null || movement == null)
            return;

        float playerY = target.position.y;
        float velocityY = movement.VerticalVelocity;
        float smoothTime = verticalSmoothTime;
        float safeHalfHeight = viewCamera != null && viewCamera.orthographic
            ? Mathf.Max(0.1f, viewCamera.orthographicSize - safeViewMargin)
            : float.PositiveInfinity;
        float upperZone = Mathf.Min(upperSoftZone, safeHalfHeight);
        float lowerZone = Mathf.Min(lowerSoftZone, safeHalfHeight);
        if (movement.IsGrounded)
        {
            // Landing settles the view; it does not unlock vertical follow.
            targetY = playerY + verticalOffset;
        }
        else if (velocityY > 0f)
        {
            float revealY = playerY + Mathf.Min(velocityY * upwardLookAhead, maxLookAhead);
            if (revealY - smoothedY > upperZone)
                targetY = Mathf.Max(targetY, revealY - upperZone);
            smoothTime = upwardSmoothTime;
        }
        else if (velocityY < 0f)
        {
            float revealY = playerY - Mathf.Min(-velocityY * downwardLookAhead, maxLookAhead);
            if (smoothedY - revealY > lowerZone)
                targetY = Mathf.Min(targetY, revealY + lowerZone);
            smoothTime = downwardSmoothTime;
        }

        smoothedY = Mathf.SmoothDamp(smoothedY, targetY, ref verticalVelocity,
            Mathf.Max(0.01f, smoothTime), Mathf.Infinity, deltaTime);

        // Safety catch for a long/fast drop; no changes to map camera bounds.
        float safeY = Mathf.Clamp(smoothedY, playerY - safeHalfHeight, playerY + safeHalfHeight);
        if (safeY != smoothedY)
        {
            smoothedY = safeY;
            targetY = safeY;
            verticalVelocity = 0f;
        }

        float cameraX = target.position.x;
        float cameraY = smoothedY;
        if (quantizeToWorldPixel)
        {
            cameraX = Mathf.Round(cameraX * pixelsPerUnit) / pixelsPerUnit;
            cameraY = Mathf.Round(cameraY * pixelsPerUnit) / pixelsPerUnit;
        }
        transform.position = new Vector3(cameraX, cameraY, transform.position.z);
    }
}
