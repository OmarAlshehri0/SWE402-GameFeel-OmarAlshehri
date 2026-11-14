using System.Collections;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private bool resolvingLoss;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out Ball ball))
            return;

        if (ball.TryGetComponent<BonusBall>(out _))
        {
            Destroy(ball.gameObject);
        }
        else
        {
            ball.gameObject.SetActive(false);
        }

        if (!resolvingLoss)
            StartCoroutine(ResolveBallLoss());
    }

    private IEnumerator ResolveBallLoss()
    {
        resolvingLoss = true;

        yield return new WaitForEndOfFrame();

        Ball[] remainingBalls =
            FindObjectsByType<Ball>(FindObjectsSortMode.None);

        if (remainingBalls.Length == 0)
        {
            GameManager.Instance.OnBallLost();
        }

        resolvingLoss = false;
    }
}
