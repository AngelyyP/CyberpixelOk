using System.Collections.Generic;
using CyberpixelOk.Weapons;
using UnityEngine;

namespace CyberpixelOk.Managers
{
    [DisallowMultipleComponent]
    public class GameSessionManager : MonoBehaviour
    {
        public static GameSessionManager Instance { get; private set; }

        private readonly List<WeaponBase> savedWeapons = new List<WeaponBase>();

        public IReadOnlyList<WeaponBase> SavedWeapons => savedWeapons;
        public bool HasSavedPlayerVitals { get; private set; }
        public float SavedPlayerHealth { get; private set; }
        public float SavedPlayerMaxHealth { get; private set; }
        public float SavedJetpackFuel { get; private set; }
        public float SavedJetpackMaxFuel { get; private set; }
        public int SavedEquippedWeaponIndex { get; private set; }

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
        }
    }
}
