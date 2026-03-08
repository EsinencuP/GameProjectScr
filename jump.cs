using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    public float jumpForce = 7f;
    public int maxJumps = 2;

    private Rigidbody2D rb;
    private int jumpCount;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
{
    float force = jumpCount == 0 ? jumpForce : jumpForce * 0.8f;

    rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);
    jumpCount++;
}
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}