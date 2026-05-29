using System.Collections.Generic;
using CyberpixelOk.Weapons;
using UnityEngine;

namespace CyberpixelOk.Managers
{
    [DisallowMultipleComponent]
    public class GameSessionManager : MonoBehaviour
    {
        public static GameSessionManager Instance { get; private set; }

        [Header("Collectibles")]
        [SerializeField] private int collectibleRequirement;

        private readonly List<WeaponBase> savedWeapons = new List<WeaponBase>();

        public IReadOnlyList<WeaponBase> SavedWeapons => savedWeapons;
        public bool HasSavedPlayerVitals { get; private set; }
        public float SavedPlayerHealth { get; private set; }
        public float SavedPlayerMaxHealth { get; private set; }
        public float SavedJetpackFuel { get; private set; }
        public float SavedJetpackMaxFuel { get; private set; }
        public int SavedEquippedWeaponIndex { get; private set; }
        public int CollectibleRequirement => collectibleRequirement;
        public int CollectedCollectibles { get; private set; }
        public bool HasCollectedRequiredCollectibles => collectibleRequirement > 0 && CollectedCollectibles >= collectibleRequirement;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void SavePlayerVitals(float currentHealth, float maxHealth, float currentFuel, float maxFuel)
        {
            SavedPlayerHealth = currentHealth;
            SavedPlayerMaxHealth = maxHealth;
            SavedJetpackFuel = currentFuel;
            SavedJetpackMaxFuel = maxFuel;
            HasSavedPlayerVitals = true;
        }

        public void SaveWeapons(IReadOnlyList<WeaponBase> weapons, int equippedIndex)
        {
            savedWeapons.Clear();

            if (weapons != null)
            {
                for (int index = 0; index < weapons.Count; index++)
                {
                    WeaponBase weapon = weapons[index];
                    if (weapon != null)
                    {
                        savedWeapons.Add(weapon);
                    }
                }
            }

            SavedEquippedWeaponIndex = equippedIndex;
        }

        public void ClearSession()
        {
            savedWeapons.Clear();
            HasSavedPlayerVitals = false;
            SavedPlayerHealth = 0f;
            SavedPlayerMaxHealth = 0f;
            SavedJetpackFuel = 0f;
            SavedJetpackMaxFuel = 0f;
            SavedEquippedWeaponIndex = -1;
            collectibleRequirement = 0;
            CollectedCollectibles = 0;
        }

        public void SetCollectibleRequirement(int requirement)
        {
            collectibleRequirement = Mathf.Max(0, requirement);
            if (collectibleRequirement > 0)
            {
                CollectedCollectibles = Mathf.Clamp(CollectedCollectibles, 0, collectibleRequirement);
            }
        }

        public void ResetCollectibleProgress()
        {
            CollectedCollectibles = 0;
        }

        public void AddCollectedCollectible(int amount = 1)
        {
            if (amount <= 0)
            {
                return;
            }

            if (collectibleRequirement <= 0)
            {
                CollectedCollectibles += amount;
                return;
            }

            CollectedCollectibles = Mathf.Clamp(CollectedCollectibles + amount, 0, collectibleRequirement);
        }
    }
}
