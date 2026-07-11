using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sr;

    void Awake()
    {
        if (GameState.justFinishedBattle)
            transform.position = GameState.playerReturnPosition;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        rb.linearVelocity = new Vector2(x, y) * speed;
        animator.SetBool("isWalking", x != 0 || y != 0);

        if (x < 0)
            sr.flipX = true;   // moving left
        else if (x > 0)
            sr.flipX = false;  // moving right
    }
}