using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPoint : Useable
{
    public GameObject prefabToBuild;
    public float buildTime = 1f;

    public override void BeginUse()
    {
        StartCoroutine("HoldToBuild");
    }

    public override void EndUse()
    {
        StopCoroutine("HoldToBuild");
    }

    private IEnumerator HoldToBuild()
    {
        float elapsedTime = 0f;
        while (elapsedTime < buildTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Instantiate(prefabToBuild, transform.position, transform.rotation);

        //TODO animate building
        
        StartCoroutine("DestroySelf");
    }

    private IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
