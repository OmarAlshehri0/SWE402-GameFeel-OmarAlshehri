using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float duration = 0.06f;
    [SerializeField] private float strength = 0.08f;

    private Vector3 originalPosition;
    private Coroutine shakeRoutine;

    private void Awake()
    {
        originalPosition = transform.localPosition;
    }

    private void Update()
    {
        // GameEndUI pauses time at game over or win.
        if (Time.timeScale == 0f)
        {
            StopShake();
        }
    }

    private void OnEnable()
    {
        GameEvents.BallHitPaddle += ShakeFromImpact;
        GameEvents.BrickDestroyed += ShakeFromImpact;
    }

    private void OnDisable()
    {
        GameEvents.BallHitPaddle -= ShakeFromImpact;
        GameEvents.BrickDestroyed -= ShakeFromImpact;
        StopShake();
    }

    private void ShakeFromImpact(ImpactInfo info)
    {
        if (Time.timeScale == 0f) return;

        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
        }

        shakeRoutine = StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;

            transform.localPosition =
                originalPosition +
                Random.insideUnitSphere * strength;

            yield return null;
        }

        StopShake();
    }

    private void StopShake()
    {
        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
            shakeRoutine = null;
        }

        transform.localPosition = originalPosition;
    }
}
