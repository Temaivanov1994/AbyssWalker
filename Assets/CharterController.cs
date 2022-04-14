using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class CharterController : MonoBehaviour
{
    [SerializeField] private State currentState;
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;

    [Header("GroundCheck Parametrs")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRaduis;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private bool isDetectingGround;
    [Header("WallCheck Parametrs")]
    [SerializeField] private Transform wallCheckPoint;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private bool isDetectingWall;
    [Header("LedgeCheck Parametrs")]
    [SerializeField] private Transform ledgeCheckPoint;
    [SerializeField] private float ledgeCheckDistance;
    [SerializeField] private bool isDetectingLedge;
    [Header("Movement Parametrs")]
    [SerializeField] private float movementDirection;
    [SerializeField] private float movementSpeed;
    [SerializeField] private bool facingRight = true;
    [SerializeField] private bool isMoving = false;
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;
    [SerializeField] private Vector3 zeroVelocity = Vector3.zero;
    [Header("Jump Parametrs")]
    [SerializeField] private bool isJump = false;
    [SerializeField] private float jumpForce;
    public UnityEvent OnLandEvent;




    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }
    private void Start()
    {
        SwitchState(State.idle);
    }
    private void Update()
    {
        UpdateStateMachinte();

        movementDirection = Input.GetAxisRaw("Horizontal");

        if (movementDirection > 0 && !facingRight)
        {
            Flip();
        }

        else if (movementDirection < 0 && facingRight)
        {
            Flip();
        }

        if (movementDirection != 0 && isDetectingGround && !isMoving)
        {
            SwitchState(State.move);
        }
        else if(movementDirection == 0)
        {
            SwitchState(State.idle);
        }

        EnviromentDetecting();
    }

    private void FixedUpdate()
    {

        Vector3 targetVelocity = new Vector2(movementDirection * movementSpeed * Time.fixedDeltaTime, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref zeroVelocity, movementSmoothing);
    }


    private void EnviromentDetecting()
    {
        isDetectingGround = CheckGround();

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
    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }


    #region StateMachine

    private void UpdateStateMachinte()
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
        }
        currentState = state;
    }


    private enum State
    {
        idle,
        move,
        jump,
    }

    #endregion

    #region IdleState

    private void EnterIdleState()
    {
        Debug.Log("EnterIdleState");
        anim.SetBool("Idle", true);
    }
    private void UpdateIdleState()
    {

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
        Debug.Log("EnterMoveState");
        isMoving = true;
    }
    private void UpdateMoveState()
    {

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
        anim.SetBool("Jump", true);
        Debug.Log("EnterJumpState");
    }
    private void UpdateJumpState()
    {

    }
    private void ExitJumpState()
    {
        anim.SetBool("Jump", false);
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
}
