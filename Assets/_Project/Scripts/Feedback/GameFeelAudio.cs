using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameFeelAudio : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioClip paddleSound;
    private AudioClip brickSound;
    private AudioClip lossSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        paddleSound = CreateTone("Paddle", 420f, 0.08f, 0.35f);
        brickSound = CreateTone("Brick", 820f, 0.12f, 0.45f);
        lossSound = CreateTone("BallLoss", 150f, 0.35f, 0.5f);
    }

    private void OnEnable()
    {
        GameEvents.BallHitPaddle += PlayPaddleSound;
        GameEvents.BrickDestroyed += PlayBrickSound;
        GameEvents.BallLost += PlayLossSound;
    }

    private void OnDisable()
    {
        GameEvents.BallHitPaddle -= PlayPaddleSound;
        GameEvents.BrickDestroyed -= PlayBrickSound;
        GameEvents.BallLost -= PlayLossSound;
    }

    private void PlayPaddleSound(ImpactInfo info)
    {
        audioSource.PlayOneShot(paddleSound);
    }

    private void PlayBrickSound(ImpactInfo info)
    {
        audioSource.PlayOneShot(brickSound);
    }

    private void PlayLossSound()
    {
        audioSource.PlayOneShot(lossSound);
    }

    private AudioClip CreateTone(
        string clipName,
        float frequency,
        float duration,
        float volume)
    {
        int sampleRate = 44100;
        int sampleCount = Mathf.CeilToInt(sampleRate * duration);

        AudioClip clip = AudioClip.Create(
            clipName,
            sampleCount,
            1,
            sampleRate,
            false);

        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float time = i / (float)sampleRate;
            float fade = 1f - (i / (float)sampleCount);

            samples[i] =
                Mathf.Sin(2f * Mathf.PI * frequency * time)
                * volume
                * fade;
        }

        clip.SetData(samples, 0);
        return clip;
    }
}
