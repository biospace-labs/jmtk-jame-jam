using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class BuildSite : Useable
{
    public BuildableBlueprint objToBuild;

    public Image buildIndicator;

    public List<ResourceItem> availableResources;
    private List<ResourceItem> _resourcesToUse = new();

    public void Start()
    {
        availableResources = new List<ResourceItem>();

        buildIndicator.fillAmount = 0;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"BuildPoint TriggerEnter {other.gameObject}");
        ResourceItem item = other.gameObject.GetComponent<ResourceItem>();
        if (item)
        {
            availableResources.Add(item);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"BuildPoint TriggerExit {other.gameObject}");
        ResourceItem item = other.gameObject.GetComponent<ResourceItem>();
        if (item)
        {
            availableResources.Remove(item);
        }
    }

    public override void BeginUse()
    {
        // check if we have enough resources
        var required = new List<ResourceType>(objToBuild.requiredResources);
        _resourcesToUse.Clear();
        foreach (var item in availableResources)
        {
            if (required.Remove(item.resourceType))
            {
                _resourcesToUse.Add(item);
            }
        }
        if (required.Count > 0) return;
        
        StartCoroutine("HoldToBuild");
    }

    public override void EndUse()
    {
        StopCoroutine("HoldToBuild");
        buildIndicator.fillAmount = 0f;
    }

    private IEnumerator HoldToBuild()
    {
        float elapsedTime = 0f;
        while (elapsedTime < objToBuild.buildTime)
        {
            elapsedTime += Time.deltaTime;
            buildIndicator.fillAmount = elapsedTime / objToBuild.buildTime;

            yield return null;
        }

        foreach (var item in _resourcesToUse)
        {
            Destroy(item.gameObject);
        }
        
        objToBuild.Build();

        Destroy(gameObject, 0.3f);
    }
}
