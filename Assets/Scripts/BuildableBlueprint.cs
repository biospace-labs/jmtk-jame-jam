using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class BuildableBlueprint : MonoBehaviour
{
    
    [Serializable]
    private class BlueprintStaticProperties
    {
        public float buildTime = 1f;
        public ResourceType[] requiredResources;
        public UnityEvent onBuildComplete;
    }

    [SerializeField]
    private BlueprintStaticProperties blueprintStaticProperties;

    public float buildTime => blueprintStaticProperties.buildTime;
    public ResourceType[] requiredResources => blueprintStaticProperties.requiredResources;
    public UnityEvent onBuildComplete => blueprintStaticProperties.onBuildComplete;

    [Header("Per Instance Properties")]
    public bool startBuilt = false;
    public BuildableBlueprint[] prerequisites;

    public void Build()
    {
        gameObject.SetActive(true);

        //TODO animate building

        onBuildComplete.Invoke();
    }
}
