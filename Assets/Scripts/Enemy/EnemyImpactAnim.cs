using System.Collections;
using UnityEngine;

public class EnemyImpactAnim : MonoBehaviour
{
    [SerializeField] private float stunDuration;
    [SerializeField] private float pushDuration;
    [SerializeField] private float pushDistance;
    [SerializeField] private float leanDuration;
    [SerializeField] private Vector3 leanAngle;

    private float currentAngleX;
    private float targetAngleX;
    private Coroutine impactRoutine;

    void Start()
    {
        currentAngleX = transform.localRotation.x;
        targetAngleX = currentAngleX + leanAngle.x;
    }

    public void PlayImpactAnim()
    {
        if (impactRoutine != null) StopCoroutine(impactRoutine);
        impactRoutine = StartCoroutine(ImpactRoutine());
    }

    private IEnumerator ImpactRoutine()
    {
        StartCoroutine(LeanBackwardRoutine(leanDuration));
        yield return StartCoroutine(PushBackRoutine(pushDuration));
    }

    private IEnumerator PushBackRoutine(float duration)
    {
        float elapsed = 0f;
        
        Vector3 direction = gameObject.GetComponent<EnemyFollow>().GetMovingDirection();
        Vector3 oppositeDir = -1 * direction;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + oppositeDir * pushDistance;

        // interpolate between current position and target position
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Vector3 nextPos = Vector3.Lerp(startPos, endPos, t);
            transform.position = nextPos;
            yield return null;
        }

        transform.position = endPos;
    }

    private IEnumerator LeanBackwardRoutine(float duration)
    {
        float elapsed = 0f;
        float halfDuration = duration * 0.5f;
        float returnDuration = duration - halfDuration;
        float startAngleX = currentAngleX;
        float endAngleX = targetAngleX;

        // phase 1: pivot backward
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);
            float newAngleX = Mathf.LerpAngle(startAngleX, endAngleX, t);
            transform.rotation = Quaternion.Euler(newAngleX, transform.eulerAngles.y, 0);
            yield return null;
        }

        // phase2: return to normal rotation
        elapsed = 0;
        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / returnDuration);
            float newAngleX = Mathf.LerpAngle(endAngleX, startAngleX, t);
            transform.rotation = Quaternion.Euler(newAngleX, transform.eulerAngles.y, 0);
            yield return null;
        }
    }
}
