using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BlackHole : MonoBehaviour
{
    [Header("Teleport Settings")]
    public bool teleportPlayer = true;
    public Vector2 teleportAreaMin = new Vector2(-100f, -100f);
    public Vector2 teleportAreaMax = new Vector2(100f, 100f);

    [Header("Effects")]
    public bool zeroVelocityOnTeleport = true;
    public ParticleSystem warpEffect;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (teleportPlayer && other.CompareTag("Player"))
        {
            // Teleport to a random position within defined area
            Vector2 newPosition = new Vector2(
                Random.Range(teleportAreaMin.x, teleportAreaMax.x),
                Random.Range(teleportAreaMin.y, teleportAreaMax.y)
            );
            other.transform.position = newPosition;

            // Zero out velocity if desired
            Rigidbody2D rb = other.attachedRigidbody;
            if (zeroVelocityOnTeleport && rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }

            // Optional particle effect
            if (warpEffect != null)
            {
                Instantiate(warpEffect, transform.position, Quaternion.identity);
            }

            Debug.Log("Player teleported by Black Hole.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, 2f); // Just a visual marker
    }
}
