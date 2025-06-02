using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public float pullStrength = 5f;
    public float pullRadius = 10f;

    void FixedUpdate()
    {
        // Find all Rigidbody2D objects in the scene
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pullRadius);

        foreach (Collider2D hit in hits)
        {
            Rigidbody2D rb = hit.attachedRigidbody;

            if (rb != null && rb.gameObject != this.gameObject)
            {
                Vector2 direction = (transform.position - rb.transform.position).normalized;
                float distance = Vector2.Distance(transform.position, rb.transform.position);

                float force = pullStrength / Mathf.Max(distance, 0.1f); // avoid divide-by-zero

                rb.AddForce(direction * force);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualize pull radius in editor
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, pullRadius);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Teleport to a fixed location or generate a random one
            Vector2 newPosition = new Vector2(Random.Range(-100f, 100f), Random.Range(-100f, 100f));
            other.transform.position = newPosition;

            // Optional: zero out velocity so player doesn't fly away immediately
            Rigidbody2D rb = other.attachedRigidbody;
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

        }
    }

}
