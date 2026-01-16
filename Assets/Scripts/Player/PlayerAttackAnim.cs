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

    [Header("Lunge forward")]
    [SerializeField] private float lungeDuration = 0.2f;
    [SerializeField] private float lungeDistance = 1f;

    [Header("Sword scale")]
    [SerializeField] private Vector3 swordScaleTo;

    // animation settings
    private float currentYAngle;
    private float targetYAngle;

    private Coroutine attackAnimationCo;

    void Start()
    {
        currentYAngle = sword.localEulerAngles.y;
        targetYAngle = currentYAngle + swordTargetAngle.z;
    }

    public void PlayAttackAnimation()
    {
        if (attackAnimationCo != null) StopCoroutine(attackAnimationCo);
        attackAnimationCo = StartCoroutine(AttackAnimationRoutine());
    }

    private IEnumerator AttackAnimationRoutine()
    {
        var swingCo = StartCoroutine(WeaponSwingRoutine(swingDuration));
        StartCoroutine(LungeForwardRoutine(lungeDuration));
        StartCoroutine(ScaleWeaponRoutine(swingDuration));
        yield return swingCo;
    }

    private IEnumerator WeaponSwingRoutine(float duration)
    {
        float startDuration = duration * 0.7f; // main swing uses ~70% of total time
        float returnDuration = duration - startDuration; // remaining time for return

        Vector3 startAngle = sword.localEulerAngles;
        Vector3 endAngle = swordTargetAngle;

        // since both angles are swapped at the end 
        // start and end angle need to be interpolated
        // startYAngle + endYAngle would yield a different result
        Vector3 offAngle = Vector3.Lerp(startAngle, endAngle, 1f);
        offAngle.y += overshootYAngle;

        // swing with overshoot
        yield return Utils.LerpVector3(startDuration, startAngle, offAngle,
            target => sword.localEulerAngles = target);

        // return from overshoot to end angle
        yield return Utils.LerpVector3(returnDuration, offAngle, endAngle,
            target => sword.localEulerAngles = target);

        // swap target angle
        swordTargetAngle *= -1f;

        // reverse overshoot y angle
        overshootYAngle *= -1f;
    }

    private IEnumerator LungeForwardRoutine(float duration)
    {
        Vector3 startPos = transform.position;
        Vector3 direction = transform.forward * lungeDistance;
        Vector3 endPos = startPos + direction;

        yield return Utils.LerpVector3(duration, startPos, endPos, 
            targetPos => transform.position = targetPos);
    }

    private IEnumerator ScaleWeaponRoutine(float duration)
    {
        float startDuration = duration * 0.7f;
        float returnDuration = duration - startDuration;

        Vector3 fromScale = sword.localScale;
        Vector3 toScale = swordScaleTo;

        yield return Utils.LerpVector3(startDuration, fromScale, toScale, 
            targetScale => sword.localScale = targetScale);

        // swap scale target
        toScale = Utils.SwapVector3(ref fromScale, ref swordScaleTo);

        yield return Utils.LerpVector3(returnDuration, fromScale, toScale, 
            targetScale => sword.localScale = targetScale);
    }

}
