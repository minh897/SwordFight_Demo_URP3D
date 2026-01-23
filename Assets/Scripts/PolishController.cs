using System;
using UnityEngine;

public class PolishController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerAttackAnim attackAnim;
    [SerializeField] private PlayerWeaponHitHandler hitHandler;
    [SerializeField] private Health health;

    [Header("Polish")]
    [SerializeField] ParticleSystem swingVFX;
    [SerializeField] ParticleSystem hitSparkVFX;
    [SerializeField] AudioSource swingSFX;
    [SerializeField] AudioSource impactSFX;
    [SerializeField] ShakeCamera cameraShake;

    void OnEnable()
    {
        attackAnim.OnSwingStarted += HandleSwing;
        hitHandler.OnHitFrame += HandleHitFrame;
        hitHandler.OnHitReaction += HandleHitReaction;
    }

    void OnDisable()
    {
        attackAnim.OnSwingStarted -= HandleSwing;
        hitHandler.OnHitFrame -= HandleHitFrame;
        hitHandler.OnHitReaction -= HandleHitReaction;
    }

    void HandleSwing()
    {
        if (swingVFX) swingVFX.Play();
        if (swingSFX) swingSFX.Play();
    }

    void HandleHitFrame()
    {
        if (hitSparkVFX) hitSparkVFX.Play();
        if (impactSFX) impactSFX.Play();
        cameraShake.PlayBounceShake();
    }

    private void HandleHitReaction()
    {
        // play enemy visual, sound effects and animation
    }

}
