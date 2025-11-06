using System.Collections;
using UnityEngine;

public class Example : MonoBehaviour
{
    float currStrength = 0f;
    float maxStrength = 20f;
    float duration = 100f;
    float elapsed = 0f;
    float maxDelta = 0f;
    float recoveryRate = 5f;

    void Update()
    {
        // StartCoroutine(ExampleCoroutine());
        maxDelta = Time.deltaTime;
        currStrength = Mathf.MoveTowards(currStrength, maxStrength, maxDelta);
    }

    private IEnumerator ExampleCoroutine()
    {
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            maxDelta = elapsed / duration;
            currStrength = Mathf.MoveTowards(currStrength, maxStrength, maxDelta);
            yield return null;
        }
        Debug.Log("Current strength: " + currStrength);
    }
}
