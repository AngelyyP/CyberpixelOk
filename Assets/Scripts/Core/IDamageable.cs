namespace CyberpixelOk.Core
{
    public interface IDamageable
    {
        bool IsDead { get; }

        void TakeDamage(DamageInfo damageInfo);
    }
}
