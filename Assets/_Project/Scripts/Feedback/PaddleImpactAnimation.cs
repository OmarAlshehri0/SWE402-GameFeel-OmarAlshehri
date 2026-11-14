using System.Collections;
using UnityEngine;

public class PaddleImpactAnimation : MonoBehaviour
{
    [SerializeField] private Transform paddleVisual;

    private Vector3 normalScale;
    private Coroutine animationRoutine;

    private void Awake()
    {
        normalScale = paddleVisual.localScale;
    }

    private void OnEnable()
    {
        GameEvents.BallHitPaddle += AnimatePaddle;
    }

    private void OnDisable()
    {
        GameEvents.BallHitPaddle -= AnimatePaddle;
    }

    private void AnimatePaddle(ImpactInfo info)
    {
        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        animationRoutine = StartCoroutine(SquashAndReturn());
    }

    private IEnumerator SquashAndReturn()
    {
        Vector3 squashScale = new Vector3(
            normalScale.x * 1.2f,
            normalScale.y * 0.7f,
            normalScale.z
        );

        float time = 0f;

        while (time < 0.06f)
        {
            time += Time.deltaTime;
            paddleVisual.localScale =
                Vector3.Lerp(normalScale, squashScale, time / 0.06f);
            yield return null;
        }

        time = 0f;

        while (time < 0.12f)
        {
            time += Time.deltaTime;
            paddleVisual.localScale =
                Vector3.Lerp(squashScale, normalScale, time / 0.12f);
            yield return null;
        }

        paddleVisual.localScale = normalScale;
        animationRoutine = null;
    }
}
