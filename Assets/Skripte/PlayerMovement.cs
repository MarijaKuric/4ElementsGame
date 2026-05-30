using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // Čita tipke WASD ili strelice
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        rb.linearVelocity = new Vector2(x, y) * speed;
    }
}