using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerAttack : MonoBehaviour
{
    public Camera cam;
    public Transform rotator, attackPoint;
    Vector2 mousePlacement;

    public float damage;
    public GameObject sweepAttack;

    public void OnAttack(InputValue input)
    {
        GameObject temp = Instantiate(sweepAttack, attackPoint.position, rotator.rotation);
        temp.GetComponent<AttackDoDamage>().damage = damage;
    }
}
