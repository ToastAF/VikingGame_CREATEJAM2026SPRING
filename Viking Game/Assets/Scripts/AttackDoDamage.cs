using UnityEngine;
using System.Linq;

public class AttackDoDamage : MonoBehaviour
{
    public float damage;
    public float knockbackForce = 1.5f;
    public GameObject bloodParticles, lightningAttack;
    public BoonHandler boonHandler;

    public float searchRadius = 10f;
    public LayerMask enemyLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().TakeDamage(damage);
            Debug.Log("I hit the thing!");


            if (boonHandler.boons[0].level > 0) //If player has a level in the Thor rune
            {
                GameObject secondNearestEnemy = FindSecondNearestEnemy();
                GameObject tempLightning = Instantiate(lightningAttack);
                LightningLogic lightningScript = tempLightning.GetComponent<LightningLogic>();
                lightningScript.damage = damage * (boonHandler.boons[0].level + 1); //Does more damage based on Thor rune level
                lightningScript.CreateArc(collision.gameObject, secondNearestEnemy); //ACTUALLY DO LIGHTNING!!!
            }

            collision.GetComponent<EnemyAI>().StunEnemy(0.5f);

            Vector2 direction = (collision.transform.position - transform.position).normalized;
            collision.GetComponent<Rigidbody2D>().AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        }
    }

    public GameObject FindSecondNearestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, searchRadius, enemyLayer);

        var secondNearest = hits
            .Select(hit => hit.transform)
            .OrderBy(t => Vector2.Distance(transform.position, t.position))
            .Skip(1) // Skips nearest enemy
            .FirstOrDefault();

        return secondNearest.gameObject;
    }
}
