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
    
    private float spriteHeight;
    private GameObject tiles;
    private ParticleSystem _buildParticles;
    private float yPosInitial;
    public float moveSpeed;

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
        tiles = gameObject.transform.GetChild(0).gameObject;
        spriteHeight = tiles.GetComponentInChildren<Renderer>().bounds.size.y;

        yPosInitial = tiles.transform.position.y;
        Debug.Log(yPosInitial);

        tiles.transform.position = new Vector2(tiles.transform.position.x,
            yPosInitial - spriteHeight);

        gameObject.SetActive(true);

        _buildParticles = GetComponentInChildren<ParticleSystem>();
        _buildParticles.Play();

        StartCoroutine(constructBuilding());
        //TODO animate building
    }

    IEnumerator constructBuilding()
    {
        while (tiles.transform.position.y < yPosInitial)
        {
            tiles.transform.position += Vector3.up * moveSpeed * Time.deltaTime;
            Debug.Log(tiles.transform.position.y);
            yield return null;
        }

        onBuildComplete.Invoke();
    }
}
