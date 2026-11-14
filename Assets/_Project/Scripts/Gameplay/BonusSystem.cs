using System.Collections;
using System.Linq;
using UnityEngine;

public class BonusSystem : MonoBehaviour
{
    public static BonusSystem Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Ball ballPrefab;
    [SerializeField] private PowerUp powerUpPrefab;
    [SerializeField] private LevelBuilder levelBuilder;

    [Header("Effects")]
    [SerializeField] private GameObject meteorEffectPrefab;

    [Header("Power-Ups")]
    [SerializeField, Range(0f, 1f)]
    private float powerUpChance = 0.2f;

    [SerializeField] private float explosionRadius = 2.2f;

    [Header("Falling Rows")]
    [SerializeField] private bool enableFallingRows = true;
    [SerializeField] private float rowInterval = 10f;

    private bool explosionArmed;
    private bool suppressPowerUps;
    private bool checkingBallLoss;

    private Ball explosionBall;
    private GameObject meteorEffectInstance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        GameEvents.BrickDestroyed += OnBrickDestroyed;

        if (enableFallingRows)
            StartCoroutine(FallingRowsRoutine());
    }

    private void OnDisable()
    {
        GameEvents.BrickDestroyed -= OnBrickDestroyed;
        StopAllCoroutines();
    }

    private void OnBrickDestroyed(ImpactInfo info)
    {
        if (explosionArmed)
        {
            explosionArmed = false;

            ExplodeAround(info.Center);
            RemoveMeteorEffect();

            if (LastLifeMode.Instance != null && explosionBall != null)
                LastLifeMode.Instance.RestoreBallVisual(explosionBall);

            explosionBall = null;
            return;
        }

        if (suppressPowerUps || powerUpPrefab == null)
            return;

        if (Random.value <= powerUpChance)
            Instantiate(powerUpPrefab, info.Center, Quaternion.identity);
    }

    public void CollectPowerUp(PowerUpType type, Vector2 position)
    {
        if (type == PowerUpType.MultiBall)
        {
            DoubleExistingBalls();
            return;
        }

        if (type == PowerUpType.Explosion)
        {
            explosionBall = FindClosestBall(position);

            if (explosionBall == null)
                return;

            explosionArmed = true;

            if (LastLifeMode.Instance != null)
                LastLifeMode.Instance.ApplyFireVisual(explosionBall);

            CreateMeteorEffect();
        }
    }

    private void DoubleExistingBalls()
    {
        if (ballPrefab == null)
            return;

        Ball[] existingBalls =
            FindObjectsByType<Ball>(FindObjectsSortMode.None);

        foreach (Ball sourceBall in existingBalls)
        {
            Ball newBall = Instantiate(
                ballPrefab,
                sourceBall.transform.position,
                sourceBall.transform.rotation);

            newBall.gameObject.AddComponent<BonusBall>();

            if (LastLifeMode.Instance != null)
                LastLifeMode.Instance.RestoreBallVisual(newBall);
        }
    }

    private Ball FindClosestBall(Vector2 position)
    {
        Ball[] balls =
            FindObjectsByType<Ball>(FindObjectsSortMode.None);

        if (balls.Length == 0)
            return null;

        return balls
            .OrderBy(ball =>
                Vector2.Distance(position, ball.transform.position))
            .First();
    }

    private void CreateMeteorEffect()
    {
        RemoveMeteorEffect();

        if (meteorEffectPrefab == null || explosionBall == null)
            return;

        meteorEffectInstance =
            Instantiate(meteorEffectPrefab, explosionBall.transform);

        meteorEffectInstance.transform.localPosition = Vector3.zero;
        meteorEffectInstance.transform.localRotation = Quaternion.identity;
        meteorEffectInstance.transform.localScale = Vector3.one * 0.25f;
    }

    private void RemoveMeteorEffect()
    {
        if (meteorEffectInstance != null)
        {
            Destroy(meteorEffectInstance);
            meteorEffectInstance = null;
        }
    }

    private void ExplodeAround(Vector2 center)
    {
        Brick[] bricks =
            FindObjectsByType<Brick>(FindObjectsSortMode.None);

        Brick[] nearbyBricks = bricks
            .Where(brick =>
                Vector2.Distance(center, brick.transform.position)
                <= explosionRadius)
            .OrderBy(brick =>
                Vector2.Distance(center, brick.transform.position))
            .Take(3)
            .ToArray();

        suppressPowerUps = true;

        foreach (Brick brick in nearbyBricks)
        {
            if (brick != null)
                brick.DestroyByPowerUp(brick.transform.position);
        }

        suppressPowerUps = false;
    }

    public void NotifyBallLost()
    {
        if (checkingBallLoss)
            return;

        StartCoroutine(CheckAllBallsLost());
    }

    private IEnumerator CheckAllBallsLost()
    {
        checkingBallLoss = true;

        yield return null;

        Ball[] remainingBalls =
            FindObjectsByType<Ball>(FindObjectsSortMode.None);

        if (remainingBalls.Length == 0)
        {
            RemoveMeteorEffect();

            if (LastLifeMode.Instance != null)
                LastLifeMode.Instance.RefreshAllBallVisuals();

            GameManager.Instance.OnBallLost();
        }

        checkingBallLoss = false;
    }

    private IEnumerator FallingRowsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(rowInterval);

            if (levelBuilder != null)
                levelBuilder.SpawnFallingRow();
        }
    }
}
