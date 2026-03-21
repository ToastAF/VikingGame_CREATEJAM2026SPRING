using UnityEngine;

public class LightningLogic : MonoBehaviour
{
    private LineRenderer line;

    [Header("Settings")]
    public int segments = 10;
    public float jitterAmount = 0.5f;
    public float arcDuration = 0.2f;

    public float damage;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = segments;
        line.enabled = false; // Keep it hidden until needed
    }

    public void CreateArc(GameObject start, GameObject end)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateArc(start, end));
    }

    private System.Collections.IEnumerator AnimateArc(GameObject start, GameObject end)
    {
        line.enabled = true;

        for (int i = 0; i < segments; i++)
        {
            float progress = (float)i / (segments - 1);
            Vector3 pos = Vector3.Lerp(start.transform.position, end.transform.position, progress);

            // Add randomness to the middle points, but keep start/end fixed
            if (i > 0 && i < segments - 1)
            {
                pos.x += Random.Range(-jitterAmount, jitterAmount);
                pos.y += Random.Range(-jitterAmount, jitterAmount);
            }

            line.SetPosition(i, pos);
        }

        //Also damage enemies
        start.GetComponent<Enemy>().TakeDamage(damage);
        end.GetComponent<Enemy>().TakeDamage(damage);


        yield return new WaitForSeconds(arcDuration);
        line.enabled = false;
    }
}