using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        material1ImagePool = new List<GameObject>();
        material1ImagePool.Add(material1Image);

        material2LayoutGroup.SetActive(false);
        material2Image = material2LayoutGroup.transform.GetChild(0).gameObject;
        material2ImagePool = new List<GameObject>();
        material2ImagePool.Add(material2Image);
    }

    public void OnResourcesChanged(List<ResourceType> missingResources)
    {

        if (missingResources.Count == 0)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        
        var numMaterial1 = 0;
        var numMaterial2 = 0;
        foreach (var r in missingResources)
        {
            switch (r)
            {
                case ResourceType.Material1:
                    numMaterial1++; break;
                case ResourceType.Material2:
                    numMaterial2++; break;
            }
        }

        material1LayoutGroup.SetActive(numMaterial1 > 0);
        if (numMaterial1 > 0)
        {
            for (int i = 0; i < material1ImagePool.Count; i++)
            {
                material1ImagePool[i].SetActive(i < numMaterial1);
            }
            for (int i = material1ImagePool.Count; i < numMaterial1; i++)
            {
                material1ImagePool.Add(Instantiate(material1Image, material1LayoutGroup.transform));
            }
        }
        
        material2LayoutGroup.SetActive(numMaterial2 > 0);
        if (numMaterial2 > 0)
        {
            for (int i = 0; i < material2ImagePool.Count; i++)
            {
                material2ImagePool[i].SetActive(i < numMaterial2);
            }
            for (int i = material2ImagePool.Count; i < numMaterial2; i++)
            {
                material2ImagePool.Add(Instantiate(material2Image, material2LayoutGroup.transform));
            }
        }
    }
}
