using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class TextboxManager : MonoBehaviour
{
    public GameObject textBox;
    public TMP_Text text;
    public TextAsset introText;
    public TextAsset levelEndText;
    public TextAsset randomText;
    public string[] textLines;
    public int currentLine;

    public int endAtLine;
    public boi boi;

    public float randomMinTime = 20;
    public float randomMaxTime = 60;

    private float randomTime;
    private float time = 0;

    private bool isActive = false;
    private LevelBlueprintManager levelManager;

    enum Dialogue
    {
        Intro,
        LevelEnd,
    }

    private Dialogue currentDialogue;

    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindObjectOfType<LevelBlueprintManager>();
        levelManager.onLevelEnd.AddListener(LevelEnd);
        
        boi = FindObjectOfType<boi>();

        randomTime = Random.Range(randomMinTime,randomMaxTime);
        if (introText != null) {
            textLines = (introText.text.Split('\n'));

            if (endAtLine == 0) {
                endAtLine = textLines.Length - 1;
            }

            currentLine = 0;
            currentDialogue = Dialogue.Intro;
            EnableTextBox();
        }
        else
        {
            levelManager.StartLevel();
        }
    }

    void LevelEnd()
    {
        if (levelEndText != null) {
            textLines = (levelEndText.text.Split('\n'));
            endAtLine = textLines.Length - 1;
            
            currentLine = 0;
            currentDialogue = Dialogue.LevelEnd;
            EnableTextBox();
        }
        else
        {
            levelManager.LoadNextScene();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive) {
            text.text = textLines[currentLine];
            if (Input.GetKeyDown(KeyCode.Space)) {
                currentLine += 1;
            }
            if (currentLine > endAtLine)
            {
                DisableTextBox();
                if (currentDialogue == Dialogue.Intro)
                {
                    levelManager.StartLevel();
                }
                else if (currentDialogue == Dialogue.LevelEnd)
                {
                    levelManager.LoadNextScene();
                }
            }
        } else {
            time += Time.deltaTime;
            if (time >= randomTime) {
                time -= randomTime;
                randomTime = Random.Range(randomMinTime, randomMaxTime);
                if (randomText != null) {
                    textLines = (randomText.text.Split('\n'));
                }
                endAtLine = textLines.Length - 1;
                currentLine = 0;
                EnableTextBox();
            }
        }
    }

    public void EnableTextBox() {
        textBox.SetActive(true);
        boi._inputsEnabled = false;
        isActive = true;
    }

    public void DisableTextBox() {
        textBox.SetActive(false);
        boi._inputsEnabled = true;
        isActive = false;
    }
}
