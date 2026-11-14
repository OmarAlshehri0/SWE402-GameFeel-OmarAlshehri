using UnityEngine;

public enum PowerUpType
{
    MultiBall,
    Explosion
}

public class PowerUp : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 2.5f;

    private PowerUpType type;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        type = (PowerUpType)Random.Range(0, 2);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = type == PowerUpType.MultiBall
                ? Color.cyan
                : Color.yellow;
        }
    }

    private void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out Paddle _)) return;

        BonusSystem.Instance.CollectPowerUp(type, transform.position);
        Destroy(gameObject);
    }
}
