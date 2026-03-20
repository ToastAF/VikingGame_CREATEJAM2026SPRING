using UnityEngine;

public class EnemySweepAttack : EnemyAttack
{
    public float radius = 1.5f;

    public override void ExecuteAttack(Transform target, float damage)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var hit in hits)
        {
            IDamageable dmg = hit.GetComponent<IDamageable>();

            if (dmg != null && hit.transform != transform)
            {
                dmg.TakeDamage(damage);
            }
        }

        Debug.Log("Sweep attack");
    }
}