using UnityEngine;

namespace CyberpixelOk.Weapons
{
    public static class WeaponDirectionUtility
    {
        public static WeaponAimDirection ResolveAimDirection(Vector2 lookInput, bool facingRight)
        {
            if (lookInput.sqrMagnitude <= 0.0001f)
            {
                return WeaponAimDirection.Front;
            }

            float absX = Mathf.Abs(lookInput.x);
            float absY = Mathf.Abs(lookInput.y);

            if (absY > absX * 1.4f)
            {
                return lookInput.y > 0f ? WeaponAimDirection.Up : WeaponAimDirection.Down;
            }

            if (lookInput.y > 0f)
            {
                return WeaponAimDirection.DiagonalUp;
            }

            if (lookInput.y < 0f)
            {
                return WeaponAimDirection.DiagonalDown;
            }

            return facingRight ? WeaponAimDirection.Front : WeaponAimDirection.Front;
        }

        public static Vector2 ToVector(WeaponAimDirection direction, bool facingRight)
        {
            float horizontal = facingRight ? 1f : -1f;

            return direction switch
            {
                WeaponAimDirection.Up => Vector2.up,
                WeaponAimDirection.DiagonalUp => new Vector2(horizontal, 1f).normalized,
                WeaponAimDirection.DiagonalDown => new Vector2(horizontal, -1f).normalized,
                WeaponAimDirection.Down => Vector2.down,
                _ => new Vector2(horizontal, 0f)
            };
        }
    }
}
