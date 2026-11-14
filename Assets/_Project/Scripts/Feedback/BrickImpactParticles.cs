using UnityEngine;

public class BrickImpactParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem effectPrefab;

    private void OnEnable()
    {
        GameEvents.BrickDestroyed += PlayEffect;
    }

    private void OnDisable()
    {
        GameEvents.BrickDestroyed -= PlayEffect;
    }

    private void PlayEffect(ImpactInfo info)
    {
        ParticleSystem effect =
            Instantiate(effectPrefab, info.Center, Quaternion.identity);

        var main = effect.main;
        main.startColor = info.Color;

        effect.Play();

        Destroy(
            effect.gameObject,
            main.duration + main.startLifetime.constantMax + 0.1f
        );
    }
}
