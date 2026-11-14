using UnityEngine;
using UnityEngine.UI;

public class LastLifeMode : MonoBehaviour
{
    public static LastLifeMode Instance { get; private set; }

    [SerializeField] private GameObject warningBorder;
    [SerializeField] private Image warningImage;
    [SerializeField] private TrailRenderer ballTrail;
    [SerializeField] private SpriteRenderer ballRenderer;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        GameEvents.LivesChanged += OnLivesChanged;
    }

    private void OnDisable()
    {
        GameEvents.LivesChanged -= OnLivesChanged;
    }

    private void Start()
    {
        RefreshAllBallVisuals();
    }

    private void OnLivesChanged(int lives)
    {
        RefreshAllBallVisuals();
    }

    public void RefreshAllBallVisuals()
    {
        int lives = GameManager.Instance.Lives;
        Color lifeColor = GetLifeColor(lives);

        if (warningBorder != null)
            warningBorder.SetActive(lives > 0 && lives <= 2);

        if (warningImage != null)
        {
            Color imageColor = lifeColor;
            imageColor.a = lives == 2 ? 0.03f : 0.06f;
            warningImage.color = imageColor;
        }

        Ball[] balls =
            FindObjectsByType<Ball>(FindObjectsSortMode.None);

        foreach (Ball ball in balls)
        {
            RestoreBallVisual(ball);
        }
    }

    public void RestoreBallVisual(Ball ball)
    {
        if (ball == null)
            return;

        Color lifeColor =
            GetLifeColor(GameManager.Instance.Lives);

        SpriteRenderer renderer =
            ball.GetComponentInChildren<SpriteRenderer>();

        TrailRenderer trail =
            ball.GetComponentInChildren<TrailRenderer>();

        if (renderer != null)
            renderer.color = lifeColor;

        if (trail != null)
        {
            trail.startColor = lifeColor;
            trail.endColor = new Color(
                lifeColor.r,
                lifeColor.g,
                lifeColor.b,
                0f
            );
        }

        if (ball == GetMainBall())
        {
            if (ballRenderer != null)
                ballRenderer.color = lifeColor;

            if (ballTrail != null)
            {
                ballTrail.startColor = lifeColor;
                ballTrail.endColor = new Color(
                    lifeColor.r,
                    lifeColor.g,
                    lifeColor.b,
                    0f
                );
            }
        }
    }

    public void ApplyFireVisual(Ball ball)
    {
        if (ball == null)
            return;

        Color fireColor =
            new Color(1f, 0.25f, 0f);

        SpriteRenderer renderer =
            ball.GetComponentInChildren<SpriteRenderer>();

        TrailRenderer trail =
            ball.GetComponentInChildren<TrailRenderer>();

        if (renderer != null)
            renderer.color = fireColor;

        if (trail != null)
        {
            trail.startColor = fireColor;
            trail.endColor = new Color(
                fireColor.r,
                fireColor.g,
                fireColor.b,
                0f
            );
        }
    }

    private Color GetLifeColor(int lives)
    {
        if (lives <= 0)
            return Color.white;

        if (lives == 1)
            return Color.red;

        if (lives == 2)
            return new Color(1f, 0.5f, 0f);

        return Color.cyan;
    }

    private Ball GetMainBall()
    {
        Ball[] balls =
            FindObjectsByType<Ball>(FindObjectsSortMode.None);

        foreach (Ball ball in balls)
        {
            if (!ball.TryGetComponent<BonusBall>(out _))
                return ball;
        }

        return null;
    }
}
