using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 startPos;
    private Vector2 targetPos;

    private float damage;
    private float travelTime;
    private float elapsedTime;
    private Rigidbody2D rb;

    public float arcHeight = 2f; // Hvor høj buen er
    // When `daggerMode` is true the projectile behaves like a dagger: it
    // persists until hitting any collider on `hitLayers` (player, walls, etc.).
    // When false it behaves like the original projectile: it follows the arc
    // and deals damage on arrival if it overlaps `targetLayer`.
    public bool daggerMode = false;
    public LayerMask hitLayers; // Layers that will break the dagger (player, walls, etc.)
    public LayerMask targetLayer; // Used when daggerMode == false (original behavior)
    public float hitRadius = 0.3f; // radius for overlap check
    public float maxLifetime = 10f; // safety destroy if nothing is hit
    public float daggerSpeed = 14f; // forward speed after arrival when daggerMode

    private bool travelComplete = false;
    private float lifeTimer = 0f;
    private Vector2 forwardDir = Vector2.right;
    private bool flyingForward = false;

    // Init fra RangedAttack
    public void Init(Vector2 target, float dmg, float time, LayerMask layers, bool dagger = false)
    {
        startPos = transform.position;
        targetPos = target;
        damage = dmg;
        travelTime = time;
        daggerMode = dagger;
        if (daggerMode)
            hitLayers = layers;
        else
            targetLayer = layers;
        elapsedTime = 0f;
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.rotation = 0f;
        }
        Vector2 diff = targetPos - startPos;
        forwardDir = diff.sqrMagnitude > 0.0001f ? diff.normalized : Vector2.right;
        if (daggerMode)
        {
            // skip arc and start flying directly toward target using velocity
            travelComplete = true;
            flyingForward = true;
            lifeTimer = 0f;
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 0f;
                rb.linearVelocity = forwardDir * daggerSpeed;
            }
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (!travelComplete)
        {
            float t = Mathf.Clamp01(elapsedTime / travelTime);

            // Base linear movement
            Vector2 linearPos = Vector2.Lerp(startPos, targetPos, t);

            // Arc (parabel)
            float height = arcHeight * 4 * (t - t * t);
            Vector2 finalPos = new Vector2(linearPos.x, linearPos.y + height);

            if (rb != null)
                rb.MovePosition(finalPos);
            else
                transform.position = finalPos;

            if (daggerMode)
            {
                // dagger checks collisions continuously while on arc
                Vector2 curPos = rb != null ? rb.position : (Vector2)transform.position;
                if (CheckForHitAtPosition(curPos)) return;
            }

            if (t >= 1f)
            {
                // arrival: stop moving
                travelComplete = true;
                lifeTimer = 0f;

                if (daggerMode)
                {
                    flyingForward = true;
                }
                else
                {
                    // original behavior: deal damage and destroy on arrival
                    Hit();
                    return;
                }
            }
        }
        else
        {
            if (daggerMode)
            {
                // continue flying forward until hitting something
                float moveDist = daggerSpeed * Time.deltaTime;
                Vector2 curPos = rb != null ? rb.position : (Vector2)transform.position;

                RaycastHit2D hit = Physics2D.CircleCast(curPos, hitRadius, forwardDir, moveDist, hitLayers);
                if (hit.collider != null)
                {
                    IDamageable dmgComp = hit.collider.GetComponent<IDamageable>();
                    if (dmgComp != null) dmgComp.TakeDamage(damage);

                    if (rb != null) rb.MovePosition(hit.point);
                    else transform.position = hit.point;

                    Destroy(gameObject);
                    return;
                }

                Vector2 nextPos = curPos + forwardDir * moveDist;
                if (rb != null) rb.MovePosition(nextPos);
                else transform.position = nextPos;

                lifeTimer += Time.deltaTime;
                if (lifeTimer >= maxLifetime)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                // non-dagger after arrival; nothing to do (Hit already handled)
            }
        }
    }

    bool CheckForHitAtPosition(Vector2 pos)
    {
        Collider2D hit = Physics2D.OverlapCircle(pos, hitRadius, hitLayers);
        if (hit != null)
        {
            IDamageable dmgComp = hit.GetComponent<IDamageable>();
            if (dmgComp != null)
            {
                dmgComp.TakeDamage(damage);
            }

            Destroy(gameObject);
            return true;
        }

        return false;
    }

    void Hit()
    {
        // Original arrival behavior
        Collider2D hit = Physics2D.OverlapCircle(transform.position, hitRadius, targetLayer);
        if (hit != null)
        {
            IDamageable dmgComp = hit.GetComponent<IDamageable>();
            if (dmgComp != null)
            {
                dmgComp.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitRadius);
    }
}