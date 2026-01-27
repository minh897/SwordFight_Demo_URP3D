using System.Collections;
using Assets.Utility;
using UnityEngine;

public class EnemyImpactAnim : MonoBehaviour
{
    [SerializeField] private float stunDuration;
    [SerializeField] private float pushDuration;
    [SerializeField] private float pushDistance;
    [SerializeField] private float leanDuration;
    [SerializeField] private Vector3 leanAngle;
    private float startXAngle;
    private float targetXAngle;

    private Coroutine impactRoutine;

    void Start()
    {
        startXAngle = transform.eulerAngles.x;
        targetXAngle = startXAngle + leanAngle.x;
    }

    public void PlayImpactAnim()
    {
        if (impactRoutine != null) StopCoroutine(impactRoutine);
        impactRoutine = StartCoroutine(ImpactRoutine());
    }

    private IEnumerator ImpactRoutine()
    {
        StartCoroutine(PivotBackwardCo(leanDuration));
        yield return StartCoroutine(PushBackCo(pushDuration));
    }

    private IEnumerator PushBackCo(float duration)
    {
        Vector3 startPos = transform.position;
        Vector3 backward = -1 * transform.forward;

        float fromDist = 0f;
        float toDist = pushDistance;

        yield return Utils.LerpOverTime(duration, fromDist, toDist, 
            currentDist =>
            {
                float eased = currentDist * currentDist;
                Vector3 newPos = startPos + currentDist * pushDistance * backward;
                transform.position = newPos;
            });
    }

    private IEnumerator PivotBackwardCo(float duration)
    {
        float startDuration = duration * 0.7f;
        float returnDuration = duration - startDuration;

        float startAngle = startXAngle;
        float endAngle = targetXAngle;

        // Pivot transform backward
        yield return Utils.LerpOverTime(startDuration, startAngle, endAngle, 
            currentAngle =>
            {
                Vector3 newEulerAngles = new(currentAngle, transform.eulerAngles.y, transform.eulerAngles.z);
                Quaternion newRotation = Quaternion.Euler(newEulerAngles);
                transform.rotation = newRotation;
            });

        // Return to normal rotation
        yield return Utils.LerpOverTime(returnDuration, endAngle, startAngle, 
            currentAngle =>
            {
                Vector3 newEulerAngles = new(currentAngle, transform.eulerAngles.y, transform.eulerAngles.z);
                Quaternion newRotation = Quaternion.Euler(newEulerAngles);
                transform.rotation = newRotation;
            });
    }
}
