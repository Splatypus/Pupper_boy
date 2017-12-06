using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFadeOut : MonoBehaviour
{

    Text text;
    [SerializeField] private float timeUntilFadeStarts = 1.0f;
    [SerializeField] private float timeToFadeOut = 1.0f;
    [SerializeField] private float timeToFadeIn = 0.25f;

    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();
        Color c = text.color;
        c.a = 0;
        text.color = c;
    }

    public void setText(string words)
    {
        text.text = words;
        Color c = text.color;
        c.a = 1.0f;
        text.color = c;
        print("setting text to " + words);
        StartCoroutine(fadeLerp(0, 1, timeToFadeIn));
        Invoke("startFade", timeUntilFadeStarts);
    }

    private void startFade()
    {
        StartCoroutine(fadeLerp(1, 0, timeToFadeOut));
    }

    private IEnumerator fadeLerp(float start, float end, float lerp_time)
    {
        float startTime = Time.time;
        float lerpAmount = 0.0f;
        while (lerpAmount < 1.0f)
        {
            lerpAmount = (Time.time - startTime) / lerp_time;
            print("lerping in t = " + lerpAmount);
            float alpha = Mathf.Lerp(start, end, lerpAmount);
            Color c = text.color;
            c.a = alpha;
            text.color = c;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
