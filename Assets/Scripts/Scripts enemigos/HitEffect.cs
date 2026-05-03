using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

/// <summary>
/// Añadir a cualquier enemigo para obtener camera shake y partículas al recibir golpes.
/// Requiere CinemachineImpulseSource en el mismo GameObject.
/// </summary>
public class HitEffect : MonoBehaviour
{
    [Header("Camera Shake")]
    [SerializeField] private float shakeForce = 0.3f;

    [Header("Hit Particles")]
    [SerializeField] private GameObject hitParticlePrefab; // Prefab con animación de sprites tipo Blasphemous

    private CinemachineImpulseSource _impulseSource;

    private void Awake()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void PlayHitEffect(Vector2 hitPosition)
    {
        // Camera shake
        if (_impulseSource != null)
            _impulseSource.GenerateImpulse(shakeForce);

        // Partículas
        if (hitParticlePrefab != null)
            Instantiate(hitParticlePrefab, hitPosition, Quaternion.identity);
    }
}