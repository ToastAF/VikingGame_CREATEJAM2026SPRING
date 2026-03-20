using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using JetBrains.Annotations;

public class DestroySelf : MonoBehaviour
{
    public float destroyDelay = 0;

    private void Start()
    {
        if(destroyDelay > 0)
        {
            StartCoroutine(DestroySelfAfterSeconds(destroyDelay));
        }
    }

    public void DestroySelfInstantly()
    {
        Destroy(gameObject);
    }

    IEnumerator DestroySelfAfterSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
