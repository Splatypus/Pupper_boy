using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleSizeChange : MonoBehaviour
{

    public void SetSize(Vector3 size) {
        transform.localScale = size;
    }

    //expands the hole from an optional start size to an end size over a given duration. Optional callback when completed
    public void Expand(Vector3 endSize, float duration, System.Action onComplete = null) {
        Expand(transform.localScale, endSize, duration, onComplete);
    }
    public void Expand(Vector3 startSize, Vector3 endSize, float duration, System.Action onComplete = null) {
        StartCoroutine(GrowHole(startSize, endSize, duration, onComplete));
    }
    public void Decay(float duration, bool destroyAtEnd = true) {
    }

    //Coroutine to grow the hole
    IEnumerator GrowHole(Vector3 startSize, Vector3 endSize, float duration, System.Action onComplete) {
        //initial setting
        transform.localScale = startSize;
        float startTime = Time.time;

        //expand over time
        while (startTime + duration > Time.time) {
            transform.localScale = Vector3.Lerp(startSize, endSize, (Time.time - startTime) / duration);
            yield return new WaitForEndOfFrame();
        }

        //post settings
        transform.localScale = endSize;
        onComplete?.Invoke();
    }
}
