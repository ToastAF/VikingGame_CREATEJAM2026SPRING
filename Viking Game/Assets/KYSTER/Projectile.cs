using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 startPos;
    private Vector2 targetPos;

    private float damage;
    private float travelTime;
    private float elapsedTime;

    public float arcHeight = 2f; // Hvor høj buen er
    public LayerMask targetLayer; // Hvilke layers kan rammes
    public float hitRadius = 0.3f; // radius for overlap check

    // Init fra RangedAttack
    public void Init(Vector2 target, float dmg, float time, LayerMask layer)
    {
        startPos = transform.position;
        targetPos = target;
        damage = dmg;
        travelTime = time;
        targetLayer = layer;
        elapsedTime = 0f;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / travelTime);

        // Base linear movement
        Vector2 linearPos = Vector2.Lerp(startPos, targetPos, t);

        // Arc (parabel)
        float height = arcHeight * 4 * (t - t * t);
        Vector2 finalPos = new Vector2(linearPos.x, linearPos.y + height);

        transform.position = finalPos;

        if (t >= 1f)
        {
            Hit();
        }
    }

    void Hit()
    {
        // Overlap med target layer
        Collider2D hit = Physics2D.OverlapCircle(transform.position, hitRadius, targetLayer);
        if (hit != null)
        {
            IDamageable dmgComp = hit.GetComponent<IDamageable>();
            if (dmgComp != null)
            {
                dmgComp.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
    }
}