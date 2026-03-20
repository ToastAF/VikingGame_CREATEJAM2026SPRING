using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public float maxHP = 200f;
    private float currentHP;

    private void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;

        Debug.Log("Player HP: " + currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died");
        // Implement game over
    }
}