using System.Collections;
using UnityEngine;

public class EnemyHitFlash : MonoBehaviour
{
    private const string EMISSIONCOLOR = "_EmissionColor";
    private static readonly int EmissionColor = Shader.PropertyToID(EMISSIONCOLOR);

    [Header("Flash Settings")]
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float flashIntensity = 2f;
    [SerializeField] private float fadeDuration = 0.2f;
    
    // components
    private Renderer[] renderers;
    private MaterialPropertyBlock mpb;

    private Color originalEmission;
    private Coroutine flashCo;

    void Awake()
    {
        renderers = GetComponentsInChildren<MeshRenderer>();
        mpb = new MaterialPropertyBlock();
        
        // cache the original emission color
        // (assuming all shared the same base color)
        if (renderers.Length > 0)
        {
            renderers[0].GetPropertyBlock(mpb);
            originalEmission = mpb.GetColor(EmissionColor);
        }
    }

    public void PlayHitFlash()
    {
        // stop any running flash animation
        if (flashCo != null) StopCoroutine(flashCo);
        // start a new fade coroutine
        flashCo = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // apply hit color instantly
        SetEmissionColor(hitColor * flashIntensity);
        // slowly fade back to original
        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            Color current = Color.Lerp(originalEmission, hitColor * flashIntensity, t);
            // apply emission color to current color
            SetEmissionColor(current);
            yield return null;
        }
        // make sure the original emission color is correct
        SetEmissionColor(originalEmission);
    }

    private void SetEmissionColor(Color color)
    {
        foreach (var rd in renderers)
        {
            rd.GetPropertyBlock(mpb);
            mpb.SetColor(EmissionColor, color);
            rd.SetPropertyBlock(mpb);
        }
    }
}
