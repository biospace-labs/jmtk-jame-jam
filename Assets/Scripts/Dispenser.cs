using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine.UI;
using UnityEngine;

public class Dispenser : Useable
{
    public SpriteRenderer itemDisplay;
    public GameObject toDispense;
    public Image indicator;
    public float timeToDispense = 1;
    // Start is called before the first frame update
    void Start()
    {

        itemDisplay.sprite = toDispense.GetComponent<SpriteRenderer>().sprite;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void BeginUse()
    { 
        StartCoroutine("HoldToDispense");
    }

    public override void EndUse()
    {
        StopCoroutine("HoldToDispense");
        indicator.fillAmount = 0f;
    }

    public void DispenseItem()
    {
        Instantiate(toDispense, gameObject.transform.position, Quaternion.identity);
    }

    private IEnumerator HoldToDispense()
    {
        float elapsedTime = 0f;
        while (elapsedTime < timeToDispense)
        {
            elapsedTime += Time.deltaTime;
            indicator.fillAmount = elapsedTime / timeToDispense;

            yield return null;
        }

        DispenseItem();
    }
}
