using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public EnemyData data;

    private float currentHP;
    private float damage;
    private float lastAttackTime;

    private EnemyAttack attack;

    public GameObject bloodParticles, damageText;

    private void Start()
    {
        Initialize();
        // Prefer ranged attack when multiple EnemyAttack components exist
        attack = GetComponent<EnemyRangedAttack>();
        if (attack == null) attack = GetComponent<PounceAttack>();
        if (attack == null) attack = GetComponent<SweepAttack>();
        if (attack == null) attack = GetComponent<EnemyAttack>();
    }

    void Initialize()
    {
        float modifier = GameManager.Instance.GetModifier();

        currentHP = data.baseHP * modifier;
        damage = data.baseDamage * modifier;
    }

    public void TakeDamage(float amount)
    {
        Instantiate(bloodParticles, new Vector3(transform.position.x, transform.position.y, -1), Quaternion.identity);

        SpawnText(amount);


        currentHP -= amount;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void SpawnText(float dmg)
    {
        GameObject textObj = Instantiate(damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0.4f, 1f), 0), Quaternion.identity);
        TextMeshProUGUI textComp = textObj.GetComponentInChildren<Transform>().Find("DamageText").GetComponent<TextMeshProUGUI>(); //We get the component here
        textComp.text = dmg.ToString();
    }

    public void TryAttack(Transform target)
    {
        if (attack == null) return;

        // 🔴 cooldown
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