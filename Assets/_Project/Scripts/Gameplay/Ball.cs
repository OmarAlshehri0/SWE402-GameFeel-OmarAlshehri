using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float speed = 9f;

    [Header("Bounce")]
    [SerializeField] private float maxBounceAngle = 60f;
    [SerializeField] private float minVerticalRatio = 0.25f;

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private float speedMultiplier = 1f;

    private float CurrentSpeed => speed * speedMultiplier;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
    }

    private void Start()
    {
        if (GameManager.Instance != null)
            SetSpeedMultiplier(GameManager.Instance.SpeedMultiplier);

        Launch();
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }

    public void Launch()
    {
        transform.position = startPosition;

        float angle = Random.Range(-45f, 45f);
        Vector2 direction =
            Quaternion.Euler(0f, 0f, angle) * Vector2.up;

        rb.linearVelocity = direction * CurrentSpeed;
    }

    private void FixedUpdate()
    {
        Vector2 velocity = rb.linearVelocity;

        if (velocity.sqrMagnitude < 0.0001f)
            return;

        Vector2 direction = velocity.normalized;

        if (Mathf.Abs(direction.y) < minVerticalRatio)
        {
            float sign = direction.y >= 0f ? 1f : -1f;
            direction.y = sign * minVerticalRatio;
            direction.Normalize();
        }

        rb.linearVelocity = direction * CurrentSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D contact = collision.GetContact(0);
        ImpactInfo info = new ImpactInfo(contact.point, contact.normal);

        if (collision.gameObject.TryGetComponent(out Paddle paddle))
        {
            BounceFromPaddle(paddle);
            GameManager.Instance.ResetCombo();
            GameEvents.RaiseBallHitPaddle(info);
        }
        else if (!collision.gameObject.TryGetComponent(out Brick _))
        {
            GameEvents.RaiseBallHitWall(info);
        }
    }

    private void BounceFromPaddle(Paddle paddle)
    {
        float halfWidth = paddle.Width * 0.5f;

        float offset =
            (transform.position.x - paddle.transform.position.x) / halfWidth;

        offset = Mathf.Clamp(offset, -1f, 1f);

        float angle = offset * maxBounceAngle;

        Vector2 direction =
            Quaternion.Euler(0f, 0f, -angle) * Vector2.up;

        rb.linearVelocity = direction * CurrentSpeed;
    }
}
