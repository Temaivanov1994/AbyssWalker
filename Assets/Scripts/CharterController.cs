using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class CharterController : MonoBehaviour, IDamageable
{
    [SerializeField] private State currentState;
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;

    [Header("GroundCheck Parametrs")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRaduis = 0.18f;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private bool isDetectingGround;
    [Header("WallCheck Parametrs")]
    [SerializeField] private Transform wallCheckPoint;
    [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private bool isDetectingWall;
    [Header("LedgeCheck Parametrs")]
    [SerializeField] private Transform ledgeCheckPoint;
    [SerializeField] private float ledgeCheckDistance = 0.5f;
    [SerializeField] private bool isDetectingLedge;
    [Header("Movement Parametrs")]
    [SerializeField] private float movementDirection;
    [SerializeField] private float movementSpeed = 400f;
    [SerializeField] private bool facingRight = true;
    [SerializeField] private bool canMove = true;
    [SerializeField] private bool isMoving;
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;
    [SerializeField] private Vector3 zeroVelocity = Vector3.zero;
    [Header("Jump Parametrs")]
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool isJump = false;
    [SerializeField] private float jumpForce = 750f;
    [SerializeField] private bool checkMultuplierJump;
    [Range(0.1f, 0.9f)] [SerializeField] private float variableJumpHeightMultiplier;
    public UnityEvent OnLandEvent;

    [Header("PrepareBattle Parametrs")]
    [SerializeField] private float preparingDuration;
    [SerializeField] private float preparingTimeLeft;

    [Header("Battle Parametrs")]
    [SerializeField] private bool isReadyForBattle = false;
    [SerializeField] private bool isInBattle = false;
    [SerializeField] private float timeForPreparingBattle;
    [SerializeField] private Animator animWeapon;
    [SerializeField] private GameObject weaponPoint;
    [SerializeField] private float rollTime;
    [SerializeField] private Vector2 startswipeDirection;
    [SerializeField] private Vector2 endSwipeDirection;
    [SerializeField] private Vector2 totalDirection;

    [Header("TouchParametrs")]
    [SerializeField] private float touchDuration;
    [SerializeField] private float touchTimeForDefense = 0.2f;
    [SerializeField] private float touchTimeForRoll = 0.2f;
    [SerializeField] private float touchTimeForLightAttack = 0.4f;
    [SerializeField] private float touchTimeForHardAttack = 0.6f;
    [SerializeField] private float touchTimeForFailPreparing = 1f;

    [Header("Defense Parametrs")]
    [SerializeField] private bool isDefense = false;
    [SerializeField] private float defenseDuration = 1f;
    [SerializeField] private float defenseTimeLeft;

    [Header("BattleRoll Parametrs")]
    [SerializeField] private bool isRoll = false;
    [SerializeField] private float rollDuration;
    [SerializeField] private float rollSpeed;
    [SerializeField] private float rollTimeLeft = 0;

    [Header("Attack Parametrs")]
    [SerializeField] private bool isAttack = false;
    [SerializeField] private float attackDuration;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float attackTimeLeft = 0;



    [Header("Audio")]
    [SerializeField] private AudioSource playerSource;
    [SerializeField] private AudioClip someSoundTest;
    [SerializeField] private AudioClip soundDefense;
    [SerializeField] private AudioClip soundReadyLightAttack;
    [SerializeField] private AudioClip soundReadyHardAttack;
    [SerializeField] private AudioClip soundFailPreparing;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        animWeapon = weaponPoint.GetComponent<Animator>();

    }
    private void Start()
    {
        SwitchState(State.idle);
    }
    private void Update()
    {
        UpdateStateMachine();

        movementDirection = Input.GetAxisRaw("Horizontal");





        

            if (movementDirection > 0  && !facingRight || totalDirection == Vector2.right && !facingRight)
            {
                Flip();
            }
            else if (movementDirection < 0 && facingRight || totalDirection == Vector2.left && facingRight)
            {
                Flip();
            }
       


        if (!isDetectingGround && !isJump && !isInBattle)
        {
            SwitchState(State.jump);
        }



        if (Input.GetButtonDown("Jump") && isDetectingGround)
        {
            if (canJump)
            {
                rb.AddForce(new Vector2(0f, jumpForce));
                SwitchState(State.jump);
            }
        }

        if (Input.GetKeyDown(KeyCode.B) && !isInBattle)
        {
            SwitchState(State.prepareBattle);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            SwitchState(State.stopBattle);
            isInBattle = false;
        }



        EnviromentDetecting();
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            Vector3 targetVelocity = new Vector2(movementDirection * movementSpeed * Time.fixedDeltaTime, rb.velocity.y);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref zeroVelocity, movementSmoothing);

        }
    }

    private void EnviromentDetecting()
    {
        isDetectingGround = CheckGround();
        isDetectingWall = Physics2D.Raycast(wallCheckPoint.position, transform.right, wallCheckDistance, whatIsGround);
        isDetectingLedge = Checkledge();
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
    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }


    #region StateMachine

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
            case State.prepareBattle:
                UpdateBattlePrepareState();
                break;
            case State.battle:
                UpdateBattleState();
                break;
            case State.defense:
                UpdateDefenseBattleState();
                break;
            case State.roll:
                UpdateRollBattleState();
                break;
            case State.lightAttack:
                UpdateLightAttackBattleState();
                break;
            case State.hardAttack:
                UpdateHardAttackBattleState();
                break;
            case State.stopBattle:
                UpdateStopBattleState();
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
            case State.prepareBattle:
                ExitBattlePrepareState();
                break;
            case State.battle:
                ExitBattleState();
                break;
            case State.defense:
                ExitDefenseBattleState();
                break;
            case State.roll:
                ExitRollBattleState();
                break;
            case State.lightAttack:
                ExitLightAttackBattleState();
                break;
            case State.hardAttack:
                ExitHardAttackBattleState();
                break;
            case State.stopBattle:
                ExitStopBattleState();
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
            case State.prepareBattle:
                EnterBattlePrepareState();
                break;
            case State.battle:
                EnterBattleState();
                break;
            case State.defense:
                EnterDefenseBattleState();
                break;
            case State.roll:
                EnterRollBattleState();
                break;
            case State.lightAttack:
                EnterLightAttackBattleState();
                break;
            case State.hardAttack:
                EnterHardAttackBattleState();
                break;
            case State.stopBattle:
                EnterStopBattleState();
                break;
        }
        currentState = state;
    }


    private enum State
    {
        idle,
        move,
        jump,
        prepareBattle,
        battle,
        defense,
        roll,
        lightAttack,
        hardAttack,
        stopBattle,
    }

    #endregion

    #region IdleState

    private void EnterIdleState()
    {

        anim.SetBool("Idle", true);
    }
    private void UpdateIdleState()
    {
        if (canMove)
        {
            if (movementDirection != 0)
            {
                SwitchState(State.move);
            }
        }

    }
    private void ExitIdleState()
    {
        anim.SetBool("Idle", false);
    }

    #endregion

    #region MoveState

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

    #endregion

    #region JumpState

    private void EnterJumpState()
    {
        checkMultuplierJump = true;
        isJump = true;
        anim.SetBool("Jump", true);


    }
    private void UpdateJumpState()
    {
        anim.SetFloat("VelocityY", rb.velocity.y);

        if (isDetectingGround && !isJump)
        {
            SwitchState(State.idle);
        }
        if (checkMultuplierJump && !Input.GetButton("Jump"))
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
        SwitchState(State.idle);
        // TODO: Create landing effect
    }
    #endregion

    #region PrepareBattleState

    private void EnterBattlePrepareState()
    {
        preparingDuration = anim.GetCurrentAnimatorStateInfo(0).length;
        rb.velocity = Vector2.zero;
        isInBattle = true;
        preparingTimeLeft = 0;
        canJump = false;
        canMove = false;
        isReadyForBattle = false;
        anim.SetBool("PrepareBattle", true);

    }
    private void UpdateBattlePrepareState()
    {
        preparingTimeLeft += Time.deltaTime;
        if (preparingTimeLeft >= preparingDuration)
        {
            SwitchState(State.battle);
        }

    }
    private void ExitBattlePrepareState()
    {
        isReadyForBattle = false;
        isInBattle = true;
        anim.SetBool("PrepareBattle", false);
    }


    #endregion

    #region BattleState

    private void EnterBattleState()
    {
        anim.SetBool("BattleIdle", true);

    }
    private void UpdateBattleState()
    {

        if (Input.GetMouseButtonDown(0))
        {
            animWeapon.SetBool("Preparing", true);
            startswipeDirection = Vector2.zero;
            startswipeDirection = Camera.main.ViewportToWorldPoint(Input.mousePosition).normalized;


            touchDuration = 0;

        }


        if (Input.GetMouseButton(0))
        {
            touchDuration += Time.deltaTime;

            if (touchDuration == touchTimeForDefense)
            {
                playerSource.PlayOneShot(someSoundTest);
            }
            else if (touchDuration == touchTimeForLightAttack)
            {
                playerSource.PlayOneShot(soundReadyLightAttack);
            }
            else if (touchDuration == touchTimeForHardAttack)
            {
                playerSource.PlayOneShot(soundReadyHardAttack);
            }
            else if (touchDuration >= touchTimeForFailPreparing)
            {
                touchDuration = 0;
                SwitchState(State.prepareBattle);

            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log(touchDuration);

            animWeapon.SetBool("Preparing", false);
            endSwipeDirection = Camera.main.ViewportToWorldPoint(Input.mousePosition).normalized;


            totalDirection = Vector2.zero;
            totalDirection = swipeDirection(startswipeDirection, endSwipeDirection);
            Debug.Log("TotalVector" + totalDirection);



            if (touchDuration <= touchTimeForDefense && totalDirection == Vector2.zero)
            {
                Debug.Log("Defense");
                SwitchState(State.defense);

            }
            else if (touchDuration <= touchTimeForRoll && totalDirection != Vector2.zero)
            {
                SwitchState(State.roll);
            }
            else if (touchDuration >= touchTimeForRoll && touchDuration <= touchTimeForLightAttack && totalDirection != Vector2.zero)
            {
                SwitchState(State.lightAttack);
            }
            else if (touchDuration >= touchTimeForLightAttack && touchDuration <= touchTimeForHardAttack && totalDirection != Vector2.zero)
            {
                SwitchState(State.hardAttack);
            }

            touchDuration = 0;
        }

    }
    private void ExitBattleState()
    {
        animWeapon.SetBool("Preparing", false);
        anim.SetBool("BattleIdle", false);
    }



    private Vector2 swipeDirection(Vector2 startPos, Vector2 endPos)
    {
        var finalDelta = endPos - startPos;
        if (finalDelta.x < -Mathf.Abs(finalDelta.y)) finalDelta = -Vector2.right;
        if (finalDelta.x > Mathf.Abs(finalDelta.y)) finalDelta = Vector2.right;

        return finalDelta;
    }

    #endregion

    #region DefenseBattleState

    private void EnterDefenseBattleState()
    {

        anim.SetBool("DefenseBattle", true);
        isDefense = true;
        defenseTimeLeft = 0;

    }
    private void UpdateDefenseBattleState()
    {
        defenseTimeLeft += Time.deltaTime;
        if (defenseTimeLeft >= defenseDuration)
        {
            SwitchState(State.battle);
        }
    }
    private void ExitDefenseBattleState()
    {
        isDefense = false;
        anim.SetBool("DefenseBattle", false);
    }

    #endregion

    #region RollBattleState

    private void EnterRollBattleState()
    {
        isRoll = true;
        rollTimeLeft = 0;
        anim.SetBool("RollBattle", true);
        Debug.Log("IsRolling");
    }
    private void UpdateRollBattleState()
    {
        if (rollTimeLeft <= rollDuration)
        {
            rb.velocity = new Vector2(rollSpeed, 0) * totalDirection;
            rollTimeLeft += Time.deltaTime;
        }

        if (rollTimeLeft >= rollDuration || isDetectingWall || isDetectingLedge)
        {
            rb.velocity = Vector2.zero;
            SwitchState(State.battle);
        }

    }
    private void ExitRollBattleState()
    {
        anim.SetBool("RollBattle", false);
        isRoll = false;
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

    #region LightAttackBattleState

    private void EnterLightAttackBattleState()
    {
        isAttack = true;
        attackTimeLeft = 0;
        anim.SetBool("LightAttack", true);
        Debug.Log("isAttack");

    }
    private void UpdateLightAttackBattleState()
    {
        if (attackTimeLeft <= attackDuration)
        {
            rb.velocity = new Vector2(attackSpeed, 0) * totalDirection;
            attackTimeLeft += Time.deltaTime;
        }

        if (attackTimeLeft >= attackDuration || isDetectingWall || isDetectingLedge)
        {
            rb.velocity = Vector2.zero;
            SwitchState(State.battle);
        }

    }
    private void ExitLightAttackBattleState()
    {
        anim.SetBool("LightAttack", false);
        isAttack = false;
    }



    #endregion

    #region HardAttackBattleState

    private void EnterHardAttackBattleState()
    {
        isRoll = true;
        rollTimeLeft = 0;
        anim.SetBool("RollBattle", true);
        Debug.Log("IsRolling");
    }
    private void UpdateHardAttackBattleState()
    {
        if (rollTimeLeft <= rollDuration)
        {
            rb.velocity = new Vector2(rollSpeed, 0) * totalDirection;
            rollTimeLeft += Time.deltaTime;
        }

        if (rollTimeLeft >= rollDuration || isDetectingWall || isDetectingLedge)
        {
            rb.velocity = Vector2.zero;
            SwitchState(State.battle);
        }

    }
    private void ExitHardAttackBattleState()
    {
        anim.SetBool("RollBattle", false);
        isRoll = false;
    }



    #endregion

    #region StopBattleState

    private void EnterStopBattleState()
    {
        anim.SetBool("StopBattle", true);
        canJump = true;
        canMove = true;

    }
    private void UpdateStopBattleState()
    {
        if (!isInBattle)
        {
            SwitchState(State.idle);
        }
    }
    private void ExitStopBattleState()
    {
        anim.SetBool("StopBattle", false);
        isInBattle = false;
    }

    #endregion
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRaduis);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(wallCheckPoint.position, wallCheckPoint.position + Vector3.right * wallCheckDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(ledgeCheckPoint.position, ledgeCheckPoint.position + Vector3.down * ledgeCheckDistance);
    }

    public void TakeDamage(int damage)
    {

    }
}
