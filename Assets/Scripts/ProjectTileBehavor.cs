using UnityEngine;

public class ProjectTileBehavor : MonoBehaviour, IDamageable
{
    [SerializeField] private ProjectileType projectileType;
    [SerializeField] private float speed;
    [SerializeField] private int damage = 1;
   
    [SerializeField] private float travelDistance;
    [SerializeField] private bool isFly = true;
   
   
    private Rigidbody2D rb;
    private float startPosX;
    private ParticleSystem blowPartical;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        blowPartical = Resources.Load<ParticleSystem>("Charters/ParticalSystems and effects/Particle System Blow");
    }

    private void Start()
    {
        startPosX = transform.position.x;
        rb.velocity = transform.right * speed;
    }
    private void Update()
    {
        if (!isFly)
        {
            Physics2D.IgnoreLayerCollision(8, 10,true);
            Physics2D.IgnoreLayerCollision(7, 10,true);
        }
        else
        {
            Physics2D.IgnoreLayerCollision(8, 10, false);
            Physics2D.IgnoreLayerCollision(7, 10, false);
        }

        if (isFly)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        }

    }

    private void FixedUpdate()
    {

        if (Mathf.Abs(startPosX - transform.position.x) >= travelDistance)
        {
            rb.gravityScale = 2;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + travelDistance, transform.position.y));
        
    }




    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isFly)
        {
            if(collision.gameObject.TryGetComponent<IDamageable>(out IDamageable example))
            {
                example.TakeDamage(damage, true);
            }
          
        }

        AfterTouchLogic();
    }

    private void AfterTouchLogic()
    {

        switch (projectileType)
        {
            case ProjectileType.DestroyOnTouch:

                rb.velocity = Vector2.zero;
                Instantiate(blowPartical, transform.position, blowPartical.transform.rotation);
                Destroy(gameObject);

                break;
            case ProjectileType.DontDestroyOnTouch:
                isFly = false;
                rb.gravityScale = 2;
                break;
            default:

                break;
        }
    }

    public void TakeDamage(int damage, bool isDamage)
    {
        rb.gravityScale = 2;
        isFly = true;
    }

    public void FireProjectTile(float travelDistance, int damage)
    {
      
        this.travelDistance = travelDistance;
        this.damage = damage;
    }


}


enum ProjectileType
{
    DestroyOnTouch,
    DontDestroyOnTouch
}