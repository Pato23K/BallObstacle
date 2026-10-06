using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    public float speed = 18f;
    public float maxSpeed = 4.5f;
    public GameManager game;

    private Rigidbody body;
    private Vector3 direction;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float x = 0f;
        float z = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) x--;
        if (Input.GetKey(KeyCode.RightArrow)) x++;
        if (Input.GetKey(KeyCode.DownArrow)) z--;
        if (Input.GetKey(KeyCode.UpArrow)) z++;
        direction = new Vector3(x, 0f, z).normalized;

        if (game != null && transform.position.y < -8f)
            game.RespawnBall();
    }

    void FixedUpdate()
    {
        if (game != null && game.HasWon) return;

        // Empujar la pelota directamente, sin torque.
        body.AddForce(direction * speed, ForceMode.Acceleration);
        Vector3 horizontal = new Vector3(body.linearVelocity.x, 0f, body.linearVelocity.z);
        horizontal = Vector3.ClampMagnitude(horizontal, maxSpeed);
        body.linearVelocity = new Vector3(horizontal.x, body.linearVelocity.y, horizontal.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (game != null) game.TouchObject(other.gameObject);
    }
}
