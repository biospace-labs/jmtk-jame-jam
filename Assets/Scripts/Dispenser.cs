using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Dispenser : Useable
{
    public SpriteRenderer[] itemDisplays;
    public GameObject toDispense;
    public Image indicator;
    public float timeToDispense = 1;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var itemDisplay in itemDisplays)
            itemDisplay.sprite = toDispense.GetComponent<SpriteRenderer>().sprite;
        indicator.fillAmount = 0f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void BeginUse()
    { 
        StartCoroutine(nameof(HoldToDispense));
    }

    public override void EndUse()
    {
        StopCoroutine(nameof(HoldToDispense));
        indicator.fillAmount = 0f;
    }

    public void DispenseItem()
    {
        Instantiate(toDispense, gameObject.transform.position + Vector3.up, Quaternion.identity);
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
