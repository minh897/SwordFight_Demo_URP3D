using System.Collections;
using Assets.Utility;
using UnityEngine;

public class PlayerAttackAnim : MonoBehaviour
{
    public event System.Action OnAttackStarted;
    public event System.Action OnAttackFinished;

    [Header("Weapon")]
    [SerializeField] private Transform weaponTransform;

    [Header("Sword swing")]
    [SerializeField] private Transform swingPos_R;
    [SerializeField] private Transform swingPos_L;
    [SerializeField] private float swingDuration;
    [SerializeField] private float overshootYAngle;
    private float startYAngle;
    private float endYAngle;
    private float targetYAngle;
    private float rotateDir;

    [Header("Lunge forward")]
    [SerializeField] private float lungeDuration;
    [SerializeField] private float lungeDistance;

    [Header("Sword scale")]
    [SerializeField] private Vector3 targetScale;
    [SerializeField] private float scaleDuration;

    private Coroutine attackAnimationCo;

    void Start()
    {
        rotateDir = -1;
        startYAngle = swingPos_R.localEulerAngles.y;
        endYAngle = swingPos_L.localEulerAngles.y;
        targetYAngle = Mathf.LerpAngle(startYAngle, endYAngle, 1f);
    }

    public void PlayAttackAnim()
    {
        if (attackAnimationCo != null) StopCoroutine(attackAnimationCo);
        attackAnimationCo = StartCoroutine(AttackCo());
    }

    private IEnumerator AttackCo()
    {
        OnAttackStarted?.Invoke();

        var swingCo = StartCoroutine(HorizontalSwingCo(swingDuration));
        StartCoroutine(LungeForwardCo(lungeDuration));
        StartCoroutine(ScaleWeaponCo(scaleDuration));
        yield return swingCo;

        OnAttackFinished?.Invoke();
    }

    private IEnumerator HorizontalSwingCo(float duration)
    {
        float startDuration = duration * 0.7f; // main swing uses ~70% of total time
        float returnDuration = duration - startDuration; // remaining time for return

        float fromYAngle = weaponTransform.localEulerAngles.y;
        float toYAngle = targetYAngle * rotateDir;

        // since both angles are swapped at the end 
        // the offYAngle value need to be interpolated
        // fromYAngle + toYAngle would yield a different result
        float offYAngle = Mathf.Lerp(fromYAngle, toYAngle, 1f) + overshootYAngle * rotateDir;

        // swing with overshoot
        yield return Utils.LerpOverTime(startDuration, fromYAngle, offYAngle,
            newAngle =>
            {
                Vector3 newEulerAngles = new(0f, newAngle, 0f);
                Quaternion newRotation = Quaternion.Euler(newEulerAngles);
                weaponTransform.localRotation = newRotation;
            });

        // return from overshoot to end angle
        yield return Utils.LerpOverTime(startDuration, offYAngle, toYAngle,
            newAngle =>
            {
                Vector3 newEulerAngles = new(0f, newAngle, 0f);
                Quaternion newRotation = Quaternion.Euler(newEulerAngles);
                weaponTransform.localRotation = newRotation;
            });

        // reverse swing direction
        rotateDir *= -1;
        // swap angle value using tuple
        (endYAngle, startYAngle) = (startYAngle, endYAngle);
        // calculate new target angle
        targetYAngle = Mathf.LerpAngle(startYAngle, endYAngle, 1f);
    }

    private IEnumerator LungeForwardCo(float duration)
    {
        Vector3 startPos = transform.position;
        Vector3 forward = transform.forward;

        float fromDist = 0f;
        float toDist = lungeDistance;

        yield return Utils.LerpOverTime(duration, fromDist, toDist, 
            currentDist =>
            {
                float eased = currentDist * currentDist; // ease-in the lunge distance
                Vector3 newPos = startPos + eased * lungeDistance * forward;
                transform.position = newPos;
            });

    }

    private IEnumerator ScaleWeaponCo(float duration)
    {
        float startDuration = duration * 0.7f;
        float returnDuration = duration - startDuration;

        float fromScale = weaponTransform.localScale.z;
        float toScale = targetScale.z;

        yield return Utils.LerpOverTime(startDuration, fromScale, toScale, 
            newValue =>
            {
                Vector3 newScale = new(
                    weaponTransform.localScale.x, 
                    weaponTransform.localScale.y, 
                    newValue);
                weaponTransform.localScale = newScale;
            });

        // swap scale target
        (fromScale, toScale) = (toScale, fromScale);

        yield return Utils.LerpOverTime(returnDuration, fromScale, toScale, 
            newValue =>
            {
                Vector3 newScale = new(
                    weaponTransform.localScale.x, 
                    weaponTransform.localScale.y, 
                    newValue);
                weaponTransform.localScale = newScale;
            });
    }

}
