using UnityEngine;

public class EnemyRangedAttack : EnemyAttack
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab; // Prefab med Projectile.cs
    public float projectileTravelTime = 0.5f;
    public float arcHeight = 2f;

    public LayerMask targetLayer;

    private bool isAttacking = false;

    public override bool InRange(Transform target)
    {
        // Her kan du sætte max range
        float dist = Vector2.Distance(transform.position, target.position);
        return dist <= 10f; // juster som ønsket
    }

    public override void ExecuteAttack(Transform target, float damage)
    {
        if (isAttacking) return;

        isAttacking = true;

        // Snapshoot player position
        Vector2 targetPos = target.position;

        // Instantiate projectile
        GameObject projGO = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        // Init projectile
        Projectile proj = projGO.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.arcHeight = arcHeight;
            proj.Init(targetPos, damage, projectileTravelTime, targetLayer);
        }

        // Stop attacking næste frame
        isAttacking = false;
    }
}