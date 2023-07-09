using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class saw : MonoBehaviour
{
    public float sawTraverseTime = 1.0f;
    public float sawDelayTime = 0.3f;
    public float trackLength = 3f;

    private SpriteRenderer _trackRenderer;
    private SpriteRenderer _sawRenderer;
    // Start is called before the first frame update
    void Start()
    {
        StartMoving();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartMoving()
    {
        _trackRenderer = transform.GetChild(0).transform.GetChild(1).GetComponent<SpriteRenderer>();
        _sawRenderer = transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>();
        _trackRenderer.size = new Vector2(1.0f / 3, trackLength);
        _trackRenderer.gameObject.transform.position = transform.position + transform.up * (trackLength / 2.0f + 1);
        StartCoroutine(Traverse(transform.position + transform.up,
            transform.position + transform.up * (trackLength + 1)));
    }

    IEnumerator Traverse(Vector2 start, Vector2 end)
    {
        Debug.Log("traverse " + start + " " + end);  
        float timeElapsed = 0f;
        while (timeElapsed < sawTraverseTime)
        {
            timeElapsed += Time.deltaTime;
            _sawRenderer.gameObject.transform.position = Vector2.Lerp(start, end, timeElapsed / sawTraverseTime);
            yield return null;
        }
        yield return new WaitForSeconds(sawDelayTime);
        StartCoroutine(Traverse(end, start));
    }
}
