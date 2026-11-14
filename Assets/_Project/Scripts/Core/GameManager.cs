using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Ball ball;

    [Header("Lives")]
    [SerializeField] private int startingLives = 3;

    [Header("Progressive Speed")]
    [SerializeField] private float speedIncreaseInterval = 20f;
    [SerializeField] private float speedIncreasePercent = 0.05f;
    [SerializeField] private float maximumSpeedMultiplier = 1.5f;

    public int Score { get; private set; }
    public int Lives { get; private set; }
    public int Combo { get; private set; }

    public float SpeedMultiplier { get; private set; } = 1f;

    private float speedTimer;
    private bool gameOver;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Score = 0;
        Lives = startingLives;
        Combo = 0;
        SpeedMultiplier = 1f;
        speedTimer = 0f;
        gameOver = false;
    }

    private void Update()
    {
        if (gameOver)
            return;

        speedTimer += Time.deltaTime;

        if (speedTimer >= speedIncreaseInterval)
        {
            speedTimer = 0f;

            SpeedMultiplier = Mathf.Min(
                SpeedMultiplier + speedIncreasePercent,
                maximumSpeedMultiplier
            );
        }
    }

    public void AddScore(int amount)
    {
        if (gameOver)
            return;

        Score += amount;
        GameEvents.RaiseScoreChanged(Score);
    }

    public int RegisterBrickCombo()
    {
        return ++Combo;
    }

    public void ResetCombo()
    {
        Combo = 0;
    }

    public void OnBallLost()
    {
        if (gameOver)
            return;

        Lives--;
        ResetCombo();

        GameEvents.RaiseLivesChanged(Lives);
        GameEvents.RaiseBallLost();

        if (Lives <= 0)
        {
            gameOver = true;

            if (ball != null)
                ball.gameObject.SetActive(false);

            return;
        }

        if (ball != null)
        {
            ball.gameObject.SetActive(true);
            ball.Launch();
        }
    }
}
