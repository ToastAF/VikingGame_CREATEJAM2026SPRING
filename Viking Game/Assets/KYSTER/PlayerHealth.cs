using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public float maxHP = 200f;
    private float currentHP;

    public Image hpBar;
    public GameObject bloodParticles, damageText;

    bool canTakeDmg = true;

    BoonHandler boonHandler;
    bool canRegen = true;

    private void Start()
    {
        currentHP = maxHP;
        boonHandler = GetComponent<BoonHandler>();
    }

    private void Update()
    {
        hpBar.fillAmount = currentHP / maxHP;

        if (canRegen && boonHandler.boons[1].level > 0 && currentHP <= maxHP)
        {
            StartCoroutine(Regen());
        }

        Mathf.Clamp(currentHP, 0, maxHP);
    }

    public void TakeDamage(float amount)
    {
        if (canTakeDmg == true)
        {
            Instantiate(bloodParticles, new Vector3(transform.position.x, transform.position.y, -1), Quaternion.identity);

            SpawnText(amount);

            currentHP -= amount;

            Debug.Log("Player HP: " + currentHP);

            StartCoroutine(TakeDmgCD());
        }

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
        GameObject textObj = Instantiate(damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0.4f, 1f), -5), Quaternion.identity);
        TextMeshProUGUI textComp = textObj.GetComponentInChildren<Transform>().Find("DamageText").GetComponent<TextMeshProUGUI>(); //We get the component here
        textComp.text = dmg.ToString();
    }

    void SpawnHealText(float hp)
    {
        GameObject textObj = Instantiate(damageText, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0.4f, 1f), -5), Quaternion.identity);
        TextMeshProUGUI textComp = textObj.GetComponentInChildren<Transform>().Find("DamageText").GetComponent<TextMeshProUGUI>(); //We get the component here
        textComp.text = "+" + hp.ToString();
        textComp.color = Color.green;
    }

    IEnumerator TakeDmgCD()
    {
        canTakeDmg = false;
        yield return new WaitForSeconds(0.3f);
        canTakeDmg = true;
    }

    IEnumerator Regen()
    {
        canRegen = false;
        float healthToHeal = 10 * boonHandler.boons[1].level;
        currentHP += healthToHeal;
        SpawnHealText(healthToHeal);

        yield return new WaitForSeconds(1);
        canRegen = true;
    }
}