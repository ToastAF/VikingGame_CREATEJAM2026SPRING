using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float detectionRange = 6f;
    public float moveSpeed = 3f;

    private Transform player;
    private Rigidbody2D rb;
    private Enemy enemy;
    private EnemyAttack attack;

    private enum State
    {
        Idle,
        Chase,
        Attack
    }

    private State currentState;

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

        float distance = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Idle:
                if (distance < detectionRange)
                    currentState = State.Chase;
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
                    MoveTowardsPlayer();
                }
                break;

            case State.Attack:
                enemy.TryAttack(player);

                if (attack != null && !attack.InRange(player))
                {
                    currentState = State.Chase;
                }
                break;
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}