using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageTextMover : MonoBehaviour
{
    Vector3 normalScale = new Vector3(1, 1, 1);
    Vector3 bigScale = new Vector3(1.5f, 1.5f, 1.5f);

    RectTransform rectTransform;
    float moveUpSpeed;
    public float speed1, speed2, bigSmallSpeed, timeToGetBigBeforeSmall;
    bool getSmall = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        StartCoroutine(ChangeScale(timeToGetBigBeforeSmall));
        moveUpSpeed = Random.Range(speed1, speed2);
    }

    void Update()
    {
        rectTransform.localPosition += new Vector3(0, moveUpSpeed * Time.deltaTime, 0);

        if(getSmall == true)
        {
            rectTransform.localScale -= normalScale * Time.deltaTime * bigSmallSpeed;
        }
        else
        {
            rectTransform.localScale += normalScale * Time.deltaTime * bigSmallSpeed * 5;
        }
    }

    IEnumerator ChangeScale(float delay)
    {
        yield return new WaitForSeconds(delay);
        getSmall = true;
    }
}
