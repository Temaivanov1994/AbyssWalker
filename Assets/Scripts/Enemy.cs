using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    private enum State
    {
        Idle,
        Walk,
        Stun,
        Charge,
        PlayerDetected,
        SerchPlayer,
        MelleAttack,
        Dead,
        Damaged,
    }

    private State currentState;


    [Header("IdleState")]
    [SerializeField] private float IdleMinTimeDuration = 1f;
    [SerializeField] private float IdleMaxTimeDuration = 3f;
    private float IdleStartTime;
    private float IdleDuration;

    [Header("WalkState")]
    [SerializeField] private float moveSpeed = 3f;
    private int facingDirection;
    private Vector2 movement;

    [Header("StunState")]
    [SerializeField] private float StunDuration = 5f;
    private float StunStartTime;

    [Header("ChargeState")]
    [SerializeField] private float chargeMoveSpeed = 7f;
    [SerializeField] private float ChargeDuration = 2f;
    private float ChargeStartTime;

    [Header("PlayerDetectedState")]
    [SerializeField] private float PlayerDetectedDuration = 0.2f;
    private float PlayerDetectedStartTime;
    private bool playerDetectedInMinAgroDistance;
    private bool playerDetectedInMaxAgroDistance;

    [Header("SerchPlayerState")]
    [SerializeField] private float timeBetweenTurns = 1f;
    private float lastTurnTime;
    [SerializeField] private int amountTurns = 3;
    private int amountTurnsDone;
    private bool isAllTurnsDone, isAllTunrsTimeDone, turnImmediately;
    [Header("MelleAttack")]
    [SerializeField] private Transform melleAttackPoint;
    [SerializeField] private float melleAttackRadius = 0.84f;
    [SerializeField] private int melleAttackDamage = 1;
    private bool playerDetectedInCloseRangeAction;
    private bool finishAttack;

    [Header("Damaged")]
    [SerializeField] private float damagedDuration = 0.2f;
    private float lastDamageTime;

    [Header("Loot")]
    [SerializeField] private GameObject Loot;
    private bool isLooted = false;
    [SerializeField] private Transform LootPoint;


    [Header("Audio")]
    [SerializeField] private AudioSource steps;
    [SerializeField] private AudioSource hurtSound;
    [SerializeField] private AudioSource detectSound;




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
    private bool groundDetected;
    private bool wallDetected;

    [SerializeField]
    private int maxHealth = 5;
    private int currentHealth;
    [SerializeField]
    private int stunMaxHealth = 3;
    private int stunCurrentHealth;



    [Space]
    [SerializeField]
    private GameObject hitPartical;
    private Rigidbody2D rb;
    private Animator anim;
   


    private void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        stunCurrentHealth = stunMaxHealth;
        facingDirection = 1;
        SwitchState(State.SerchPlayer);

    }
    private void Update()
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
            case State.Damaged:
                UpdateDamagedState();
                break;


        }
    }



    //IdleState________________________________________________________

    private void EnterIdleState()
    {
        anim.SetBool("Idle", true);
        IdleStartTime = Time.time;
        movement.Set(0, rb.velocity.y);
        rb.velocity = movement;
        IdleDuration = Random.Range(IdleMinTimeDuration, IdleMaxTimeDuration);
    }
    private void UpdateIdleState()
    {
        DoChecks();
        if (Time.time >= IdleStartTime + IdleDuration)
        {
            SwitchState(State.Walk);
        }



    }
    private void ExitIdleState()
    {
        anim.SetBool("Idle", false);
        Flip();
    }

    //WalkState_________________________________________________________

    private void EnterWalkState()
    {
        anim.SetBool("Walk", true);
    }
    private void UpdateWalkState()
    {
        DoChecks();

        if (!groundDetected || wallDetected)
        {
            SwitchState(State.Idle);
        }
        else if (playerDetectedInMaxAgroDistance)
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
    // StunState_________________________________________________________

    private void EnterStunState()
    {
        StunStartTime = Time.time;
        movement.Set(0, rb.velocity.y);
        rb.velocity = movement;
        anim.SetBool("Stun", true);
        stunCurrentHealth = stunMaxHealth;
    }
    private void UpdateStunState()
    {
        if (Time.time >= StunStartTime + StunDuration)
        {
            if (playerDetectedInCloseRangeAction)
            {
                SwitchState(State.PlayerDetected);
            }
            else
            {
                SwitchState(State.SerchPlayer);

            }
        }


    }
    private void ExitStunState()
    {
        anim.SetBool("Stun", false);
    }
    //ChargeState________________________________________________________
    private void EnterChargeState()
    {
        anim.SetBool("Charge", true);
        ChargeStartTime = Time.time;
    }
    private void UpdateChargeState()
    {
        DoChecks();

        if (Time.time >= ChargeStartTime + ChargeDuration || !groundDetected || wallDetected)
        {

            SwitchState(State.SerchPlayer);
        }
        else
        {
            movement.Set(chargeMoveSpeed * facingDirection, rb.velocity.y);
            rb.velocity = movement;
            if (playerDetectedInCloseRangeAction)
            {
                SwitchState(State.MelleAttack);
            }
        }
    }
    private void ExitChargeState()
    {
        anim.SetBool("Charge", false);
    }

    //PlayerDetected________________________________________________
    private void EnterPlayerDetectedState()
    {
        anim.SetBool("PlayerDetected", true);
        detectSound.Play();
        movement.Set(0, rb.velocity.y);
        rb.velocity = movement;
        PlayerDetectedStartTime = Time.time;
    }
    private void UpdatePlayerDetectedState()
    {
        if (Time.time >= PlayerDetectedStartTime + PlayerDetectedDuration)
        {
            SwitchState(State.Charge);
        }
        else if (playerDetectedInCloseRangeAction)
        {
            SwitchState(State.MelleAttack);
        }
    }
    private void ExitPlayerDetectedState()
    {
        anim.SetBool("PlayerDetected", false);
    }
    //SerchPlayer___________________________________________________
    private void EnterSerchPlayerState()
    {
        anim.SetBool("Idle", true);
        movement.Set(0, rb.velocity.y);
        rb.velocity = movement;
        amountTurns = Random.Range(2, 5);
        amountTurnsDone = 0;
        isAllTurnsDone = false;
        isAllTunrsTimeDone = false;
    }
    private void UpdateSerchPlayerState()
    {

        if (turnImmediately)
        {
            Flip();
            lastTurnTime = Time.time;
            amountTurnsDone++;
            turnImmediately = false;

        }
        else if (Time.time >= lastTurnTime + timeBetweenTurns && !isAllTurnsDone)
        {

            Flip();
            DoChecks();
            lastTurnTime = Time.time;
            amountTurnsDone++;
        }

        if (amountTurnsDone >= amountTurns)
        {
            isAllTurnsDone = true;
        }

        if (Time.time >= lastTurnTime + timeBetweenTurns && isAllTurnsDone)
        {
            isAllTunrsTimeDone = true;
        }

        if (playerDetectedInMaxAgroDistance && groundDetected)
        {
            SwitchState(State.PlayerDetected);
        }
        else if (isAllTunrsTimeDone)
        {
            SwitchState(State.Walk);
        }

    }
    private void ExitSerchPlayerState()
    {
        anim.SetBool("Idle", false);
    }
    //MelleAttack________________________________________________________
    private void EnterMelleAttackState()
    {
        anim.SetBool("MelleAttack", true);
        movement.Set(0, rb.velocity.y);
       
        rb.velocity = movement;
        finishAttack = false;


    }
    private void UpdateMelleAttackState()
    {
        DoChecks();
        if (finishAttack)
        {
            if (playerDetectedInCloseRangeAction)
            {
                SwitchState(State.MelleAttack);
            }
            else
            {
                SwitchState(State.SerchPlayer);
            }
        }
    }
    private void ExitMelleAttackState()
    {
        anim.SetBool("MelleAttack", false);
    }

    //DeadState____________________________________________________
    private void EnterDeadState()
    {
        anim.SetBool("Dead", true);
        movement.Set(0, rb.velocity.y);
        rb.velocity = movement;
        gameObject.layer = 12;
    }
    private void UpdateDeadState()
    {

    }
    private void ExitDeadState()
    {

    }

    //DamagedState__________________________________________
    private void EnterDamagedState()
    {

        anim.SetBool("Damaged", true);
        lastDamageTime = Time.time;
        movement.Set(0, rb.velocity.y);
        rb.velocity = movement;
    }
    private void UpdateDamagedState()
    {
        if (Time.time >= lastDamageTime + damagedDuration && currentHealth > 0)
        {
            if (stunCurrentHealth <= 0)
            {

                SwitchState(State.Stun);
            }
            else
            {
                SwitchState(State.SerchPlayer);
            }
        }
        else if (currentHealth <= 0)
        {
            SwitchState(State.Dead);
        }

    }
    private void ExitDamagedState()
    {
        anim.SetBool("Damaged", false);
    }


    //AudioSources__________________________________________________
    public void StepsSound()
    {
        steps.Play();
    }

    //OtherFunctions________________________________________________


    private void DoChecks()
    {
        groundDetected = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(wallCheckPoint.position, transform.right, wallCheckDistance, whatIsGround);
        playerDetectedInMinAgroDistance = Physics2D.Raycast(playerCheckPoint.position, transform.right, playerCheckInMinAgroDistance, whatIsPlayer);
        playerDetectedInMaxAgroDistance = Physics2D.Raycast(playerCheckPoint.position, transform.right, playerCheckInMaxAgroDistance, whatIsPlayer);
        playerDetectedInCloseRangeAction = Physics2D.Raycast(playerCheckPoint.position, transform.right, playerCheckInCloseRangeAction, whatIsPlayer);

    }
    public void FinishAttack()
    {
        finishAttack = true;
    }
   

    private void GetLoot()
    {

        isLooted = true;
        Instantiate(Loot, LootPoint.position, Quaternion.identity);
    }

   
    private void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0, 180, 0);
    }
    private void SwitchState(State state)
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
            case State.Damaged:
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
            case State.Damaged:
                EnterDamagedState();
                break;
        }
        currentState = state;
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
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(melleAttackPoint.position, melleAttackRadius);

    }
}


