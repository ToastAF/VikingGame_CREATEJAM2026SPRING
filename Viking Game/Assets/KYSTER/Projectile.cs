using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float damage;

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (!collision.CompareTag("Player")) return;

        IDamageable target = collision.GetComponent<IDamageable>();

        if (target != null)
        {
            target.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}