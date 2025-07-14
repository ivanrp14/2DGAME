using UnityEngine;

public class Effect : MonoBehaviour, IPoolable
{
    private float timer;
    private bool isActive;

    private ParticleSystem particleSystem;
    private float effectDuration;

    void Awake()
    {
        particleSystem = GetComponentInChildren<ParticleSystem>();

        if (particleSystem != null)
        {
            var main = particleSystem.main;
            effectDuration = main.duration + main.startLifetime.constantMax;
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} no tiene ParticleSystem. Usa duración manual o amplía soporte.");
            effectDuration = 1f;
        }
    }

    void Update()
    {
        if (!isActive) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            PoolManager.Instance.Return(gameObject);
        }
    }


    public void OnReturnToPool()
    {
        throw new System.NotImplementedException();
    }
}
