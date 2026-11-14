using UnityEngine;

public class PortalGate : MonoBehaviour
{
    [SerializeField] private PortalGate linkedPortal;
    [SerializeField] private float teleportCooldown = 0.2f;

    private float lastTeleportTime = -10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out Ball ball))
            return;

        if (linkedPortal == null)
            return;

        if (Time.time - lastTeleportTime < teleportCooldown)
            return;

        lastTeleportTime = Time.time;
        linkedPortal.lastTeleportTime = Time.time;

        Vector3 exitPosition =
            linkedPortal.transform.position;

        exitPosition.y += 0.6f;

        ball.transform.position = exitPosition;
    }
}
