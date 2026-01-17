using UnityEngine;

public class Spin : MonoBehaviour
{
    public Transform rot1;
    public Transform rot2;

    // Angular speed in degrees per sec.
    public float speed = 45.0f;
    public float timeCount = 0.0f;

    Vector3 rot1EulerAngles = Vector3.zero;
    Vector3 rot2EulerAngles = Vector3.zero;

    Vector3 currentEulerAngles;
    Quaternion currentRotation;

    float x;
    float y;
    float z;

    void Start()
    {
        // Initialize the quaternion to identity
        currentRotation = Quaternion.identity;
        currentEulerAngles = Vector3.zero;

        rot1EulerAngles += rot1.rotation.eulerAngles;
        rot2EulerAngles += rot2.rotation.eulerAngles;
        float t = Mathf.LerpAngle(rot1EulerAngles.y, rot2EulerAngles.y, 1f);

        Debug.Log("Rot1 euler angle Y: " + rot1EulerAngles.y);
        Debug.Log("Rot2 euler angle Y: " + rot2EulerAngles.y);
        Debug.Log("Interpolated Rot1 and Rot2: " + t);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X)) x = 1 - x;
        if (Input.GetKeyDown(KeyCode.Y)) y = 1 - y;
        if (Input.GetKeyDown(KeyCode.Z)) z = 1 - z;

        //modifying the Vector3, based on input multiplied by speed and time
        currentEulerAngles += new Vector3(x, y, z) * Time.deltaTime * speed;

        //creating a new Quaternion from the updated euler angles
        currentRotation = Quaternion.Euler(currentEulerAngles);

        // set the rotation of the gameObject
        transform.rotation = currentRotation;
    }
}
