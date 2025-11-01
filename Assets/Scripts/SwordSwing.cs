using System.Collections;
using UnityEngine;

public class SwordSwing : MonoBehaviour
{
    [SerializeField] private Transform swordPivot;
    [SerializeField] private float swingDuration = 0.3f;
    [SerializeField] private float rotationAngle = 180f;

    private bool swinging;
    private float currentYRotation;
    private float targetYRotation;

    private void Start()
    {
        currentYRotation = swordPivot.localEulerAngles.y;
        targetYRotation = currentYRotation + rotationAngle;
    }

    public void Swing()
    {
        // Since the swing only activate upon input the coroutine can only resume then
        // it doesn't matter if the result of a yield return is. So I need to find out
        // how to continue the swing even when there is no input. Nope that is not it. 
        // It was the swapping of current rotation and targetRotation that messes everything up
        if (!swinging)
            StartCoroutine(SwingCoroutine());
    }

    private IEnumerator SwingCoroutine()
    {
        Debug.Log("Start swinging " + swordPivot.localEulerAngles);
        swinging = true;

        float elapsed = 0f;
        float startY = currentYRotation;
        float endY = targetYRotation;

        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / swingDuration);
            float yRot = Mathf.Lerp(startY, endY, t);
            swordPivot.localRotation = Quaternion.Euler(0, yRot, 0);

            Debug.Log("Swinging rotation: " + swordPivot.localEulerAngles);
            yield return null;
        }

        // swap for next swing
        currentYRotation = endY;
        Debug.Log("Current Y: " + currentYRotation);
        targetYRotation = startY;
        Debug.Log("Target Y: " + targetYRotation);

        swinging = false;
    }
}
