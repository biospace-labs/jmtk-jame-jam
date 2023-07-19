using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class TextboxManager : MonoBehaviour
{
    public GameObject textBox;
    public TMP_Text text;
    public TextAsset introText;
    public TextAsset levelEndText;
    public TextAsset randomText;

    public boi boi;

    public float randomMinTime = 20;
    public float randomMaxTime = 60;

    private float randomTime;
    private float time = 0;

    private bool isActive = false;
    private LevelBlueprintManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindObjectOfType<LevelBlueprintManager>();
        levelManager.onLevelEnd.AddListener(OnLevelEnd);
        
        boi = FindObjectOfType<boi>();

        randomTime = Random.Range(randomMinTime,randomMaxTime);

        if (introText != null) {
            var lines = introText.text.Split('\n');
            PlayLines(lines, () => levelManager.StartLevel());  
        }
        else
        {
            levelManager.StartLevel();
        }
    }

    private void OnLevelEnd()
    {
        if (levelEndText != null) {
            var lines = levelEndText.text.Split('\n');
            PlayLines(lines, () => levelManager.LoadNextScene());  
        }
        else
        {
            levelManager.LoadNextScene();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive && randomText != null)
        {
            time += Time.deltaTime;
            if (time >= randomTime) {
                time -= randomTime;
                randomTime = Random.Range(randomMinTime, randomMaxTime);

                var lines = randomText.text.Split('\n');
                PlayLines(lines);
            }
        }
    }

    public void PlayLines(IEnumerable<string> lines, Action then = null)
    {
        StartCoroutine(PlayLinesCoroutine(lines, then));
    }

    private IEnumerator PlayLinesCoroutine(IEnumerable<string> lines, Action then)
    {
        EnableTextBox();
        foreach (string line in lines)
        {
            text.text = line;
            yield return null;
            yield return new WaitUntil(() => Input.GetButtonDown("Grab") || Input.GetButtonDown("Use"));
        }
        DisableTextBox();
        then();
    }

    private void EnableTextBox() {
        textBox.SetActive(true);
        boi._inputsEnabled = false;
        isActive = true;
    }

    private void DisableTextBox() {
        textBox.SetActive(false);
        boi._inputsEnabled = true;
        isActive = false;
    }
}
