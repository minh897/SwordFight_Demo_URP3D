using System.Collections;
using Assets.Utility;
using UnityEngine;

public class PlayerAnimAttack : MonoBehaviour
{
    [SerializeField] private Transform sword;
    [SerializeField] private float swingDuration = 0.3f;

    [Header("Swing angles")]
    [SerializeField] private Vector3 swordTargetAngle;
    [SerializeField] private float overshootYAngle = -30f;
    [SerializeField] private Transform swingRot_1;
    [SerializeField] private Transform swingRot_2;

    [Header("Lunge forward")]
    [SerializeField] private float lungeDuration = 0.2f;
    [SerializeField] private float lungeDistance = 1f;

    [Header("Sword scale")]
    [SerializeField] private Vector3 swordScaleTo;

    // animation settings
    // private float currentYAngle;
    // private float targetYAngle;

    private Coroutine attackAnimationCo;

    void Start()
    {
        // currentYAngle = sword.localEulerAngles.y;
        // targetYAngle = currentYAngle + swordTargetAngle.y;
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
        // StartCoroutine(ScaleWeaponRoutine(swingDuration));
        yield return swingCo;
    }

    private IEnumerator HorizontalSwingCo(float duration)
    {
        float startDuration = duration * 0.7f; // main swing uses ~70% of total time
        float returnDuration = duration - startDuration; // remaining time for return

        float startYAngle = sword.localEulerAngles.y;
        float endYAngle = swordTargetAngle.y;

        // since both angles are swapped at the end 
        // start and end angle need to be interpolated
        // startYAngle + endYAngle would yield a different result
        float offYAngle = Mathf.Lerp(startYAngle, endYAngle, 1f) + overshootYAngle;

        // swing with overshoot
        yield return Utils.LerpOverTime(startDuration, startYAngle, offYAngle,
            targetYAngle =>
            {
                Vector3 newEulerAngles = new(0f, targetYAngle, 0f);
                Quaternion newRotation = Quaternion.Euler(newEulerAngles);
                sword.rotation = newRotation;
            });

        // return from overshoot to end angle
        yield return Utils.LerpOverTime(returnDuration, offYAngle, endYAngle,
            targetYAngle =>
            {
                Vector3 newEulerAngles = new(0f, targetYAngle, 0f);
                Quaternion newRotation = Quaternion.Euler(newEulerAngles);
                sword.rotation = newRotation;
            });

        // swap target angle
        // reverse overshoot y angle
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
