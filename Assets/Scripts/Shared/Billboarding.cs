using UnityEngine;

public class Billboarding : MonoBehaviour
{
    void Update()
    {
        transform.rotation = Quaternion.Euler(0f, Camera.main.transform.eulerAngles.y, 0f);
    }
}
