public interface IDamageable
{
    

   

    public void TakeDamage(int damage , DamageType damageType);




}

public enum DamageType
{
    physical, unblockable, magical, heal
}