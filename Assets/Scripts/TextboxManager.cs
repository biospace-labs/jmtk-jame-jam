using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextboxManager : MonoBehaviour
{
    public GameObject textBox;
    public TMP_Text text;
    public TextAsset introText;
    public TextAsset randomText;
    public string[] textLines;
    public int currentLine;

    public int endAtLine;
    public boi boi;

    public float randomTime = 60;
    private float time = 0;

    private bool isActive = false;
    
    // Start is called before the first frame update
    void Start()
    {
        boi = FindObjectOfType<boi>();

        if (introText != null) {
            textLines = (introText.text.Split('\n'));
        }

        if (endAtLine == 0) {
            endAtLine = textLines.Length - 1;
        }
        EnableTextBox();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive) {
            text.text = textLines[currentLine];
            if (Input.GetKeyDown(KeyCode.Space)) {
                currentLine += 1;
            }
            if (currentLine > endAtLine) {
                DisableTextBox();
            }
        } else {
            time += Time.deltaTime;
            if (Random.Range(0.0f, (randomTime - time) * 60) < 1.0f) {
                time = 0;
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
