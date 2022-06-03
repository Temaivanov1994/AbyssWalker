using UnityEngine;

[RequireComponent(typeof (CreatureStats))]
public class Enemy : MonoBehaviour
{



    /*[Header("Loot")]
    [SerializeField] private GameObject Loot;
    private bool isLooted = false;
    [SerializeField] private Transform LootPoint;*/


    /* [Header("Audio")]
     [SerializeField] private AudioSource steps;
     [SerializeField] private AudioSource hurtSound;
     [SerializeField] private AudioSource detectSound;*/

    [SerializeField] private CreatureStats creatureStats;


    [Header("Checks")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private float playerCheckInCloseRangeAction;
    [SerializeField] private float playerCheckInMinAgroDistance;
    [SerializeField] private float playerCheckInMaxAgroDistance;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Transform wallCheckPoint;
    [SerializeField] private Transform playerCheckPoint;

    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsPlayer;

    private bool isGroundDetected;
    private bool isWallDetected;
    private bool isAttackPlayerDistance;
    private bool isScaredPlayerDistance;


    [Space]
    [SerializeField]
   
    private ParticleSystem blowPartical;
    private Rigidbody2D rb;
    private Animator anim;
   


    private void Awake()
    {
        creatureStats = GetComponent<CreatureStats>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
       
    }
    private void Start()
    {
       
        SwitchState(State.SerchPlayer);
        
        blowPartical = Resources.Load<ParticleSystem>("Charters/ParticalSystems and effects/Particle System Blow");


    }
    private void Update()
    {
       

        StateMachine();
        Detecting();
    }


    #region StateMachine

    private State currentState;




    private void StateMachine()
    {
        switch (currentState)
        {
            case State.Walk:
                UpdateWalkState();
                break;
            case State.Idle:
                UpdateIdleState();
                break;
            case State.Stun:
                UpdateStunState();
                break;
            case State.Charge:
                UpdateChargeState();
                break;
            case State.PlayerDetected:
                UpdatePlayerDetectedState();
                break;
            case State.SerchPlayer:
                UpdateSerchPlayerState();
                break;
            case State.MelleAttack:
                UpdateMelleAttackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
            case State.TakeDamage:
                UpdateDamagedState();
                break;


        }
    }
    public void SwitchState(State state)
    {
        switch (currentState)
        {
            case State.Idle:
                ExitIdleState();
                break;
            case State.Walk:
                ExitWalkState();
                break;
            case State.Stun:
                ExitStunState();
                break;
            case State.Charge:
                ExitChargeState();
                break;
            case State.PlayerDetected:
                ExitPlayerDetectedState();
                break;
            case State.SerchPlayer:
                ExitSerchPlayerState();
                break;
            case State.MelleAttack:
                ExitMelleAttackState();
                break;
            case State.Dead:
                ExitDeadState();
                break;
            case State.TakeDamage:
                ExitDamagedState();
                break;
        }
        switch (state)
        {
            case State.Idle:
                EnterIdleState();
                break;
            case State.Walk:
                EnterWalkState();
                break;
            case State.Stun:
                EnterStunState();
                break;
            case State.Charge:
                EnterChargeState();
                break;
            case State.PlayerDetected:
                EnterPlayerDetectedState();
                break;
            case State.SerchPlayer:
                EnterSerchPlayerState();
                break;
            case State.MelleAttack:
                EnterMelleAttackState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
            case State.TakeDamage:
                EnterDamagedState();
                break;
        }
        currentState = state;


    }

    public enum State
    {
        Idle,
        Walk,
        Stun,
        Charge,
        PlayerDetected,
        SerchPlayer,
        MelleAttack,
        Dead,
        TakeDamage,
    }
    #endregion




    #region Stats

    

    [Header("Battle Stats")]
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float knockbackForce = 200;



    #endregion

    #region IdleState

    [Header("IdleState")]
    [SerializeField] private float IdleMinTimeDuration = 1f;
    [SerializeField] private float IdleMaxTimeDuration = 3f;
    private float IdleTimeLeft;
    private float IdleDuration;
    private void EnterIdleState()
    {
        anim.SetBool("Idle", true);
        IdleTimeLeft = 0;
        movement.Set(0, rb.velocity.y);
        rb.velocity = movement;
        IdleDuration = Random.Range(IdleMinTimeDuration, IdleMaxTimeDuration);
    }
    private void UpdateIdleState()
    {

        if (IdleTimeLeft >= IdleDuration)
        {
            SwitchState(State.Walk);
        }

        if (isDetectedPlayerDistance && isGroundDetected)
        {
            SwitchState(State.PlayerDetected);
        }
        IdleTimeLeft += Time.deltaTime;


    }
    private void ExitIdleState()
    {
        anim.SetBool("Idle", false);
        if (!isGroundDetected || isWallDetected)
        {
            Flip();
        }
    }


    #endregion

    #region WalkState


    [Header("WalkState")]
    [SerializeField] private float moveSpeed = 3f;
    private int facingDirection = 1;
    private Vector2 movement;
    private void EnterWalkState()
    {
        anim.SetBool("Walk", true);
    }
    private void UpdateWalkState()
    {
        if (!isGroundDetected || isWallDetected)
        {
            SwitchState(State.Idle);
        }
        else if (isDetectedPlayerDistance)
        {
            SwitchState(State.PlayerDetected);
        }
        else
        {
            movement.Set(moveSpeed * facingDirection, rb.velocity.y);
            rb.velocity = movement;
        }
    }
    private void ExitWalkState()
    {
        anim.SetBool("Walk", false);
    }

    #endregion

    #region StunState

    [Header("StunState")]
    [SerializeField] private float stunDuration = 2f;
    private float stunTimeLeft;
    private void EnterStunState()
    {
        stunTimeLeft = 0;
        movement.Set(0, rb.velocity.y);
        rb.velocity = movement;
        anim.SetBool("Stun", true);
        creatureStats.RecoveryStunHealth();
    }
    private void UpdateStunState()
    {
        if (stunTimeLeft >= stunDuration)
        {
            if (isAttackPlayerDistance)
            {
                SwitchState(State.MelleAttack);
            }
            else
            {
                SwitchState(State.SerchPlayer);

            }
        }
        stunTimeLeft += Time.deltaTime;

    }
    private void ExitStunState()
    {
        anim.SetBool("Stun", false);
    }

    #endregion

    #region ChargeState

    [Header("ChargeState")]
    [SerializeField] private float chargeMoveSpeed = 7f;
    [SerializeField] private float chargeDuration = 2f;
    private float chargeTimeLeft;

    private void EnterChargeState()
    {
        anim.SetBool("Charge", true);
        chargeTimeLeft = 0;
    }
    private void UpdateChargeState()
    {

        if (chargeTimeLeft >= chargeDuration || !isGroundDetected || isWallDetected)
        {
            SwitchState(State.SerchPlayer);
        }
        else
        {
            movement.Set(chargeMoveSpeed * facingDirection, rb.velocity.y);
            rb.velocity = movement;
            if (isAttackPlayerDistance && !isAttack)
            {
                SwitchState(State.MelleAttack);
            }
        }
        chargeTimeLeft += Time.deltaTime;
    }
    private void ExitChargeState()
    {
        anim.SetBool("Charge", false);
    }

    #endregion

    #region DetectedPlayer

    [Header("PlayerDetectedState")]
    [SerializeField] private float detectedPlayerDuration = 0.2f;
    private float detectedPlayerTimeLeft = 0;
    private bool isDetectedPlayerDistance;
    private void EnterPlayerDetectedState()
    {
        anim.SetBool("PlayerDetected", true);
        movement.Set(0, rb.velocity.y);
        rb.velocity = movement;
        detectedPlayerTimeLeft = 0;
    }
    private void UpdatePlayerDetectedState()
    {
        if (detectedPlayerTimeLeft >= detectedPlayerDuration)
        {
            SwitchState(State.Charge);
        }
        else if (isAttackPlayerDistance && !isAttack)
        {
            SwitchState(State.MelleAttack);
        }

        detectedPlayerTimeLeft += Time.deltaTime;
    }
    private void ExitPlayerDetectedState()
    {
        anim.SetBool("PlayerDetected", false);
    }
    //SerchPlayer___________________________________________________

    #endregion

    #region SerchPlayer

    [Header("SerchPlayerState")]
    [SerializeField] private float timeBetweenTurns = 1f;
    [SerializeField] private int amountTurns = 3;
    private float flipTimeLeft;
    private int amountTurnsDone;
    private bool isAllTurnsDone, isAllTunrsTimeDone, turnImmediately;
    private void EnterSerchPlayerState()
    {
        anim.SetBool("Idle", true);
        movement.Set(0, rb.velocity.y);
        rb.velocity = movement;
        amountTurns = Random.Range(2, 5);
        amountTurnsDone = 0;
        flipTimeLeft = 0;
        isAllTurnsDone = false;
        isAllTunrsTimeDone = false;
    }
    private void UpdateSerchPlayerState()
    {


        if (flipTimeLeft >= timeBetweenTurns && !isAllTurnsDone)
        {
            Flip();
            flipTimeLeft = 0;
            amountTurnsDone++;
        }

        if (amountTurnsDone >= amountTurns)
        {
            isAllTurnsDone = true;
        }

        if (Time.time >= flipTimeLeft + timeBetweenTurns && isAllTurnsDone)
        {
            isAllTunrsTimeDone = true;
        }

        if (isDetectedPlayerDistance && isGroundDetected)
        {
            SwitchState(State.PlayerDetected);
        }
        else if (isAllTunrsTimeDone)
        {
            SwitchState(State.Idle);
        }

        flipTimeLeft += Time.deltaTime;
    }
    private void ExitSerchPlayerState()
    {
        anim.SetBool("Idle", false);
    }

    #endregion  

    #region Attack

    [Header("MelleAttack")]
    [SerializeField] private AttackType attackType;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float melleAttackRadius = 0.84f;

   
    [SerializeField] private bool isAttack = false;
    [SerializeField] private float attackDuration;
    [SerializeField] private float attackCooldown;
    private float attackCooldownTimeLeft = 0;
    private float attackTimeLeft = 0;

    [Header("BlowAttack")]
    [SerializeField] private float blowAttackRadius = 1f;
    [SerializeField] private bool isDetectedPlayerInRadius;

    [Header("RangeAttack")]

    [SerializeField] private GameObject projectTile;
    [SerializeField] private float speedProjectTile;
    [SerializeField] private float travelDistanceProjectTile;

    private void EnterMelleAttackState()
    {
        anim.SetBool("MelleAttack", true);
        movement.Set(0, rb.velocity.y);
        rb.velocity = movement;
        isAttack = true;
        attackTimeLeft = 0;
        attackCooldownTimeLeft = 0;

    }
    private void UpdateMelleAttackState()
    {
        switch (attackType)
        {
            case AttackType.melleAttack:

                if (attackTimeLeft >= attackDuration)
                {

                    if (attackCooldownTimeLeft >= attackCooldown)
                    {
                        SwitchState(State.SerchPlayer);
                    }
                    else
                    {
                        SwitchState(State.Idle);
                    }

                }
                break;
            case AttackType.rangeAttack:
                if (attackTimeLeft >= attackDuration)
                {

                    if (attackCooldownTimeLeft >= attackCooldown)
                    {
                        SwitchState(State.SerchPlayer);
                    }
                    else
                    {
                        SwitchState(State.Idle);
                    }

                }
               
                break;
            case AttackType.blow:

                if (attackTimeLeft <= attackDuration)
                {
                    if (!isDetectedPlayerInRadius)
                    {
                        SwitchState(State.SerchPlayer);
                    }

                }

                break;
        }

        attackCooldownTimeLeft += Time.deltaTime;
        attackTimeLeft += Time.deltaTime;
    }
    private void ExitMelleAttackState()
    {
        isAttack = false;
        anim.SetBool("MelleAttack", false);
    }

    public void SwitcherAttackType()
    {
        switch (attackType)
        {
            case AttackType.melleAttack:
                MelleAttack();
                break;
            case AttackType.rangeAttack:
                RangeAttack();
                break;
            case AttackType.blow:
                Blow();
                break;
        }
    }

    private void MelleAttack()
    {
        Collider2D[] hitEnemy = Physics2D.OverlapCircleAll(attackPoint.position, melleAttackRadius, whatIsPlayer);

        foreach (Collider2D target in hitEnemy)
        {
            Rigidbody2D rbTarget = target.GetComponent<Rigidbody2D>();

            target.GetComponent<IDamageable>().TakeDamage(attackDamage, DamageType.physical);
            Vector2 knockbackDirection = new Vector2(target.transform.position.x - transform.position.x, 1.5f);
            rbTarget.AddForce(knockbackDirection.normalized * knockbackForce);

        }
    }

    private void Blow()
    {
        Collider2D[] hitEnemy = Physics2D.OverlapCircleAll(transform.position, blowAttackRadius, whatIsPlayer);

        foreach (Collider2D target in hitEnemy)
        {
            Rigidbody2D rbTarget = target.GetComponent<Rigidbody2D>();

            target.GetComponent<IDamageable>().TakeDamage(attackDamage, DamageType.magical);
            Vector2 knockbackDirection = target.transform.position - transform.position;
            rbTarget.AddForce(knockbackDirection.normalized * knockbackForce);
        }
        creatureStats.currentHealth = 0;
        Instantiate(blowPartical, transform.position, blowPartical.transform.rotation);
        StartCoroutine(CameraManager.instance.CameraShake(0.2f, 2));
        SwitchState(State.Dead);
    }

    private void RangeAttack()
    {
        GameObject _projectTile = Instantiate(projectTile, attackPoint.position, attackPoint.rotation);
        _projectTile.GetComponent<ProjectTileBehavor>().FireProjectTile(travelDistanceProjectTile, attackDamage);
    }
    #endregion

    #region TakeDamage

    [Header("Damaged")]
    [SerializeField] private float takeDamageDuration = 0.2f;
    private float takeDamageTimeLeft;
    [SerializeField] private float knockbackDuration = 0.5f;


    private float knockbackTimeLeft = 0;
    private void EnterDamagedState()
    {
        anim.SetBool("Damaged", true);
        takeDamageTimeLeft = 0;
        knockbackTimeLeft = 0;
        movement.Set(0, rb.velocity.y);
        rb.velocity = movement;
    }
    private void UpdateDamagedState()
    {
        if (takeDamageTimeLeft >= takeDamageDuration)
        {
            if (knockbackTimeLeft >= knockbackDuration && creatureStats.currentHealth > 0 && isGroundDetected)
            {

                if (creatureStats.currentStunHealth <= 0)
                {

                    SwitchState(State.Stun);
                }
                else
                {
                    SwitchState(State.SerchPlayer);
                }
            }
            else if (creatureStats.currentHealth <= 0)
            {
                SwitchState(State.Dead);
            }
        }
        knockbackTimeLeft += Time.deltaTime;
        takeDamageTimeLeft += Time.deltaTime;
    }
    private void ExitDamagedState()
    {
        anim.SetBool("Damaged", false);
        movement.Set(0, rb.velocity.y);
        rb.velocity = movement;
    }
    #endregion



    #region Dead

    [Header("DeadParametrs")]
    [SerializeField] private bool isDead;
    private void EnterDeadState()
    {

        isDead = true;
        anim.SetBool("Dead", true);
        this.gameObject.layer = LayerMask.NameToLayer("Dead");
        
    }
    private void UpdateDeadState()
    {
        movement.Set(0, rb.velocity.y);
        rb.velocity = movement;
    }
    private void ExitDeadState()
    {
        this.gameObject.layer = LayerMask.NameToLayer("Enemy");
       /* creatureStats.healthBar.enabled = true;*/
        isDead = false;
    }
    #endregion
    private void Detecting()
    {
        isGroundDetected = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(wallCheckPoint.position, transform.right, wallCheckDistance, whatIsGround);
        isScaredPlayerDistance = Physics2D.Raycast(playerCheckPoint.position, transform.right, playerCheckInMinAgroDistance, whatIsPlayer);
        isDetectedPlayerDistance = Physics2D.Raycast(playerCheckPoint.position, transform.right, playerCheckInMaxAgroDistance, whatIsPlayer);
        isAttackPlayerDistance = Physics2D.Raycast(playerCheckPoint.position, transform.right, playerCheckInCloseRangeAction, whatIsPlayer);
        isDetectedPlayerInRadius = CheckPlayerInRadius();
    }



    private bool CheckPlayerInRadius()
    {
        Collider2D[] hitEnemy = Physics2D.OverlapCircleAll(transform.position, blowAttackRadius, whatIsPlayer);

        foreach (Collider2D target in hitEnemy)
        {
            if (target != null)
            {
                return true;
            }


        }
        return false;
    }


    private void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0, 180, 0);
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheckPoint.position, groundCheckPoint.position + (Vector3)Vector2.down * groundCheckDistance);
        Gizmos.DrawLine(wallCheckPoint.position, wallCheckPoint.position + (Vector3)Vector2.right * wallCheckDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(playerCheckPoint.position + (Vector3)Vector2.down * -0.1f, playerCheckPoint.position + (Vector3)Vector2.right * playerCheckInMinAgroDistance * facingDirection + (Vector3)Vector2.down * -0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(playerCheckPoint.position + (Vector3)Vector2.down * -0.2f, playerCheckPoint.position + (Vector3)Vector2.right * playerCheckInMaxAgroDistance * facingDirection + (Vector3)Vector2.down * -0.2f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(playerCheckPoint.position, playerCheckPoint.position + (Vector3)Vector2.right * playerCheckInCloseRangeAction * facingDirection);
        if (attackType == AttackType.melleAttack)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(attackPoint.position, melleAttackRadius);
        }
        else if (attackType == AttackType.rangeAttack)
        {

        }
        else if (attackType == AttackType.blow)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, blowAttackRadius);
        }

    }

    


}

public enum AttackType
{
    melleAttack,
    rangeAttack,
    blow
}
