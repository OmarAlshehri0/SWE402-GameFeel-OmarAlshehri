using System.Collections;
using UnityEngine;

public class ScorePulse : MonoBehaviour
{
    [SerializeField] private RectTransform scoreText;
    [SerializeField] private float pulseScale = 1.3f;
    [SerializeField] private float pulseTime = 0.12f;

    private Vector3 normalScale;
    private Coroutine pulseRoutine;

    private void Awake()
    {
        normalScale = scoreText.localScale;
    }

    private void OnEnable()
    {
        GameEvents.ScoreChanged += OnScoreChanged;
    }

    private void OnDisable()
    {
        GameEvents.ScoreChanged -= OnScoreChanged;
    }

    private void OnScoreChanged(int score)
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        pulseRoutine = StartCoroutine(Pulse());
    }

    private IEnumerator Pulse()
    {
        Vector3 largeScale = normalScale * pulseScale;
        float time = 0f;

        while (time < pulseTime)
        {
            time += Time.deltaTime;
            scoreText.localScale =
                Vector3.Lerp(normalScale, largeScale, time / pulseTime);
            yield return null;
        }

        time = 0f;

        while (time < pulseTime)
        {
            time += Time.deltaTime;
            scoreText.localScale =
                Vector3.Lerp(largeScale, normalScale, time / pulseTime);
            yield return null;
        }

        scoreText.localScale = normalScale;
        pulseRoutine = null;
    }
}
