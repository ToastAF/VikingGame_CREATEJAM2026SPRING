using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float detectionRange = 6f;
    public float moveSpeed = 3f;
    [Tooltip("Layers that block line of sight (walls, obstacles)")]
    public LayerMask sightBlockingMask = 0;

    [Tooltip("How long the player must be continuously visible before the enemy becomes alerted")]
    public float aggroDelay = 0.5f;

    [Tooltip("Debug: show LOS raycast in Scene view")]
    public bool debugLOS = false;
    [Tooltip("Preferred distance to keep from the player when using ranged attacks")]
    public float rangedPreferredDistance = 6f;

    [Tooltip("Tolerance around preferred distance where the enemy will hold position")]
    public float rangedDistanceBuffer = 1f;

    private Transform player;
    private Rigidbody2D rb;
    private Enemy enemy;
    private EnemyAttack attack;

    public bool isStunned = false;

    private enum State
    {
        Idle,
        Chase,
        Attack
    }

    private State currentState;

    private float seenTimer = 0f;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;

        rb = GetComponent<Rigidbody2D>();
        enemy = GetComponent<Enemy>();
        attack = GetComponent<EnemyAttack>();
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // 🔹 STOP ALT hvis vi er pouncing
        if (attack is PounceAttack pounce && pounce.IsPouncing())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        // 🔹 kun kør AI hvis ikke stunned
        if (!isStunned)
        {
            switch (currentState)
            {
                case State.Idle:
                    // require the player to be within detection range AND visible for a short time
                    if (distance < detectionRange && HasLineOfSight())
                    {
                        seenTimer += Time.fixedDeltaTime;
                        if (seenTimer >= aggroDelay)
                        {
                            currentState = State.Chase;
                            seenTimer = 0f;
                        }
                    }
                    else
                    {
                        seenTimer = 0f;
                    }
                    break;

                case State.Chase:
                    if (attack != null && attack.InRange(player))
                    {
                        currentState = State.Attack;
                        rb.linearVelocity = Vector2.zero;
                    }
                    else if (distance > detectionRange)
                    {
                        currentState = State.Idle;
                    }
                    else
                    {
                        if (attack is EnemyRangedAttack)
                        {
                            MaintainRangedDistance(distance);
                        }
                        else
                        {
                            MoveTowardsPlayer();
                        }
                    }
                    break;

                case State.Attack:
                    enemy.TryAttack(player);

                    // 🔹 hold attack state under pounce
                    if (attack is PounceAttack p)
                    {
                        if (!p.IsPouncing() && !attack.InRange(player))
                        {
                            currentState = State.Chase;
                        }
                    }
                    else
                    {
                        if (attack != null && !attack.InRange(player))
                        {
                            currentState = State.Chase;
                        }

                        // If using ranged attack, try to maintain preferred distance while attacking
                        if (attack is EnemyRangedAttack)
                        {
                            MaintainRangedDistance(distance);
                        }
                    }
                    break;
            }
        }
    }

    void MaintainRangedDistance(float distance)
    {
        float min = Mathf.Max(0f, rangedPreferredDistance - rangedDistanceBuffer);
        float max = rangedPreferredDistance + rangedDistanceBuffer;

        Vector2 dir = (player.position - transform.position).normalized;

        if (distance < min)
        {
            // too close -> move away
            rb.linearVelocity = -dir * moveSpeed;
        }
        else if (distance > max)
        {
            // too far -> approach
            rb.linearVelocity = dir * moveSpeed;
        }
        else
        {
            // within preferred band -> stop moving
            rb.linearVelocity = Vector2.zero;
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }

    bool HasLineOfSight()
    {
        if (player == null) return false;

        Vector2 dir = (player.position - transform.position);
        float dist = dir.magnitude;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir.normalized, dist, sightBlockingMask);
        if (debugLOS)
        {
            Debug.DrawLine(transform.position, player.position, hit.collider == null ? Color.green : Color.red, 0.1f);
        }

        // If raycast hits nothing in blocking mask, we have LOS
        return hit.collider == null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    public void StunEnemy(float duration)
    {
        StartCoroutine(Stun(duration));
    }

    IEnumerator Stun(float cd)
    {
        isStunned = true;
        yield return new WaitForSeconds(cd);
        isStunned = false;
    }
}