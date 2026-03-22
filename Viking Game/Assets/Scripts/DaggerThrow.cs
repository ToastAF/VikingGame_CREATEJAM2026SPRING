using Mono.Cecil;
using System.Collections;
using UnityEngine;

public class DaggerThrow : MonoBehaviour
{
    public float damage, daggerSpeed, projectileRotationOffset;
    Rigidbody2D rb;

    private Vector2 forwardDir = Vector2.right;
    public Vector2 target;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Vector2 startPos = transform.position;

        Vector2 dir = (target - (Vector2)transform.position).normalized;
        transform.up = dir;
        transform.Rotate(0f, 0f, projectileRotationOffset);

        //transform.Rotate(0f, 0f, projectileRotationOffset); // Rotate Object
        
        Vector2 diff = target - startPos;
        forwardDir = diff.sqrMagnitude > 0.0001f ? diff.normalized : Vector2.right;

        rb.linearVelocity = forwardDir * daggerSpeed;

        StartCoroutine(KillAfterTime());
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    IEnumerator KillAfterTime()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}
