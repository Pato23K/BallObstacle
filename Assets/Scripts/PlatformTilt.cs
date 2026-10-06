using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlatformTilt : MonoBehaviour
{
    public float interval = 4f;
    public float angle = 5f;
    public float rotationSpeed = 3f;

    private Rigidbody body;
    private float timer;
    private int step;
    private Quaternion target = Quaternion.identity;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (timer >= interval)
        {
            timer = 0f;
            step = (step + 1) % 4;
            if (step == 0) target = Quaternion.Euler(angle, 0f, 0f);
            if (step == 1) target = Quaternion.Euler(0f, 0f, angle);
            if (step == 2) target = Quaternion.Euler(-angle, 0f, 0f);
            if (step == 3) target = Quaternion.Euler(0f, 0f, -angle);
        }

        body.MoveRotation(Quaternion.RotateTowards(body.rotation, target,
            rotationSpeed * Time.fixedDeltaTime));
    }
}
