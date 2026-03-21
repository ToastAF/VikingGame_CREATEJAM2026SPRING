using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public float maxHP = 200f;
    private float currentHP;

    public Image hpBar;
    public GameObject bloodParticles, damageText;

    private void Start()
    {
        currentHP = maxHP;
    }

    private void Update()
    {
        hpBar.fillAmount = currentHP / maxHP;
    }

    public void TakeDamage(float amount)
    {
        Instantiate(bloodParticles, new Vector3(transform.position.x, transform.position.y, -1), Quaternion.identity);

        SpawnText(amount);

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

    void SpawnText(float dmg)
    {
        GameObject textObj = Instantiate(damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0.4f, 1f), 0), Quaternion.identity);
        TextMeshProUGUI textComp = textObj.GetComponentInChildren<Transform>().Find("DamageText").GetComponent<TextMeshProUGUI>(); //We get the component here
        textComp.text = dmg.ToString();
    }
}