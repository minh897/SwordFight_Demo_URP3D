using System.Collections;
using UnityEngine;

public class PlayerAnimAttack : MonoBehaviour
{
    [SerializeField] private Transform sword;
    [SerializeField] private float swingDuration = 0.3f;
    [SerializeField] private float overshootYAngle = -30f;
    [SerializeField] private Vector3 swingAngle;
    [SerializeField] private float lungeDuration = 0.2f;
    [SerializeField] private float lungeDistance = 1f;
    [SerializeField] private Vector3 stretchScale;

    // animation settings
    private float currentYAngle;
    private float targetYAngle;
    private float currentZScale;
    private float targetZScale;

    void Start()
    {
        currentYAngle = sword.localEulerAngles.y;
        targetYAngle = currentYAngle + swingAngle.z;
        currentZScale = sword.localScale.z;
        targetZScale = currentZScale + stretchScale.z;
    }

    public IEnumerator AttackAnimationRoutine()
    {
        var swingCo = StartCoroutine(WeaponSwingRoutine(swingDuration));
        StartCoroutine(LungeForwardRoutine(lungeDuration));
        StartCoroutine(ScaleWeaponRoutine(swingDuration));
        yield return swingCo;
    }

    private IEnumerator WeaponSwingRoutine(float duration)
    {
        float halfDuration = duration * 0.7f; // main swing uses ~70% of total time
        float returnDuration = duration - halfDuration; // remaining time for return
        float startYAngle = currentYAngle;
        float endYAngle = targetYAngle;

        // since both angles are swapped at the end 
        // start and end angle need to be interpolated
        // startYAngle + endYAngle would yield a different result
        float offsetYAngle = Mathf.Lerp(startYAngle, endYAngle, 1f) + overshootYAngle;

        // --- Phase 1: Swing with overshoot ---
        float elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            // converts time into a normalized progress value
            // and then clamp that progress between 0 and 1 
            // (prevent it to hit 0.2 or 1.1 something like that)
            float t = Mathf.Clamp01(elapsed / halfDuration);
            float yRot = Mathf.Lerp(startYAngle, offsetYAngle, t);
            sword.localRotation = Quaternion.Euler(0, yRot, 0);
            yield return null;
        }

        // --- Phase 2: Return from overshoot to end angle ---
        elapsed = 0f;
        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            // Ease back from overshoot to final resting angle
            float t = Mathf.Clamp01(elapsed / returnDuration);
            float yRot = Mathf.Lerp(offsetYAngle, endYAngle, t);
            sword.localRotation = Quaternion.Euler(0, yRot, 0);
            yield return null;
        }

        // make sure the rotation hit the target
        sword.localRotation = Quaternion.Euler(0, endYAngle, 0);
        // swap angle for next swing
        currentYAngle = endYAngle;
        targetYAngle = startYAngle;
        // change overshoot angle after each swing
        overshootYAngle *= -1;
    }

    private IEnumerator LungeForwardRoutine(float duration)
    {
        Vector3 startPos = transform.position;
        Vector3 direction = transform.forward * lungeDistance;
        Vector3 endPos = startPos + direction;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Vector3 towardPosition = Vector3.Lerp(startPos, endPos, t);
            transform.position = towardPosition;
            yield return null;
        }

        transform.position = endPos;
    }

    private IEnumerator ScaleWeaponRoutine(float duration)
    {
        float halfDuration = duration * 0.7f;
        float returnDuration = duration - halfDuration;

        float startZScale = currentZScale;
        float endZScale = targetZScale;
        Vector3 targetScale = new(1, 1, targetZScale);

        float elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);
            float zScale = Mathf.Lerp(startZScale, endZScale, t);
            sword.localScale = new(1, 1, zScale);
            yield return null;
        }
        sword.localScale = targetScale;

        float newStartZScale = endZScale;
        float newTargetZScale = startZScale;
        
        elapsed = 0f;
        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / returnDuration);
            float zScale = Mathf.Lerp(newStartZScale, newTargetZScale, t);
            sword.localScale = new(1, 1, zScale);
            yield return null;
        }
    }
}
