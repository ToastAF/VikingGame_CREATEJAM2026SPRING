using UnityEngine;

public class AttackDoDamage : MonoBehaviour
{
    public float damage;
    public float knockbackForce = 1.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().TakeDamage(damage);
            Debug.Log("I hit the thing!");

            collision.GetComponent<EnemyAI>().StunEnemy(0.5f);

            Vector2 direction = (collision.transform.position - transform.position).normalized;
            collision.GetComponent<Rigidbody2D>().AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        }
    }
}
