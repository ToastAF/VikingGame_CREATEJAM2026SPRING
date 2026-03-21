using UnityEngine;

public class PounceAttack : EnemyAttack
{
    public float pounceDistance = 5f;
    public float pounceDuration = 0.4f;
    public float impactRadius = 1f;

    private bool isPouncing = false;

    private Vector2 startPos;
    private Vector2 targetPos;

    private float elapsedTime;
    private float damageStored;

    public override bool InRange(Transform target)
    {
        float dist = Vector2.Distance(transform.position, target.position);
        return dist <= pounceDistance;
    }

    public override void ExecuteAttack(Transform target, float damage)
    {
        if (isPouncing) return;

        Debug.Log("POUNCE TRIGGERED");

        isPouncing = true;

        startPos = transform.position;
        targetPos = target.position; // snapshot

        elapsedTime = 0f;
        damageStored = damage;
    }

    void Update()
    {
        if (!isPouncing) return;

        elapsedTime += Time.deltaTime;
        float t = elapsedTime / pounceDuration;

        // movement
        Vector2 pos = Vector2.Lerp(startPos, targetPos, t);

        // arc
        float height = 1.5f * 4 * (t - t * t);
        pos.y += height;

        transform.position = pos;

        if (t >= 1f)
        {
            Impact();
            isPouncing = false;
        }
    }

    void Impact()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, impactRadius);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;

            IDamageable dmg = hit.GetComponent<IDamageable>();
            if (dmg != null)
            {
                dmg.TakeDamage(damageStored);
            }
        }

        Debug.Log("POUNCE IMPACT");
    }

    public bool IsPouncing()
    {
        return isPouncing;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pounceDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }
}