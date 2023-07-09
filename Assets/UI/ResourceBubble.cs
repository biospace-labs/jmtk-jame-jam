using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBubble : MonoBehaviour
{
    public GameObject material1LayoutGroup;
    public GameObject material2LayoutGroup;
    
    private GameObject material1Image;
    private GameObject material2Image;
    private List<GameObject> material1ImagePool;
    private List<GameObject> material2ImagePool;

    public void Start()
    {
        material1LayoutGroup.SetActive(false);
        material1Image = material1LayoutGroup.transform.GetChild(0).gameObject;
        material2LayoutGroup.SetActive(false);
        material2Image = material2LayoutGroup.transform.GetChild(0).gameObject;
    }

    public void OnResourcesChanged(List<ResourceType> missingResources)
    {
        return;
        missingResources.Sort();
        foreach (var r in missingResources)
        {
            switch (r)
            {
                case ResourceType.Material1:
                    // material1ImagePool
                case ResourceType.Material2:
                    // material2ImagePool
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
