using System;
using System.Collections;
using UnityEngine;

[Flags]
public enum SquashStretchAxis
{
    None = 0,
    X = 1,
    Y = 2,
    Z = 4
}

public class SquashAndStretch : MonoBehaviour
{
    [Header("Squash and Stretch Core")] 
    [SerializeField, Tooltip("Defaults to current GO if not set.")] private Transform transformToAffect;
    [SerializeField] private SquashStretchAxis axisToAffect = SquashStretchAxis.Y;
    [SerializeField, Range(0, 1f)] private float animationDuration = 0.25f;
    [SerializeField] private bool canBeOverwritten;
    [SerializeField] private bool playOnStart;
    [SerializeField] private bool playsEveryTime = true;
    [SerializeField, Range(0,100f)] private float chanceToPlay = 100f;
    
    [Header("Animation Settings")] 
    [SerializeField] private float initialScale = 1f;
    [SerializeField] private float maximumScale = 1.3f;
    [SerializeField] private bool resetToInitialScaleAfterAnimation = true;
    [SerializeField] private bool reverseAnimationCurveAfterPlaying;
    private bool _isReversed;

    [SerializeField] private AnimationCurve squashAndStretchCurve = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(0.25f, 1f),
        new Keyframe(1f, 0f)
    );

    // [Header("Looping Settings")] 
    // [SerializeField] private bool looping;
    // [SerializeField] private float loopingDelay = 0.5f;

    private Coroutine _squashAndStretchCoroutine;
    private Vector3 _initialScaleVector;
    // private WaitForSeconds _loopingDelayWaitForSeconds;

    private bool AffectX => (axisToAffect & SquashStretchAxis.X) != 0;
    private bool AffectY => (axisToAffect & SquashStretchAxis.Y) != 0;
    private bool AffectZ => (axisToAffect & SquashStretchAxis.Z) != 0;

    private static event Action SquashAndStretchAllObjectsLikeThisEvent;

    private void Awake()
    {
        if (transformToAffect == null)
            transformToAffect = transform;

        _initialScaleVector = transformToAffect.localScale;
        // _loopingDelayWaitForSeconds = new WaitForSeconds(loopingDelay);
    }

    public static void SquashAndStretchAllObjectsLikeThis()
    {
        SquashAndStretchAllObjectsLikeThisEvent?.Invoke();
    }
    
    private void OnEnable()
    {
        SquashAndStretchAllObjectsLikeThisEvent += PlaySquashAndStretch;
    }

    private void OnDisable()
    {
        if (_squashAndStretchCoroutine != null)
            StopCoroutine(_squashAndStretchCoroutine);

        SquashAndStretchAllObjectsLikeThisEvent -= PlaySquashAndStretch;
    }

    private void Start()
    {
        if (playOnStart)
            CheckForAndStartCoroutine();
    }

    public void PlaySquashAndStretch()
    {
        // if (looping && !canBeOverwritten) 
        //     return;

        CheckForAndStartCoroutine();
    }
    
    private void CheckForAndStartCoroutine()
    {
        if (axisToAffect == SquashStretchAxis.None)
        {
            Debug.Log("Axis to affect is set to None.", gameObject);
            return;
        }

        if (_squashAndStretchCoroutine != null)
        {
            StopCoroutine(_squashAndStretchCoroutine);
            if (playsEveryTime && resetToInitialScaleAfterAnimation)
                transformToAffect.localScale = _initialScaleVector;
        }

        _squashAndStretchCoroutine = StartCoroutine(SquashAndStretchEffect());
    }

    private IEnumerator SquashAndStretchEffect()
    {
        // do
        // {
            
        // } while (looping);

        // if (!playsEveryTime)
        // {
        //     float random = UnityEngine.Random.Range(0, 100f);
        //     if (random > chanceToPlay)
        //     {
        //         yield return null;
        //         continue;
        //     }
        // }
        
        if (reverseAnimationCurveAfterPlaying)
            _isReversed = !_isReversed;
        
        float curvePosition;
        float elapsedTime = 0;
        Vector3 originalScale = _initialScaleVector;
        Vector3 modifiedScale = originalScale;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            
            if (_isReversed)
                curvePosition = 1 - (elapsedTime / animationDuration);
            else
                curvePosition = elapsedTime / animationDuration;
            
            float curveValue = squashAndStretchCurve.Evaluate(curvePosition);
            float remappedValue = initialScale + (curveValue * (maximumScale - initialScale));

            float minimumThreshold = 0.0001f;
            if (Mathf.Abs(remappedValue) < minimumThreshold)
                remappedValue = minimumThreshold;

            if (AffectX)
                modifiedScale.x = originalScale.x * remappedValue;
            else
                modifiedScale.x = originalScale.x / remappedValue;

            if (AffectY)
                modifiedScale.y = originalScale.y * remappedValue;
            else
                modifiedScale.y = originalScale.y / remappedValue;

            if (AffectZ)
                modifiedScale.z = originalScale.z * remappedValue;
            else
                modifiedScale.z = originalScale.z / remappedValue;

            transformToAffect.localScale = modifiedScale;

            yield return null;
        }

        if (resetToInitialScaleAfterAnimation)
            transformToAffect.localScale = originalScale;

        // if (looping)
        //     yield return _loopingDelayWaitForSeconds;
    }

    // public void SetLooping(bool shouldLoop)
    // {
    //     looping = shouldLoop;
    // }

    // public void Setup(SquashStretchAxis axis, float time, float zeroMap, float oneMap, AnimationCurve curve,
    //     bool loop, float delay, bool playImmediately = false)
    // {
    //     axisToAffect = axis;
    //     animationDuration = time;
    //     initialScale = zeroMap;
    //     maximumScale = oneMap;
    //     squashAndStretchCurve = curve;
    //     looping = loop;
    //     loopingDelay = delay;

    //     _loopingDelayWaitForSeconds = new WaitForSeconds(loopingDelay);

    //     if (playImmediately) CheckForAndStartCoroutine();
    // }
}