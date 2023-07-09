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
    public ResourceBubble bubble;
    public GameObject canBuildIndicator;

    private List<ResourceItem> availableResources;

    public void Start()
    {
        availableResources = new List<ResourceItem>();
        UpdateIndicators();

        buildIndicator.fillAmount = 0;
    }

    public void Update()
    {
        if (!objToBuild)
        {
            Debug.LogWarning("BuildSite: objToBuild is null, destroying self!");
            Destroy(gameObject);
        }
        else if (objToBuild.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("BuildSite: objToBuild already active, destroying self!");
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log($"BuildPoint TriggerEnter {other.gameObject}");
        ResourceItem item = other.gameObject.GetComponent<ResourceItem>();
        if (item)
        {
            availableResources.Add(item);
            UpdateIndicators();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //Debug.Log($"BuildPoint TriggerExit {other.gameObject}");
        ResourceItem item = other.gameObject.GetComponent<ResourceItem>();
        if (item)
        {
            availableResources.Remove(item);
            UpdateIndicators();
        }
    }
    private void UpdateIndicators()
    {
        var missing = new List<ResourceType>(objToBuild.requiredResources);

        foreach (var item in availableResources)
        {
            missing.Remove(item.resourceType);
        }

        bubble.OnResourcesChanged(missing);
        canBuildIndicator.SetActive(missing.Count == 0);
    }

    public override void BeginUse()
    {
        StartCoroutine("HoldToBuild");
    }

    public override void EndUse()
    {
        StopCoroutine("HoldToBuild");
        buildIndicator.fillAmount = 0f;
    }

    private IEnumerator HoldToBuild()
    {
        // check if we have enough resources
        var missing = new List<ResourceType>(objToBuild.requiredResources);
        var resourcesToUse = new List<ResourceItem>();

        foreach (var item in availableResources)
        {
            if (missing.Remove(item.resourceType))
            {
                resourcesToUse.Add(item);
            }
        }

        if (missing.Count > 0) yield break;
        
        float elapsedTime = 0f;
        while (elapsedTime < objToBuild.buildTime)
        {
            elapsedTime += Time.deltaTime;
            buildIndicator.fillAmount = elapsedTime / objToBuild.buildTime;
            
            // check resources still in range??

            yield return null;
        }

        foreach (var item in resourcesToUse)
        {
            Destroy(item.gameObject);
        }
        
        objToBuild.Build();

        Destroy(gameObject, 0.3f);
    }
}
