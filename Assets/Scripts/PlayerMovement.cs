using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5;
    public int facingDirection = 1;
    public Rigidbody2D rb;

    public Animator anim;

    private bool isKnockedBack;
 

    // FixedUpdate is called 50 per frame
    void FixedUpdate()
    {
        if(isKnockedBack == false)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if((horizontal > 0 && transform.localScale.x > 0) ||
            (horizontal < 0 && transform.localScale.x < 0))
            {
                Flip();
            }

            if (horizontal != 0)
            {
                anim.SetFloat("horizontal", horizontal);
                anim.SetFloat("vertical", 0);
            }
            else
            {
                anim.SetFloat("horizontal", horizontal);
                anim.SetFloat("vertical", vertical);
            }

            rb.linearVelocity = new Vector2(horizontal, vertical) * speed; 
        }

    }

    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3 (transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    public void Knockback(Transform enemy, float force, float stunTime)
    {
        isKnockedBack = true;
        Vector2 direction = (transform.position - enemy.position).normalized;
        rb.linearVelocity = direction * force;
        StartCoroutine(KnockBackCounter(stunTime));
    }

    IEnumerator KnockBackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.linearVelocity = Vector2.zero;
        isKnockedBack = false;
    }
}
