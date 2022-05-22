using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float knockbackForce = 500;

    [Header("GroundCheck Parametrs")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRaduis = 0.18f;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private bool isDetectingGround;
    [SerializeField] int currentRorate = 0;

    void Start()
    {
        SetRotation();

    }


    


    private bool CheckGround()
    {

        isDetectingGround = false;


        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckPoint.position, groundCheckRaduis, whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                isDetectingGround = true;

                return true;
            }
        }
        return false;
    }


    private void SetRotation()
    {
        
        isDetectingGround = CheckGround();
        if (!isDetectingGround && currentRorate<=4)
        {
            transform.Rotate(0, 0, 90);
            currentRorate += 1;
            SetRotation();

        }
        else if(currentRorate>=4)
        {
            Destroy(gameObject);
        }
       


    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rbTarget = collision.GetComponent<Rigidbody2D>();
            if (rbTarget.velocity.y < -5f)
            {
                collision.GetComponent<IDamageable>().TakeDamage(attackDamage, true);

                Vector2 knockbackDirection = collision.transform.position - transform.position;
                rbTarget.AddForce(knockbackDirection.normalized * knockbackForce);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRaduis);
    }
}
