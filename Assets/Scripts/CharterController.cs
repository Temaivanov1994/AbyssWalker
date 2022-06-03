using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Rigidbody2D))]
public class CharterController : MonoBehaviour
{
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        healthBar = GetComponentInChildren<HealthBar>();
        staminaBar = GetComponentInChildren<StaminaBar>();
    }
    private void Start()
    {
        InitHealthBar();
        InitStaminaBar();
        bloodPartical = Resources.Load<ParticleSystem>("Charters/ParticalSystems and effects/Particle System Blood");
        healPartical = Resources.Load<ParticleSystem>("Charters/ParticalSystems and effects/Particle System Heal");
        OnLandingParticleSystem = Resources.Load<ParticleSystem>("Charters/ParticalSystems and effects/OnLanding");

        SwitchState(State.idle);
    }
    private void Update()
    {

        UpdateStateMachine();


        if (movementDirection > 0 && !facingRight)
        {
            Flip();
        }
        else if (movementDirection < 0 && facingRight)
        {
            Flip();
        }

        if (!isDetectingGround && !isJump && !isAttack && !isKnockback && !isDead)
        {
            SwitchState(State.jump);
        }

        #region KeyboardControl

        if (!isKnockback && !isDead)
        {
            movementDirection = Input.GetAxisRaw("Horizontal");

        }

        if (Input.GetButtonDown("Jump") && isDetectingGround)
        {
            if (canJump)
            {
                rb.AddForce(new Vector2(0f, jumpForce));
                SwitchState(State.jump);
            }
        }



        if (Input.GetMouseButtonDown(0) && !isAttack && !isDead)
        {
            if (isDetectingGround && currentStamina >= groundAttackStaminaCost)
            {
                currentStamina -= groundAttackStaminaCost;
                staminaBar.SetStamina(currentStamina);
                SwitchState(State.groundAttack);
            }
            else if (!isDetectingGround && currentStamina >= flyAttackStaminaCost)
            {
                currentStamina -= flyAttackStaminaCost;
                staminaBar.SetStamina(currentStamina);
                SwitchState(State.flyAttack);
            }

        }
        if (Input.GetMouseButtonDown(1) && !isDefense && !isDead)
        {
            if (isDetectingGround && currentStamina >= defenseStaminaCost)
            {
                currentStamina -= defenseStaminaCost;
                staminaBar.SetStamina(currentStamina);
                SwitchState(State.defense);

            }
        }
        #endregion

        if (regenerationStaminaTimeLeft >= regenerationStaminaDuration)
        {

            if (!isAttack && !isDefense)
            {
                if (currentStamina <= maxStamina)
                {
                    currentStamina += 1;
                    staminaBar.SetStamina(currentStamina);

                }

            }
            else
            {
                regenerationStaminaTimeLeft = 0;
            }

        }
        else
        {
            regenerationStaminaTimeLeft += Time.deltaTime;
        }

        Detecting();
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            Vector3 targetVelocity = new Vector2(movementDirection * movementSpeed * Time.fixedDeltaTime, rb.velocity.y);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref zeroVelocity, movementSmoothing);

        }
    }




    #region Stats

    [Header("PlayerStats")]

    [SerializeField] private int maxHealth;
    private int currentHealth;
    [SerializeField] private int maxStamina;
    private float currentStamina;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private StaminaBar staminaBar;
    [SerializeField] private float groundAttackStaminaCost = 30;
    [SerializeField] private float flyAttackStaminaCost = 15;
    [SerializeField] private float defenseStaminaCost = 10;

    [Header("PlayerBattleStats")]
    [SerializeField] private int groundAttackDamage;
    [SerializeField] private int flyAttackDamage;
    [SerializeField] private float knockbackForce = 200;
    [SerializeField] private float regenerationStaminaDuration = 1f;
    private float regenerationStaminaTimeLeft = 0;

    private void InitHealthBar()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth, currentHealth);
        healthBar.SetHealth(currentHealth);
    }
    private void InitStaminaBar()
    {
        currentStamina = maxStamina;
        staminaBar.SetMaxStamina(maxStamina, currentStamina);
        staminaBar.SetStamina(currentStamina);
    }

    #endregion

    #region StateMachine

    [SerializeField] private State currentState;
    private void UpdateStateMachine()
    {
        switch (currentState)
        {

            case State.idle:
                UpdateIdleState();
                break;
            case State.move:
                UpdateMoveState();
                break;
            case State.jump:
                UpdateJumpState();
                break;
            case State.defense:
                UpdateDefenseBattleState();
                break;
            case State.drinkEstus:
                UpdateEstusState();
                break;
            case State.flyAttack:
                UpdateFlyAttackBattleState();
                break;
            case State.groundAttack:
                UpdateGroundAttackState();
                break;
            case State.knockBack:
                UpdateKnockBackState();
                break;
            case State.dead:
                UpdateDeadState();
                break;
        }
    }
    private void SwitchState(State state)
    {
        switch (currentState)
        {
            case State.idle:
                ExitIdleState();
                break;
            case State.move:
                ExitMoveState();
                break;
            case State.jump:
                ExitJumpState();
                break;
            case State.defense:
                ExitDefenseBattleState();
                break;
            case State.drinkEstus:
                ExitEstusState();
                break;
            case State.flyAttack:
                ExitFlyAttackBattleState();
                break;
            case State.groundAttack:
                ExitGroundAttackState();
                break;
            case State.knockBack:
                ExitKnockBackState();
                break;
            case State.dead:
                ExitDeadState();
                break;
        }
        switch (state)
        {
            case State.idle:
                EnterIdleState();
                break;
            case State.move:
                EnterMoveState();
                break;
            case State.jump:
                EnterJumpState();
                break;
            case State.defense:
                EnterDefenseBattleState();
                break;
            case State.drinkEstus:
                EnterEstusState();
                break;
            case State.flyAttack:
                EnterFlyAttackBattleState();
                break;
            case State.groundAttack:
                EnterGroundAttackState();
                break;
            case State.knockBack:
                EnterKnockBackState();
                break;
            case State.dead:
                EnterDeadState();
                break;
        }
        currentState = state;
    }


    private enum State
    {
        idle,
        move,
        jump,
        defense,
        drinkEstus,
        flyAttack,
        groundAttack,
        knockBack,
        dead
    }

    #endregion

    #region CheckParametrs and Metods and Components


    [Header("Components")]
    private Rigidbody2D rb;
    private Animator anim;

    [Header("GroundCheck Parametrs")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRaduis = 0.18f;
    [SerializeField] private LayerMask whatIsGround;
    private bool isDetectingGround;
    [Header("WallCheck Parametrs")]
    [SerializeField] private Transform wallCheckPoint;
    [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private LayerMask whatIsWall;
    private bool isDetectingWall;
    [Header("LedgeCheck Parametrs")]
    [SerializeField] private Transform ledgeCheckPoint;
    [SerializeField] private float ledgeCheckDistance = 0.5f;
    private bool isDetectingLedge;

    [Header("EnemyCheck Paramerts")]
    [SerializeField] private Transform enemyCheckPoint;
    [SerializeField] private float enemyCheckDistance = 10f;
    [SerializeField] private bool isDetectingEnemy;
    [SerializeField] private LayerMask whatIsEnemy;




    private void Detecting()
    {
        isDetectingGround = CheckGround();
        isDetectingWall = Physics2D.Raycast(wallCheckPoint.position, transform.right, wallCheckDistance, whatIsGround);
        isDetectingLedge = Checkledge();
        isDetectingEnemy = CheckEnemy();
    }


    private bool CheckEnemy()
    {

        if (Physics2D.Raycast(enemyCheckPoint.position, transform.right, enemyCheckDistance, whatIsEnemy))
        {
            return true;
        }

        return false;
    }
    private bool CheckGround()
    {
        bool wasGrounded = isDetectingGround;
        isDetectingGround = false;


        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckPoint.position, groundCheckRaduis, whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                isDetectingGround = true;
                if (!wasGrounded)
                {
                    OnLandEvent.Invoke();

                }
                return true;
            }
        }
        return false;
    }
    private bool Checkledge()
    {
        if (!Physics2D.Raycast(ledgeCheckPoint.position, Vector2.down, ledgeCheckDistance, whatIsGround) && isDetectingGround)
        {
            return true;
        }
        return false;
    }


    private int FacingRightToInt()
    {
        if (facingRight)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }
    #endregion

    /* #region TouchParametrs and Metods

     [Header("TouchParametrs")]
     [SerializeField] private float touchTimeForDefense = 0.2f;
     [SerializeField] private float touchTimeForRoll = 0.2f;
     [SerializeField] private float touchTimeForLightAttack = 0.4f;
     [SerializeField] private float touchTimeForHardAttack = 0.6f;
     [SerializeField] private float touchTimeForFailPreparing = 1f;
     private float touchDuration;


     private Vector2 swipeDirection(Vector2 startPos, Vector2 endPos)
     {
         var finalDelta = endPos - startPos;
         if (finalDelta.x < -Mathf.Abs(finalDelta.y)) finalDelta = -Vector2.right;
         if (finalDelta.x > Mathf.Abs(finalDelta.y)) finalDelta = Vector2.right;

         return finalDelta;
     }

     #endregion*/

    #region IdleState

    private void EnterIdleState()
    {

        anim.SetBool("Idle", true);
    }
    private void UpdateIdleState()
    {
        if (canMove && !isKnockback)
        {
            if (movementDirection != 0)
            {
                SwitchState(State.move);
            }
        }

        if (Input.GetKeyDown(KeyCode.H) && currentHealth != maxHealth)
        {
            SwitchState(State.drinkEstus);
        }
    }
    private void ExitIdleState()
    {
        anim.SetBool("Idle", false);
    }

    #endregion

    #region MoveState


    [Header("Movement Parametrs")]
    [SerializeField] private float movementSpeed = 400f;
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;
    private bool facingRight = true;
    private bool canMove = true;
    private bool isMoving;
    private float movementDirection;
    private Vector3 zeroVelocity = Vector3.zero;
    private void EnterMoveState()
    {
        anim.SetBool("Move", true);

        isMoving = true;
    }
    private void UpdateMoveState()
    {
        if (movementDirection == 0)
        {
            SwitchState(State.idle);

        }
    }
    private void ExitMoveState()
    {
        isMoving = false;
        anim.SetBool("Move", false);
    }

    private void Flip()
    {
        if (!isAttack)
        {
            facingRight = !facingRight;

            transform.Rotate(0, 180, 0);
        }
    }

    #endregion

    #region JumpState

    [Header("Jump Parametrs")]
    [SerializeField] private float jumpForce = 750f;
    [Range(0.1f, 0.9f)] [SerializeField] private float variableJumpHeightMultiplier;
    public UnityEvent OnLandEvent;
    private ParticleSystem OnLandingParticleSystem;
    private bool canJump = true;
    private bool isJump = false;
    [SerializeField] private bool jumpButton = false;
    private bool checkMultuplierJump;
    private void EnterJumpState()
    {

        checkMultuplierJump = true;
        isJump = true;
        anim.SetBool("Jump", true);


    }
    private void UpdateJumpState()
    {
        anim.SetFloat("VelocityY", rb.velocity.y);

        if (isDetectingGround && isJump && !isDead)
        {
            SwitchState(State.idle);
        }
        if (checkMultuplierJump && /*(!jumpButton ||*/ !Input.GetButton("Jump"))
        {
            checkMultuplierJump = false;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
        }
    }
    private void ExitJumpState()
    {
        anim.SetBool("Jump", false);
        isJump = false;
    }


    public void OnlandingEvent()
    {
       
        Instantiate(OnLandingParticleSystem, transform.position, OnLandingParticleSystem.transform.rotation);
    }
    #endregion

    #region DefenseBattleState

    [Header("Defense Parametrs")]
    [SerializeField] private float defenseDuration = 1f;
    private bool isDefense = false;
    private float defenseTimeLeft;
    private void EnterDefenseBattleState()
    {
        canMove = false;
        canJump = false;
        rb.velocity = Vector2.zero;
        anim.SetBool("DefenseBattle", true);
        isDefense = true;
        defenseTimeLeft = 0;


    }
    private void UpdateDefenseBattleState()
    {
        defenseTimeLeft += Time.deltaTime;
        if (defenseTimeLeft >= defenseDuration)
        {
            SwitchState(State.idle);
        }
    }
    private void ExitDefenseBattleState()
    {
        canMove = true;
        canJump = true;
        isDefense = false;
        anim.SetBool("DefenseBattle", false);
    }

    #endregion

    #region FlyAttackBattleState

    [Header("Attack Parametrs")]
    [SerializeField] private float groundAttackDuration = 0.5f;
    [SerializeField] private float attackSpeed = 5;
    [SerializeField] private float flyAttackDuration = 0.3f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRaduis = 0.3f;

    private bool isAttack = false;
    private float attackTimeLeft = 0;

    private void EnterFlyAttackBattleState()
    {

        isAttack = true;
        attackTimeLeft = 0;
        anim.SetBool("LightAttack", true);


    }
    private void UpdateFlyAttackBattleState()
    {
        if (attackTimeLeft <= flyAttackDuration)
        {
            attackTimeLeft += Time.deltaTime;
        }

        if (attackTimeLeft >= flyAttackDuration || isDetectingWall || isDetectingLedge)
        {

            SwitchState(State.jump);
        }
    }
    private void ExitFlyAttackBattleState()
    {
        anim.SetBool("LightAttack", false);
        isAttack = false;
    }



    #endregion

    #region GroundAttackBattleState

    [SerializeField] private float comboAttack = 0;
    private void EnterGroundAttackState()
    {
        canMove = false;
        canJump = false;
        isAttack = true;
        attackTimeLeft = 0;
        comboAttack += 1;
        if (comboAttack == 4)
        {
            comboAttack = 1;
        }

        anim.SetFloat("ComboAttack", comboAttack);
        anim.SetBool("HardAttack", true);

    }
    private void UpdateGroundAttackState()
    {

        if (attackTimeLeft <= groundAttackDuration)
        {
            if (isDetectingEnemy || isDetectingWall || isDetectingLedge)
            {
                rb.velocity = Vector2.zero;
            }
            else
            {
                rb.velocity = new Vector2(attackSpeed * FacingRightToInt(), 0);
            }
            attackTimeLeft += Time.deltaTime;


        }

        if (attackTimeLeft >= groundAttackDuration)
        {
            rb.velocity = Vector2.zero;
            SwitchState(State.idle);
        }

    }
    private void ExitGroundAttackState()
    {
        anim.SetBool("HardAttack", false);
        canMove = true;
        canJump = true;
        isAttack = false;
    }

    public void Attack()
    {
        Collider2D[] hitEnemy = Physics2D.OverlapCircleAll(attackPoint.position, attackRaduis, whatIsEnemy);

        foreach (Collider2D enemy in hitEnemy)
        {
            Rigidbody2D rbTarget = enemy.GetComponent<Rigidbody2D>();
            if (isDetectingGround)
            {

                if (enemy.CompareTag("ProjectTile"))
                {
                    enemy.GetComponent<IDamageable>().TakeDamage(groundAttackDamage, DamageType.physical);
                    Vector2 knockbackDirection = new Vector2(rbTarget.transform.position.x - transform.position.x, 0.5f);
                    rbTarget.AddForce(knockbackDirection.normalized * knockbackForce * 3);
                }
                else if (enemy.CompareTag("Enemy"))
                {
                    enemy.GetComponent<IDamageable>().TakeDamage(groundAttackDamage, DamageType.physical);
                    Vector2 knockbackDirection = new Vector2(rbTarget.transform.position.x - transform.position.x, 1f);
                    rbTarget.AddForce(knockbackDirection.normalized * knockbackForce);
                }


            }
            else
            {
                enemy.GetComponent<IDamageable>().TakeDamage(flyAttackDamage, DamageType.physical);
                Vector2 knockbackDirection = new Vector2(rbTarget.transform.position.x - transform.position.x, 1f);
                rbTarget.AddForce(knockbackDirection.normalized * knockbackForce);
            }

        }
    }

    #endregion

    #region Knockback

    [Header("KnockBack Parametrs")]
    [SerializeField] private float knockbackDuration = 0.5f;
    private float knockbackTimeLeft = 0;
    private bool isKnockback = false;
    [SerializeField] private ParticleSystem bloodPartical;

    private void EnterKnockBackState()
    {
        canMove = false;
        canJump = false;
        isKnockback = true;
        knockbackTimeLeft = 0;
        anim.SetBool("TakeDamage", true);


    }
    private void UpdateKnockBackState()
    {

        if (knockbackTimeLeft >= knockbackDuration && isDetectingGround)
        {
            if (currentHealth <= 0)
            {
                SwitchState(State.dead);
                rb.velocity = Vector2.zero;

            }
            else if (currentHealth > 0)
            {
                SwitchState(State.idle);
                rb.velocity = Vector2.zero;

            }
        }
        knockbackTimeLeft += Time.deltaTime;

    }
    private void ExitKnockBackState()
    {
        anim.SetBool("TakeDamage", false);
        canMove = true;
        canJump = true;
        isKnockback = false;
    }

    #endregion

    #region Dead

    [Header("Dead Parametrs")]

    private bool isDead = false;

    private void EnterDeadState()
    {
        canMove = false;
        canJump = false;
        isDead = true;
        anim.SetBool("Dead", true);
        this.gameObject.layer = 11;
      

    }
    private void UpdateDeadState()
    {

    }
    private void ExitDeadState()
    {
        anim.SetBool("Dead", false);
        canMove = true;
        canJump = true;
        isDead = false;
        this.gameObject.layer = 8;
    }

    #endregion

    #region DrinkEstus

    [Header("Estus Parametrs")]
    [SerializeField] private ParticleSystem healPartical;
    [SerializeField] private float drinkEstusDuration = 3f;
    [SerializeField] private int healingPower = 3;
    private bool successDrink = false;
    private float drinkEstusTimeLeft;
    private bool isDrinking = false;

    private void EnterEstusState()
    {
        drinkEstusTimeLeft = 0;
        successDrink = false;
        canMove = false;
        canJump = false;
        isDrinking = true;

        anim.SetBool("Estus", true);


    }
    private void UpdateEstusState()
    {

        if (!isDead)
        {
            if (drinkEstusTimeLeft >= defenseDuration)
            {
                successDrink = true;
                SwitchState(State.idle);
            }
        }
        drinkEstusTimeLeft += Time.deltaTime;

    }
    private void ExitEstusState()
    {

        anim.SetBool("Estus", false);
        if (successDrink)
        {
            TakeDamage(healingPower, false);
        }
        canMove = true;
        canJump = true;
        successDrink = false;
        isDrinking = false;

    }

    #endregion


    #region AudioParametrs and Metods
    [Header("Audio")]
    [SerializeField] private AudioSource playerSource;
    [SerializeField] private AudioClip someSoundTest;
    [SerializeField] private AudioClip soundDefense;
    [SerializeField] private AudioClip soundReadyLightAttack;
    [SerializeField] private AudioClip soundReadyHardAttack;
    [SerializeField] private AudioClip soundFailPreparing;



    #endregion


    #region Screen Buttons

    public void MovementButtons(int direction)
    {
        if (!isDead)
        {
            movementDirection = direction;

        }

    }

    public void JumpButton(bool _jumpButton)
    {
        jumpButton = _jumpButton;

        if (isDetectingGround && _jumpButton == true)
        {
            if (canJump)
            {
                rb.AddForce(new Vector2(0f, jumpForce));
                SwitchState(State.jump);
            }
        }
    }

    public void AttackButton()
    {

        if (!isAttack && !isDead)
        {
            if (isDetectingGround && currentStamina >= groundAttackStaminaCost)
            {
                currentStamina -= groundAttackStaminaCost;
                staminaBar.SetStamina(currentStamina);
                SwitchState(State.groundAttack);
            }
            else if (!isDetectingGround && currentStamina >= flyAttackStaminaCost)
            {
                currentStamina -= flyAttackStaminaCost;
                staminaBar.SetStamina(currentStamina);
                SwitchState(State.flyAttack);
            }

        }
    }
    public void DefenceButton()
    {

        if (!isDefense && !isDead)
        {
            if (isDetectingGround && currentStamina >= defenseStaminaCost)
            {
                currentStamina -= defenseStaminaCost;
                staminaBar.SetStamina(currentStamina);
                SwitchState(State.defense);

            }
        }



    }

    #endregion
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRaduis);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(wallCheckPoint.position, wallCheckPoint.position + Vector3.right * wallCheckDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(ledgeCheckPoint.position, ledgeCheckPoint.position + Vector3.down * ledgeCheckDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(enemyCheckPoint.position, enemyCheckPoint.position + Vector3.right * enemyCheckDistance);
        Gizmos.DrawWireSphere(attackPoint.position, attackRaduis);
    }

    public void TakeDamage(int damage, bool isDamage)
    {
        if (isDamage)
        {

            if (!isKnockback && !isDefense && !isDead)
            {
                currentHealth -= damage;
                Instantiate(bloodPartical, transform.position, bloodPartical.transform.rotation);
                SwitchState(State.knockBack);

            }
            if (isDefense)
            {
                SwitchState(State.groundAttack);
            }

        }
        else
        {
            currentHealth += damage;
            currentHealth = Mathf.Clamp(currentHealth, -1, maxHealth);
            Instantiate(healPartical, transform.position, bloodPartical.transform.rotation);
           
        }
        healthBar.SetHealth(currentHealth);

    }



}
