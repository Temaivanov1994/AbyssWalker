using UnityEngine;

public class CreatureStats : MonoBehaviour, IDamageable
{

    [SerializeField] private int maxHealth = 5;
    public int currentHealth;
    [SerializeField] private int maxStunHealth = 3;
    public int currentStunHealth;
    [SerializeField] private int armor = 1;
    [SerializeField] private int magicalResistance = 3;


    [SerializeField] protected HealthBar healthBar;
    [SerializeField] private ParticleSystem bloodPartical;
    [SerializeField] private Enemy enemy;
    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        healthBar = GetComponentInChildren<HealthBar>();
        
        bloodPartical = Resources.Load<ParticleSystem>("Charters/ParticalSystems and effects/Particle System Blood");
       
    }

    private void Start()
    {
        InitHealthBar();
    }

    private void InitHealthBar()
    {
        currentHealth = maxHealth;
        currentStunHealth = maxStunHealth;
        healthBar.SetMaxHealth(maxHealth, currentHealth);
        healthBar.SetHealth(currentHealth);
    }
   
    public void TakeDamage(int damageOrHealValue, DamageType damageType)
    {

        switch (damageType)
        {
            case DamageType.physical:
                PhysicalDamage(damageOrHealValue);
                break;
            case DamageType.unblockable:
                UnblockableDamage(damageOrHealValue);
                break;
            case DamageType.magical:
                MagicalDamage(damageOrHealValue);
                break;
            case DamageType.heal:
                Heal(damageOrHealValue);
                break;

        }


        enemy.SwitchState(Enemy.State.TakeDamage);

        /*SwitchState(State.TakeDamage);*/







    }



    private void PhysicalDamage(int damage)
    {
        int pureDamage = damage - armor ;
        pureDamage = Mathf.Clamp(pureDamage, 1, damage);
        currentHealth -= pureDamage;
        currentStunHealth -= pureDamage;
        OnChangedHealthBar(pureDamage);
    }

    private void UnblockableDamage(int damage)
    {
        currentHealth -= damage;
        currentStunHealth -= damage;
        OnChangedHealthBar(damage);
    }

    private void MagicalDamage(int damage)
    {
        int pureDamage = damage- magicalResistance;
        pureDamage = Mathf.Clamp(pureDamage, 1, damage);
        currentHealth -= pureDamage;
        OnChangedHealthBar(pureDamage);
    }
    private void Heal(int heal)
    {
        currentHealth += heal;
        currentHealth = Mathf.Clamp(currentHealth, 1, maxHealth);
        OnChangedHealthBar(heal);

    }

    private void OnChangedHealthBar(int value)
    {
        if (currentHealth > 0)
        {
            healthBar.ShowFloatingText(value, transform);
        }
        else if (currentHealth <= 0)
        {
            healthBar.enabled = false;
        }
        healthBar.SetHealth(currentHealth);

        Instantiate(bloodPartical, transform.position, bloodPartical.transform.rotation);


    }



    public void RecoveryStunHealth()
    {
        currentStunHealth = maxStunHealth;
    }
}
