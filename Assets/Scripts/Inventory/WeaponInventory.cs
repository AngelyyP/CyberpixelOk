using System;
using System.Collections.Generic;
using CyberpixelOk.Managers;
using CyberpixelOk.Weapons;
using UnityEngine;

namespace CyberpixelOk.Inventory
{
    [DisallowMultipleComponent]
    public class WeaponInventory : MonoBehaviour
    {
        [SerializeField] private List<WeaponBase> startingWeapons = new List<WeaponBase>();
        [SerializeField] private int startingEquippedIndex;

        private readonly List<WeaponLoadoutEntry> loadout = new List<WeaponLoadoutEntry>();

        public event Action<WeaponBase> EquippedWeaponChanged;
        public event Action WeaponsChanged;

        public IReadOnlyList<WeaponLoadoutEntry> Loadout => loadout;
        public WeaponBase CurrentWeapon => CurrentLoadoutEntry != null ? CurrentLoadoutEntry.Weapon : null;
        public int CurrentWeaponIndex { get; private set; } = -1;

        private WeaponLoadoutEntry CurrentLoadoutEntry => CurrentWeaponIndex >= 0 && CurrentWeaponIndex < loadout.Count ? loadout[CurrentWeaponIndex] : null;

        private void Awake()
        {
            if (loadout.Count == 0 && startingWeapons.Count > 0)
            {
                for (int index = 0; index < startingWeapons.Count; index++)
                {
                    AddWeapon(startingWeapons[index]);
                }

                if (loadout.Count > 0)
                {
                    EquipWeapon(Mathf.Clamp(startingEquippedIndex, 0, loadout.Count - 1));
                }
            }
        }

        public bool AddWeapon(WeaponBase weapon)
        {
            if (weapon == null || HasWeapon(weapon))
            {
                return false;
            }

            loadout.Add(new WeaponLoadoutEntry(weapon));

            if (CurrentWeaponIndex < 0)
            {
                EquipWeapon(0);
            }

            WeaponsChanged?.Invoke();
            return true;
        }

        public bool HasWeapon(WeaponBase weapon)
        {
            if (weapon == null)
            {
                return false;
            }

            for (int index = 0; index < loadout.Count; index++)
            {
                if (loadout[index].Weapon == weapon)
                {
                    return true;
                }
            }

            return false;
        }

        public void EquipWeapon(int index)
        {
            if (loadout.Count == 0)
            {
                CurrentWeaponIndex = -1;
                EquippedWeaponChanged?.Invoke(null);
                return;
            }

            CurrentWeaponIndex = Mathf.Clamp(index, 0, loadout.Count - 1);
            EquippedWeaponChanged?.Invoke(CurrentWeapon);
        }

        public void EquipNextWeapon()
        {
            if (loadout.Count == 0)
            {
                return;
            }

            int nextIndex = CurrentWeaponIndex + 1;
            if (nextIndex >= loadout.Count)
            {
                nextIndex = 0;
            }

            EquipWeapon(nextIndex);
        }

        public void EquipPreviousWeapon()
        {
            if (loadout.Count == 0)
            {
                return;
            }

            int previousIndex = CurrentWeaponIndex - 1;
            if (previousIndex < 0)
            {
                previousIndex = loadout.Count - 1;
            }

            EquipWeapon(previousIndex);
        }

        public bool TryFireCurrentWeapon(WeaponContext weaponContext)
        {
            WeaponLoadoutEntry currentEntry = CurrentLoadoutEntry;
            if (currentEntry == null || currentEntry.Weapon == null)
            {
                return false;
            }

            return currentEntry.Weapon.TryFire(weaponContext, currentEntry.RuntimeState);
        }

        public void AddAmmoToCurrentWeapon(int ammoAmount)
        {
            WeaponLoadoutEntry currentEntry = CurrentLoadoutEntry;
            if (currentEntry == null || currentEntry.Weapon == null)
            {
                return;
            }

            currentEntry.Weapon.AddAmmo(currentEntry.RuntimeState, ammoAmount);
        }

        public void AddAmmoToAllWeapons(int ammoAmount)
        {
            for (int index = 0; index < loadout.Count; index++)
            {
                WeaponLoadoutEntry entry = loadout[index];
                if (entry?.Weapon == null)
                {
                    continue;
                }

                entry.Weapon.AddAmmo(entry.RuntimeState, ammoAmount);
            }
        }

        public void RestoreFromSession(GameSessionManager sessionManager)
        {
            if (sessionManager == null)
            {
                return;
            }

            loadout.Clear();
            for (int index = 0; index < sessionManager.SavedWeapons.Count; index++)
            {
                AddWeapon(sessionManager.SavedWeapons[index]);
            }

            if (sessionManager.SavedEquippedWeaponIndex >= 0 && sessionManager.SavedEquippedWeaponIndex < loadout.Count)
            {
                EquipWeapon(sessionManager.SavedEquippedWeaponIndex);
            }
        }

        public void SaveToSession(GameSessionManager sessionManager)
        {
            if (sessionManager == null)
            {
                return;
            }

            List<WeaponBase> savedWeapons = new List<WeaponBase>(loadout.Count);
            for (int index = 0; index < loadout.Count; index++)
            {
                if (loadout[index]?.Weapon != null)
                {
                    savedWeapons.Add(loadout[index].Weapon);
                }
            }

            sessionManager.SaveWeapons(savedWeapons, CurrentWeaponIndex);
        }
    }
}
