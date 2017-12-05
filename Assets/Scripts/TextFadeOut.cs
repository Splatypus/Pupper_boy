using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFadeOut : MonoBehaviour
{

    Text text;
    [SerializeField] private float timeUntilFadeStarts = 1.0f;
    [SerializeField] private float timeToFade = 1.0f;

    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();
        print("starting color: " + text.color);
        Color c = text.color;
        c.a = 0;
        text.color = c;
        print("after color: " + text.color);
    }

    public void setText(string words)
    {
        print("setting text to " + words);
        text.text = words;
        Color c = text.color;
        c.a = 1.0f;
        text.color = c;
        //StartCoroutine(fadeText());
        Invoke("startFade", timeUntilFadeStarts);
    }

    private void startFade()
    {
        StartCoroutine(fadeText());
    }

    private IEnumerator fadeText()
    {
        float startTime = Time.time;
        while (text.color.a > 0)
        {
            float lerpAmount = 1f - ((Time.time - startTime) / timeToFade);
            float alpha = 1.0f * lerpAmount;
            Color c = text.color;
            c.a = alpha;
            text.color = c;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            setText("Hello World!");
        }
    }
}
