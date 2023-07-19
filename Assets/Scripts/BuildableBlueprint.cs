using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BuildableBlueprint : MonoBehaviour
{
    
    [Serializable]
    private class BlueprintStaticProperties
    {
        public float buildTime = 1f;
        public ResourceType[] requiredResources;
        public UnityEvent onBuildComplete;
    }
    
    private GameObject _tiles;
    private GameObject _blueprint;
    private ParticleSystem _buildParticles;
    private Vector2 _animationInitialPosition;
    private Vector2 _animationFinalPosition;
    public float _animationTime = 1f;
    public float trembleIntensity = 0.1f;
    public float _spriteHeight = 1;
    public AudioSource _audioSource;

    [SerializeField]
    private BlueprintStaticProperties blueprintStaticProperties;

    public float buildTime => blueprintStaticProperties.buildTime;
    public ResourceType[] requiredResources => blueprintStaticProperties.requiredResources;
    public UnityEvent onBuildComplete => blueprintStaticProperties.onBuildComplete;

    [Header("Per Instance Properties")]
    public bool startBuilt = false;
    public BuildableBlueprint[] prerequisites;

    public void ShowBlueprint()
    {
        _blueprint = transform.GetChild(1).gameObject;
        _blueprint.transform.SetParent(null);
        _blueprint.transform.position -= Vector3.back;
        _blueprint.SetActive(true);
    }

    public void Build()
    {
        gameObject.SetActive(true);

        _tiles = transform.GetChild(0).gameObject;
        _buildParticles = GetComponentInChildren<ParticleSystem>(true);
        _animationFinalPosition = _tiles.transform.position;
        _animationInitialPosition = _tiles.transform.position - gameObject.transform.up * _spriteHeight;

        _tiles.transform.position = _animationInitialPosition;

        _buildParticles.Play();
        _audioSource.Play();

        StartCoroutine(constructBuilding());
        //TODO animate building
    }

    IEnumerator constructBuilding()
    {
        float timeElapsed = 0f;
        while (timeElapsed < _animationTime)
        {
            timeElapsed += Time.deltaTime;
            _tiles.transform.position = Vector3.Lerp(_animationInitialPosition, _animationFinalPosition, timeElapsed / _animationTime);
            _tiles.transform.position += (Vector3) ((Vector2) UnityEngine.Random.insideUnitSphere * trembleIntensity);
            yield return null;
        }
        _blueprint.SetActive(false);
        onBuildComplete.Invoke();
    }
}
