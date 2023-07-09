using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildableBlueprint : MonoBehaviour
{
    public ResourceType[] requiredResources;
    public float buildTime = 1f;

    [Header("Per Instance Options")]
    public bool startBuilt = false;
    public BuildableBlueprint[] prerequisites;

    public void Build()
    {
        gameObject.SetActive(true);

        //TODO animate building
    }
}
