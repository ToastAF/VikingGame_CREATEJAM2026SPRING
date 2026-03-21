using UnityEngine;

public abstract class EnemyAttack : MonoBehaviour
{
    public float attackRange = 1.5f;

    public virtual bool InRange(Transform target)
    {
        return Vector2.Distance(transform.position, target.position) <= attackRange;
    }

    public abstract void ExecuteAttack(Transform target, float damage);
}