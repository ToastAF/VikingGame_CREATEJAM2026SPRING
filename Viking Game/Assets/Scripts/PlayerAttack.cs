using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerAttack : MonoBehaviour
{
    public Camera cam;
    public Transform rotator, attackPoint;
    Vector2 mousePlacement;

    PlayerMove playerMoveScript;

    public float damage;
    public float attackDelay = 1;
    public GameObject sweepAttack;

    bool canAttack = true;

    private void Start()
    {
        playerMoveScript = GetComponent<PlayerMove>();
    }

    public void OnAttack(InputValue input)
    {
        if (canAttack)
        {
            GameObject temp = Instantiate(sweepAttack, attackPoint.position, rotator.rotation);
            temp.GetComponent<AttackDoDamage>().damage = damage;

            StartCoroutine(AttackCD(attackDelay));
        }
    }

    IEnumerator AttackCD(float cd)
    {
        canAttack = false;
        playerMoveScript.cannotMove = true;
        yield return new WaitForSeconds(0.2f);

        playerMoveScript.cannotMove = false;

        yield return new WaitForSeconds(cd);
        canAttack = true;
    }
}
