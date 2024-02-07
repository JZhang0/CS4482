using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    private Animator    m_animator;
    private float       m_timeSinceCreation = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    private void Update()
    {
        m_timeSinceCreation += Time.deltaTime;

        if(m_timeSinceCreation >= 1.5f)
            hit();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag != "Spell")
            hit();
    }

    private void hit()
    {
        m_animator.SetTrigger("Hit");
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<CapsuleCollider2D>().enabled = false;
    }

    public void destroyEvent()
    {
        float animTime = m_animator.GetCurrentAnimatorStateInfo(0).length;

        Destroy(gameObject, animTime);
    }
}
