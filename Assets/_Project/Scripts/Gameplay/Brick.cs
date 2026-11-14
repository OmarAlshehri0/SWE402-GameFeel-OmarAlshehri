using UnityEngine;

public class Brick : MonoBehaviour
{
    [SerializeField] private int hitPoints = 1;
    [SerializeField] private int scoreValue = 10;

    [Header("Special Brick")]
    [SerializeField] private bool isInfectionBrick;
    [SerializeField] private bool isBossBrick;
    [SerializeField] private float infectionRadius = 2.4f;

    [Header("Effects")]
    [SerializeField] private GameObject volcanoEffectPrefab;
    [SerializeField] private GameObject infectionIndicatorPrefab;

    private SpriteRenderer spriteRenderer;
    private GameObject infectionIndicatorInstance;
    private bool destroyed;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ConfigureInfection(bool value)
    {
        isInfectionBrick = value;

        if (!value)
            return;

        isBossBrick = false;

        if (spriteRenderer != null)
            spriteRenderer.color = Color.black;

        if (infectionIndicatorPrefab != null)
        {
            infectionIndicatorInstance = Instantiate(
                infectionIndicatorPrefab,
                transform
            );

            infectionIndicatorInstance.transform.localPosition =
                Vector3.zero;

            infectionIndicatorInstance.transform.localRotation =
                Quaternion.identity;

            infectionIndicatorInstance.transform.localScale =
                Vector3.one * 0.06f;
        }
    }

    public void ConfigureBoss(bool value)
    {
        isBossBrick = value;

        if (!value)
            return;

        isInfectionBrick = false;
        hitPoints = 3;
        scoreValue = 50;

        if (spriteRenderer != null)
            spriteRenderer.color = new Color(0.7f, 0.1f, 1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.TryGetComponent(out Ball _))
            return;

        ContactPoint2D contact = collision.GetContact(0);
        TakeHit(contact.point, contact.normal);
    }

    public void DestroyByPowerUp(Vector2 point)
    {
        if (destroyed)
            return;

        hitPoints = 1;
        TakeHit(point, Vector2.up);
    }

    private void TakeHit(Vector2 point, Vector2 normal)
    {
        if (destroyed)
            return;

        hitPoints--;

        if (isBossBrick)
        {
            if (hitPoints == 2 && spriteRenderer != null)
                spriteRenderer.color =
                    new Color(1f, 0.55f, 0.05f);

            if (hitPoints == 1 && spriteRenderer != null)
                spriteRenderer.color = Color.red;
        }

        if (hitPoints > 0)
            return;

        destroyed = true;

        if (infectionIndicatorInstance != null)
            Destroy(infectionIndicatorInstance);

        GameManager.Instance.AddScore(scoreValue);

        Color color =
            spriteRenderer != null
                ? spriteRenderer.color
                : Color.white;

        Vector2 size =
            spriteRenderer != null
                ? spriteRenderer.bounds.size
                : Vector2.one;

        int combo =
            GameManager.Instance.RegisterBrickCombo();

        GameEvents.RaiseBrickDestroyed(
            new ImpactInfo(
                point,
                normal,
                1f,
                color,
                transform.position,
                size,
                combo
            )
        );

        if (isInfectionBrick)
        {
            SpawnVolcanoEffect();
            InfectNearbyBricks();
        }

        Destroy(gameObject);
    }

    private void InfectNearbyBricks()
    {
        Brick[] bricks =
            FindObjectsByType<Brick>(
                FindObjectsSortMode.None
            );

        foreach (Brick brick in bricks)
        {
            if (brick == null || brick == this)
                continue;

            float distance =
                Vector2.Distance(
                    transform.position,
                    brick.transform.position
                );

            if (distance <= infectionRadius)
                brick.DestroyByPowerUp(
                    brick.transform.position
                );
        }
    }

    private void SpawnVolcanoEffect()
    {
        if (volcanoEffectPrefab == null)
            return;

        GameObject effect = Instantiate(
            volcanoEffectPrefab,
            transform.position,
            Quaternion.identity
        );

        effect.transform.localScale =
            Vector3.one * 0.10f;
    }
}
