using UnityEngine;

public class SweepAttack : EnemyAttack
{
    public float radius = 1.5f;

    public override void ExecuteAttack(Transform target, float damage)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var hit in hits)
        {
          
            if (!hit.CompareTag("Player")) continue;

            IDamageable dmg = hit.GetComponent<IDamageable>();
            if (dmg != null)
            {
                dmg.TakeDamage(damage);
            }
        }

        Debug.Log("Sweep attack");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}