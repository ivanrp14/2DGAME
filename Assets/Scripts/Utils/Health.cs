using System.Collections;
using Microlight.MicroBar;
using UnityEngine;
using UnityEngine.Events;

namespace Utils
{
    public class Health : MonoBehaviour
    {
        [SerializeField]
        private float maxhealth;

        [SerializeField]
        private float health;

        public UnityAction<float> Hurt;
        public UnityAction<float> Heal;

        public UnityAction OnZeroLifes;
        public bool canTakeDamage = true;
        public bool canTakeHeal = true;

        [Header("Blink Effect")]
        public bool blink = true;
        public float blinkDuration = 0.5f;

        public float shrinkDuration = 0.5f;
        public bool shrinkOnDeath = true;

        public MicroBar microBar;
        private SpriteRenderer[] spriteRenderers;

        // ✅ Nueva variable: indica si se está recibiendo daño actualmente
        public bool isGettingDamage = false;

        private void Start()
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            health = maxhealth;
            if (microBar != null)
            {
                microBar.Initialize(maxhealth);
            }
            else
            {
                Debug.LogWarning("MicroBar component not found. Health bar will not be displayed.");
            }
        }

        public float GetMaxHealth() => maxhealth;
        public float GetCurrenthealth() => health;
        public void SetMaxHealth(float maxh) => maxhealth = maxh;
        public void SetHealth(float h) => health = h;

        public void TakeDamage(float damage)
        {
            if (damage < 0)
            {
                Debug.LogWarning("Damage cannot be negative. Damage value: " + damage);
                return;
            }
            if (health <= 0)
            {
                Debug.LogWarning("Cannot take damage when health is already zero or less. Current health: " + health);
                return;
            }
            if (!canTakeDamage)
            {
                return;
            }

            health -= damage;

            if (microBar != null)
            {
                microBar.UpdateBar(health);
            }

            Hurt?.Invoke(health);

            // ✅ Al recibir daño, activar isGettingDamage y blink
            if (blink)
            {
                StartCoroutine(BlinkEffect(blinkDuration));
            }
            else
            {
                // Si no hay blink, marcar como daño momentáneo
                StartCoroutine(BriefDamageState());
            }

            if (health <= 0)
            {
                OnZeroLifes?.Invoke();
                canTakeDamage = false;
            }
        }

        public void TakeHeal(float heal)
        {
            if (!canTakeHeal)
            {
                return;
            }
            if (health + heal > maxhealth)
            {
                health = maxhealth;
            }
            else
            {
                health += heal;
            }
            if (microBar != null)
            {
                microBar.UpdateBar(health);
            }
            Heal?.Invoke(health);
        }

        IEnumerator BlinkEffect(float duration = 0.5f)
        {
            if (spriteRenderers == null || spriteRenderers.Length == 0)
            {
                Debug.LogWarning("No SpriteRenderer found for blink effect.");
                yield break;
            }

            float delay = duration / 5f;
            canTakeDamage = false;
            isGettingDamage = true; // ✅ Activamos flag de daño

            for (int i = 0; i < 5; i++)
            {
                foreach (SpriteRenderer sr in spriteRenderers)
                    sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);

                yield return new WaitForSeconds(delay);

                foreach (SpriteRenderer sr in spriteRenderers)
                    sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);

                yield return new WaitForSeconds(delay);
            }

            canTakeDamage = true;
            isGettingDamage = false; // ✅ Al terminar el blink, ya no se está recibiendo daño
        }

        // ✅ Si no hay blink, activamos isGettingDamage durante un frame breve
        IEnumerator BriefDamageState()
        {
            isGettingDamage = true;
            yield return new WaitForSeconds(0.1f);
            isGettingDamage = false;
        }
    }
}
