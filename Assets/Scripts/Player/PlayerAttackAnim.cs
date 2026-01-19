using System.Collections;
using Assets.Utility;
using UnityEngine;

public class PlayerAnimAttack : MonoBehaviour
{
    [SerializeField] private Transform sword;

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
    [SerializeField] private Vector3 swordScaleTo;
    [SerializeField] private float scaleDuration;

    private Coroutine attackAnimationCo;

    void Start()
    {
        rotateDir = -1;
        startYAngle = swingPos_R.localEulerAngles.y;
        endYAngle = swingPos_L.localEulerAngles.y;
        targetYAngle = Mathf.LerpAngle(startYAngle, endYAngle, 1f);
    }

    public void PlayAttackAnimation()
    {
        if (attackAnimationCo != null) StopCoroutine(attackAnimationCo);
        attackAnimationCo = StartCoroutine(AttackAnimationRoutine());
    }

    private IEnumerator AttackAnimationRoutine()
    {
        var swingCo = StartCoroutine(HorizontalSwingCo(swingDuration));
        // StartCoroutine(LungeForwardRoutine(lungeDuration));
        // StartCoroutine(ScaleWeaponRoutine(scaleDuration));
        yield return swingCo;
    }

    private IEnumerator HorizontalSwingCo(float duration)
    {
        float startDuration = duration * 0.7f; // main swing uses ~70% of total time
        float returnDuration = duration - startDuration; // remaining time for return

        float fromYAngle = sword.localEulerAngles.y;
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
                sword.localRotation = newRotation;
            });

        // return from overshoot to end angle
        yield return Utils.LerpOverTime(startDuration, offYAngle, toYAngle,
            newAngle =>
            {
                Vector3 newEulerAngles = new(0f, newAngle, 0f);
                Quaternion newRotation = Quaternion.Euler(newEulerAngles);
                sword.localRotation = newRotation;
            });

        // reverse swing direction
        rotateDir *= -1;
        // swap angle value using tuple
        (endYAngle, startYAngle) = (startYAngle, endYAngle);
        // calculate new target angle
        targetYAngle = Mathf.LerpAngle(startYAngle, endYAngle, 1f);
    }

    // private IEnumerator LungeForwardRoutine(float duration)
    // {
    //     Vector3 startPos = transform.position;
    //     Vector3 direction = transform.forward * lungeDistance;
    //     Vector3 endPos = startPos + direction;

    //     yield return Utils.LerpOverTime(duration, startPos, endPos, 
    //         targetPos => transform.position = targetPos);
    // }

    // private IEnumerator ScaleWeaponRoutine(float duration)
    // {
    //     float startDuration = duration * 0.7f;
    //     float returnDuration = duration - startDuration;

    //     Vector3 fromScale = sword.localScale;
    //     Vector3 toScale = swordScaleTo;

    //     yield return Utils.LerpOverTime(startDuration, fromScale, toScale, 
    //         targetScale => sword.localScale = targetScale);

    //     // swap scale target
    //     toScale = Utils.SwapVector3(ref fromScale, ref swordScaleTo);

    //     yield return Utils.LerpOverTime(returnDuration, fromScale, toScale, 
    //         targetScale => sword.localScale = targetScale);
    // }

}
