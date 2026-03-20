using UnityEngine;

public class RangedAttack : EnemyAttack
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;

    public override void ExecuteAttack(Transform target, float damage)
    {
        if (projectilePrefab == null) return;

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        Vector2 dir = (target.position - transform.position).normalized;

        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = dir * projectileSpeed;
        }

        Projectile projectile = proj.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetDamage(damage);
        }

        Debug.Log("Ranged attack");
    }
}