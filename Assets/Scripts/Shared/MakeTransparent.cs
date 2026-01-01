using UnityEngine;

public class MakeTransparent : MonoBehaviour
{
    [SerializeField] private Material transparentMat;

    public void SetMatToTransparent(Renderer[] renderers)
    {
        foreach (var rd in renderers)
        {
            rd.material = transparentMat;
        }
    }
}
