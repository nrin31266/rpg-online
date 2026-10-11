using UnityEngine;

public class PlayerTestMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float fallGravityMultiplier = 2.5f;

    [Header("Two-stage jump — probe values")]
    [SerializeField, Min(0.01f)] private float holdThreshold = 0.4f;
    [SerializeField, Min(0f)] private float minimumStage2UpwardVelocity = 0.5f;
    [SerializeField, Range(0f, 0.35f)] private float stage2VelocityFraction = 0.18f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float defaultGravityScale;
    private float jumpStartTime;
    private bool jumpInProgress;
    private bool stage2Consumed;
    private bool stage2WindowClosed;
    private bool continuousJumpHold;

    // Camera được phép đọc trạng thái này,
    // nhưng không được tự sửa isGrounded.
    public bool IsGrounded => isGrounded;
    public float VerticalVelocity => rb != null ? rb.linearVelocity.y : 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravityScale = rb.gravityScale;
    }

    private void Update()
    {
        float horizontal = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
            horizontal = -1f;

        if (Input.GetKey(KeyCode.RightArrow))
            horizontal = 1f;

        // Di chuyển ngang
        rb.linearVelocity = new Vector2(
            horizontal * moveSpeed,
            rb.linearVelocity.y
        );

        ProcessJumpInput(Input.GetKeyDown(KeyCode.UpArrow),
            Input.GetKey(KeyCode.UpArrow), Time.time);

        UpdateFallGravity();
    }

    private void UpdateFallGravity()
    {
        // Rơi nhanh hơn lúc đi xuống
        if (rb.linearVelocity.y < 0f)
            rb.gravityScale = defaultGravityScale * fallGravityMultiplier;
        else
            rb.gravityScale = defaultGravityScale;
    }

    // Both input routes consume the same extension on the current jump.
    private void ProcessJumpInput(bool pressed, bool held, float now)
    {
        bool startedThisFrame = pressed && isGrounded;
        if (startedThisFrame)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            rb.gravityScale = defaultGravityScale;
            jumpStartTime = now;
            jumpInProgress = true;
            stage2Consumed = false;
            stage2WindowClosed = false;
            continuousJumpHold = held;
            isGrounded = false;
        }

        if (!held)
            continuousJumpHold = false;

        float elapsed = now - jumpStartTime;
        // Close before the apex, and never reopen on this jump after a bounce.
        if (jumpInProgress && VerticalVelocity <= minimumStage2UpwardVelocity)
            stage2WindowClosed = true;

        if (startedThisFrame || !jumpInProgress || stage2Consumed ||
            stage2WindowClosed || elapsed < 0f)
            return;

        bool holdTrigger = continuousJumpHold && held && elapsed >= holdThreshold;
        // A second press may be relaxed; ascent, not double-tap speed, gates it.
        bool secondPressTrigger = pressed;
        if (!holdTrigger && !secondPressTrigger)
            return;

        // Add a modest contribution; never reset Y to a full second launch.
        rb.linearVelocity += Vector2.up * (jumpForce * stage2VelocityFraction);
        stage2Consumed = true;
        continuousJumpHold = false;
    }

    private void OnCollisionEnter2D(Collision2D collision) => UpdateGroundContact(collision);

    private void OnCollisionStay2D(Collision2D collision) => UpdateGroundContact(collision);

    private void UpdateGroundContact(Collision2D collision)
    {
        // Sandbox hiện tại chỉ có GroundTilemap.
        if (collision.gameObject.name != "GroundTilemap")
            return;

        // A remaining takeoff contact must not reset this jump's one-shot state.
        if (VerticalVelocity > 0.05f)
        {
            isGrounded = false;
            return;
        }

        bool supported = false;
        // Chỉ coi là đứng đất khi mặt tiếp xúc hướng lên.
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                supported = true;
                break;
            }
        }

        if (supported && !isGrounded)
        {
            jumpInProgress = false;
            stage2Consumed = false;
            stage2WindowClosed = false;
            continuousJumpHold = false;
        }
        isGrounded = supported;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "GroundTilemap")
            isGrounded = false;
    }
}
