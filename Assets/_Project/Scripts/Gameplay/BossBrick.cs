using UnityEngine;

public class BossBrick : MonoBehaviour
{
    [Header("Boss Settings")]
    [SerializeField] private int hitPoints = 5;
    [SerializeField] private int scoreValue = 100;

    private SpriteRenderer spriteRenderer;
    private int currentHits;
    private bool destroyed;

    private void Awake()
    {
        spriteRenderer =
            GetComponent<SpriteRenderer>();

        currentHits = hitPoints;

        if (spriteRenderer != null)
            spriteRenderer.color =
                new Color(0.65f, 0.1f, 1f);
    }

    private void OnCollisionEnter2D(
        Collision2D collision
    )
    {
        if (destroyed)
            return;

        if (!collision.gameObject.TryGetComponent(out Ball _))
            return;

        ContactPoint2D contact =
            collision.GetContact(0);

        currentHits--;

        if (spriteRenderer != null)
        {
            float damage =
                1f - (float)currentHits / hitPoints;

            spriteRenderer.color =
                Color.Lerp(
                    new Color(0.65f, 0.1f, 1f),
                    Color.red,
                    damage
                );
        }

        if (currentHits > 0)
            return;

        destroyed = true;

        GameManager.Instance.AddScore(scoreValue);

        GameEvents.RaiseBrickDestroyed(
            new ImpactInfo(
                contact.point,
                contact.normal,
                1f,
                spriteRenderer != null
                    ? spriteRenderer.color
                    : Color.magenta,
                transform.position,
                transform.localScale,
                0
            )
        );

        Destroy(gameObject);
    }
}
