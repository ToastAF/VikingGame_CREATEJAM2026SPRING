using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerAttack : MonoBehaviour
{
    public Camera cam;
    public Transform rotator, attackPoint;
    Vector2 mousePlacement;

    PlayerMove playerMoveScript;
    SpriteRenderer spriteRenderer;
    Animator anim;
    BoonHandler boonHandler;

    public float damage;
    public float attackDelay = 1;
    public float moveAfterAttackDelay = 0.5f;
    public GameObject sweepAttack;

    bool canAttack = true;

    private void Start()
    {
        playerMoveScript = GetComponent<PlayerMove>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        boonHandler = GetComponent<BoonHandler>();
    }

    public void OnAttack(InputValue input)
    {
        if (canAttack)
        {
            GameObject temp = Instantiate(sweepAttack, attackPoint.position, rotator.rotation);
            AttackDoDamage tempScript = temp.GetComponent<AttackDoDamage>();
            tempScript.damage = damage;
            tempScript.boonHandler = boonHandler;

        
            
            canAttack = false;
            //Flip player based on attacking direction
            if (Mouse.current.position.ReadValue().x > Screen.width / 2.0f)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }
            StartCoroutine(AttackCD(attackDelay));
        }
    }

    IEnumerator AttackCD(float cd)
    {
 
        playerMoveScript.cannotMove = true;
        anim.SetBool("IsAttacking", true);

        yield return new WaitForSeconds(moveAfterAttackDelay);

        anim.SetBool("IsAttacking", false);
        playerMoveScript.cannotMove = false;

        yield return new WaitForSeconds(cd);


        canAttack = true;
    }
}
