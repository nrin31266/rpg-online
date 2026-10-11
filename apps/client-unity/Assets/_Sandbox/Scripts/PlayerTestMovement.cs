
using UnityEngine;

public class PlayerTestMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;

    private Rigidbody2D rb;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float horizontal = 0f;

        // Di chuyển trái
        if (Input.GetKey(KeyCode.LeftArrow))
            horizontal = -1f;

        // Di chuyển phải
        if (Input.GetKey(KeyCode.RightArrow))
            horizontal = 1f;

        rb.linearVelocity = new Vector2(
            horizontal * moveSpeed,
            rb.linearVelocity.y
        );

        // Nhảy bằng mũi tên lên
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

            isGrounded = false;
        }
    }

    // Kiểm tra nhân vật đang đứng trên Tilemap
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.name != "GroundTilemap")
            return;

        // Chỉ tính là đứng trên đất khi tiếp xúc từ phía trên
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }

        isGrounded = false;
    }

    // Khi rời khỏi Tilemap
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "GroundTilemap")
        {
            isGrounded = false;
        }
    }
}
