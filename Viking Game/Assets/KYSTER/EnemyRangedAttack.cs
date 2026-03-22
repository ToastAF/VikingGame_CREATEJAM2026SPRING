using UnityEngine;

public class EnemyRangedAttack : EnemyAttack
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab; // Prefab med Projectile.cs
    public float projectileTravelTime = 0.5f;
    public float arcHeight = 2f;

    public LayerMask hitLayers;
    public bool daggerProjectile = false;
    [Tooltip("Speed of dagger projectiles when daggerProjectile is enabled")]
    public float daggerSpeed = 14f;
    [Tooltip("Degrees to offset projectile sprite so its 'point' faces the flight direction")]
    public float projectileRotationOffset = -45f;

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
        DaggerThrow tempScript = projGO.GetComponent<DaggerThrow>();
        if(tempScript != null)
        {
            tempScript.damage = damage;
            tempScript.target = targetPos;
        }


        // Init projectile
            Projectile proj = projGO.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.arcHeight = arcHeight;
            proj.Init(targetPos, damage, projectileTravelTime, hitLayers, daggerProjectile);
            if (daggerProjectile)
                proj.daggerSpeed = daggerSpeed;

            // set rotation so the sprite points along flight direction using vector assignment
            Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
            // Use transform.up so sprites that point 'up' face the direction; adjust with offset
            projGO.transform.up = dir;
            projGO.transform.Rotate(0f, 0f, projectileRotationOffset);
        }

        // Stop attacking næste frame
        isAttacking = false;
    }
}