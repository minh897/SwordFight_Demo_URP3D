using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageFlash : MonoBehaviour
{
    public IReadOnlyList<Renderer> Renderers => renderers;

    [Header("Flash Settings")]
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float flashIntensity = 2f;
    [SerializeField] private float fadeDuration = 0.2f;
    
    private Renderer[] renderers;

    private MaterialPropertyBlock mpb;
    private Color originalEmission;
    private Coroutine flashCo;

    private const string EMISSIONCOLOR = "_EmissionColor";
    private static readonly int EmissionColor = Shader.PropertyToID(EMISSIONCOLOR);

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

    public void FlashColor()
    {
        // stop any running flash animation
        if (flashCo != null) StopCoroutine(flashCo);
        // start a new fade coroutine
        flashCo = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // apply hit color instantly
        Color targetEmission = hitColor * flashIntensity;
        SetEmissionColor(targetEmission, renderers);

        // slowly fade back to original
        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            Color current = Color.Lerp(targetEmission, originalEmission, t);
            // apply emission color to current color
            SetEmissionColor(current, renderers);
            yield return null;
        }
        // make sure the original emission color is correct
        SetEmissionColor(originalEmission, renderers);
    }

    private void SetEmissionColor(Color color, Renderer[] rendererList)
    {
        foreach (var rd in rendererList)
        {
            rd.GetPropertyBlock(mpb);
            mpb.SetColor(EmissionColor, color);
            rd.SetPropertyBlock(mpb);
        }
    }
}
