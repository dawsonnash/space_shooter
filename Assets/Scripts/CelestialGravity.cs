using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CelestialGravity : MonoBehaviour
{
    [Header("Gravitational Settings")]
    [Tooltip("Gravity strength at the center of mass. 0 = none, 10 = small planet, 300 = sun, 500+ = black hole")]
    public float gravity = 100f;

    [Tooltip("The maximum radius at which gravity has any effect.")]
    public float maxGravitationalRadius = 50f;

    void FixedUpdate()
    {
        // Apply gravity to all rigidbodies within range
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, maxGravitationalRadius);

        foreach (Collider2D hit in hits)
        {
            Rigidbody2D rb = hit.attachedRigidbody;

            if (rb != null && rb.gameObject != this.gameObject)
            {
                Vector2 direction = (transform.position - rb.transform.position);
                float distance = direction.magnitude;

                if (distance > maxGravitationalRadius || distance < 0.01f)
                    continue;

                float normalizedDistance = distance / maxGravitationalRadius;

                // Falloff: gravity strongest at center, zero at max radius
                float force = gravity * (1f - normalizedDistance);

                rb.AddForce(direction.normalized * force);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, maxGravitationalRadius);
    }
}
