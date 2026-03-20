using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public EnemyData data;

    private float currentHP;
    private float damage;
    private float lastAttackTime;

    private EnemyAttack attack;

    private void Start()
    {
        Initialize();
        attack = GetComponent<EnemyAttack>();
    }

    void Initialize()
    {
        float modifier = GameManager.Instance.GetModifier();

        currentHP = data.baseHP * modifier;
        damage = data.baseDamage * modifier;
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;

        Debug.Log(gameObject.name + " HP: " + currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void TryAttack(Transform target)
    {
        if (attack == null) return;

        if (Time.time - lastAttackTime < data.attackCooldown)
            return;

        if (attack.InRange(target))
        {
            attack.ExecuteAttack(target, damage);
            lastAttackTime = Time.time;
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}