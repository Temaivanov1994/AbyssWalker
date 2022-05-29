using UnityEngine;
using UnityEngine.Events;

public class boxBehavour : MonoBehaviour
{
    Rigidbody2D rb;
    [Header("GroundCheck Parametrs")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRaduis = 0.18f;
    [SerializeField] private LayerMask whatIsGround;
    private bool isDetectingGround;
    public UnityEvent OnLandEvent;
    private ParticleSystem OnLandingParticleSystem;



    private void Awake()
    {
        OnLandingParticleSystem = Resources.Load<ParticleSystem>("Charters/ParticalSystems and effects/OnLanding");
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
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


    public void OnlandingEvent()
    {
        Instantiate(OnLandingParticleSystem, transform.position, OnLandingParticleSystem.transform.rotation);

    }
    private void OnCollisionStay2D(Collision2D collision)
    {
       
        if (!isDetectingGround)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && rb.velocity.y < -5)
        {
            Destroy(collision.gameObject);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRaduis);
    }
}
