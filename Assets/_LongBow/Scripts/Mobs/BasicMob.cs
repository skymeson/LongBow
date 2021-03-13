/// <summary>
/// Base class for all mobs.
/// </summary>
namespace LongBow
{
    using ScriptableObjectArchitecture;
    using UnityEngine;

    public class BasicMob : MonoBehaviour
    {
        // this will handle mob health, movement, and attacks
        // can be inherited to create new mobs

        // a mob will be spawned, move towards the levels target, and attack the target on arrival
        // mobs can be damaged and if their health is reduced to 0 they are despawned
        [SerializeField] private IntReference startingHealth = default;

        private bool isActive = false;
        private int currentHealth;

        protected virtual void Update()
        {
            if (!isActive) return;
            OnMobUpdate();
        }

        /// <summary>
        /// Call when a mob is spawned into the game.
        /// </summary>
        public virtual void OnMobSpawned()
        {
            currentHealth = startingHealth.Value;
            isActive = true;
        }

        /// <summary>
        /// Call when mob is damaged.
        /// </summary>
        public virtual void OnMobDamaged(int damage)
        {
            if (!isActive) return;
            currentHealth -= damage;
            if (currentHealth > 0) return;
            OnMobDestroyed();
        }

        /// <summary>
        /// Call if the mob is destroyed by an external event.
        /// </summary>
        public virtual void ForceDestroyMob()
        {
            if (!isActive) return;
            OnMobDestroyed();
        }

        protected virtual void OnMobDestroyed()
        {
            if (!isActive) return;
            isActive = false;
            currentHealth = 0;
        }

        protected virtual void OnMobUpdate()
        {
            if (!isActive) return;

            // do stuff while mob ticks
            // generally moving towards target
        }
    }
}
