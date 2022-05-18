using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private int attackDamage =1;
    [SerializeField] private float knockbackForce = 200;
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rbTarget = collision.GetComponent<Rigidbody2D>();
            collision.GetComponent<IDamageable>().TakeDamage(attackDamage, true);

            Vector2 knockbackDirection = collision.transform.position - transform.position;
            rbTarget.AddForce(knockbackDirection.normalized * knockbackForce);
        }
    }
}
