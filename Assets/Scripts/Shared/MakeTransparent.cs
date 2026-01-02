using System.Collections.Generic;
using UnityEngine;

public class MakeTransparent : MonoBehaviour
{
    [SerializeField] private Material transparentMat;

    public void SetMatToTransparent(IReadOnlyList<Renderer> renderers)
    {
        foreach (var rd in renderers)
        {
            rd.material = transparentMat;
        }
    }
}
