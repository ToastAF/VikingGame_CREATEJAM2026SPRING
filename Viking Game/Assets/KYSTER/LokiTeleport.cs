using System.Collections;
using UnityEngine;

public class LokiTeleport : MonoBehaviour
{
    [Tooltip("How often Loki teleports to the player (seconds)")]
    public float teleportInterval = 3f;

    [Tooltip("Preferred radius around the player to appear (tries multiple samples)")]
    public float teleportRadius = 1f;

    [Tooltip("If true, teleport away from player when closer than dangerDistance")]
    public bool teleportAwayWhenClose = true;

    [Tooltip("Distance under which Loki considers himself in danger and will teleport away")]
    public float dangerDistance = 2f;

    [Tooltip("Layer mask used to detect obstacles when choosing a teleport spot")]
    public LayerMask obstacleMask = 0;
    public LayerMask walkableMask = 0;

    [Tooltip("Optional VFX to spawn at origin and destination")]
    public GameObject teleportEffectPrefab;

    private Transform player;
    private EnemyAttack attack;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        attack = GetComponent<EnemyAttack>();

        StartCoroutine(TeleportRoutine());
    }

    IEnumerator TeleportRoutine()
    {
        // initial delay before first teleport so it happens once every interval
        yield return new WaitForSeconds(teleportInterval);

        while (true)
        {
            if (player != null)
            {
                // If currently pouncing, skip teleport until next interval
                if (attack is PounceAttack p && p.IsPouncing())
                {
                    // skip this cycle
                }
                else
                {
                    float dist = Vector2.Distance(transform.position, player.position);

                    // Only teleport when player is too close and teleportAwayWhenClose is enabled
                    if (teleportAwayWhenClose && dist <= dangerDistance)
                    {
                        TryTeleportToPlayer(true);
                    }
                    // otherwise do not teleport every interval (prevents constant teleporting)
                }
            }

            yield return new WaitForSeconds(teleportInterval);
        }
    }

    void TryTeleportToPlayer()
    {
        TryTeleportToPlayer(false);
    }

    void TryTeleportToPlayer(bool away)
    {
        Vector2 playerPos = player.position;

        // sample points around the player to find a non-obstructed position
        Vector2 chosenPos = playerPos;
        const int attempts = 12;
        float checkRadius = 0.2f;

        for (int i = 0; i < attempts; i++)
        {
            Vector2 candidate;

            if (away)
            {
                Vector2 dirFromPlayer = (Vector2)transform.position - playerPos;
                if (dirFromPlayer.sqrMagnitude < 0.0001f)
                {
                    float a = Random.Range(0f, Mathf.PI * 2f);
                    dirFromPlayer = new Vector2(Mathf.Cos(a), Mathf.Sin(a));
                }

                dirFromPlayer.Normalize();
                candidate = playerPos + dirFromPlayer * teleportRadius;
            }
            else
            {
                float angle = Random.Range(0f, Mathf.PI * 2f);
                Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * teleportRadius;
                candidate = playerPos + offset;
            }

            // if obstacleMask is zero, the check will be skipped (no mask)
            bool blocked = obstacleMask != 0 && Physics2D.OverlapCircle(candidate, checkRadius, obstacleMask) != null;

            bool isOnWalkable = walkableMask != 0 && Physics2D.OverlapCircle(candidate, checkRadius * 0.5f, walkableMask) != null;
            if (!blocked && isOnWalkable)
            {
                chosenPos = candidate;
                break;
            }
        }

        // stop any Rigidbody2D movement so teleport is crisp
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        // optional VFX at origin
        if (teleportEffectPrefab != null)
            Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);

        transform.position = new Vector3(chosenPos.x, chosenPos.y, transform.position.z);

        // ensure enemy is no longer stunned after teleport (player knockback/stun shouldn't persist)
        EnemyAI ai = GetComponent<EnemyAI>();
        if (ai != null)
            ai.isStunned = false;

        // optional VFX at destination
        if (teleportEffectPrefab != null)
            Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, 0.2f);

        if (player != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(player.position, teleportRadius);
        }
    }
}
