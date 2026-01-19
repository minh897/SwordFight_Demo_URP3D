using System.Collections;
using Assets.Utility;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public Transform rot1;
    public Transform rot2;

    // Angular speed in degrees per sec.
    public float speed = 45.0f;
    public float timeCount = 0.0f;

    float rot1EulerAngles;
    float rot2EulerAngles;

    Vector3 currentEulerAngles;
    Quaternion currentRotation;

    float x;
    float y;
    float z;
    float f;
    float dir = -1;

    void Start()
    {
        // Initialize the quaternion to identity
        currentRotation = Quaternion.identity;
        currentEulerAngles = transform.localEulerAngles;
        SetupEulerAngles();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X)) x = 1 - x; // toggle x rotation
        if (Input.GetKeyDown(KeyCode.Y)) y = 1 - y; // toggle y rotation
        if (Input.GetKeyDown(KeyCode.Z)) z = 1 - z; // toggle z rotation
        if (Input.GetKeyDown(KeyCode.F)) f = 1 - f; // toggle FreeSpinning
        if (Input.GetKeyDown(KeyCode.V)) dir *= -1; // change spin direction
        if (Input.GetKeyDown(KeyCode.C)) StartTestSpinCo();
        
        if (f == 1) FreeSpinning();
    }

    private void SetupEulerAngles()
    {
        rot1EulerAngles = rot1.localEulerAngles.y;
        rot2EulerAngles = rot2.localEulerAngles.y;
        float t = Mathf.DeltaAngle(rot1EulerAngles, rot2EulerAngles);
        Debug.Log("Rot1 euler angle Y: " + rot1EulerAngles);
        Debug.Log("Rot2 euler angle Y: " + rot2EulerAngles);
        Debug.Log("Interpolated Rot1 and Rot2: " + t);
    }

    private void FreeSpinning()
    {
        // modifying the Vector3, based on input multiplied by speed and time
        currentEulerAngles += dir * speed * Time.deltaTime * new Vector3(x, y, z);
        //creating a new Quaternion from the updated euler angles
        currentRotation = Quaternion.Euler(currentEulerAngles);
        // set the rotation of the gameObject
        transform.rotation = currentRotation;
    }

    private void StartTestSpinCo()
    {
        StartCoroutine(TestSpinCo());
    }

    private IEnumerator TestSpinCo()
    {
        currentEulerAngles = transform.localEulerAngles;
        float targetYAngle = Mathf.LerpAngle(rot1EulerAngles, rot2EulerAngles, 1f);
        Debug.Log("Current euler angles " + currentEulerAngles.y);
        Debug.Log("Target euler angles " + targetYAngle);

        yield return Utils.LerpOverTime(.5f, currentEulerAngles.y, targetYAngle * dir,
            newAngle =>
            {
                Vector3 newEulerAngles = new(0f, newAngle, 0f);
                Quaternion newRotation = Quaternion.Euler(newEulerAngles);
                transform.rotation = newRotation;
            });

        dir *= -1; // change spin direction
        (rot2EulerAngles, rot1EulerAngles) = (rot1EulerAngles, rot2EulerAngles);
    }
}
