using UnityEngine;

public class EnemyRangedAttack : EnemyAttack
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;

    public override void ExecuteAttack(Transform target, float damage)
    {
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        Vector2 dir = (target.position - transform.position).normalized;

        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        rb.linearVelocity = dir * projectileSpeed;

        Projectile projectile = proj.GetComponent<Projectile>();
        projectile.SetDamage(damage);

        Debug.Log("Ranged attack");
    }
}