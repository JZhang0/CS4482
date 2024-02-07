using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farmer : MonoBehaviour
{
    [SerializeField] float m_speed = 6.0f;
    [SerializeField] float m_health = 100.0f;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private int m_facingDirection = 1;
    private int m_MeleeWeapon = 0;
    private int m_RangeWeapon = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_body2d = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_health > 0.0f)
        {
            // -- Handle input and movement --
            float inputX = Input.GetAxis("Horizontal");
            float inputY = Input.GetAxis("Vertical");


            // Swap direction of sprite depending on walk direction
            if (inputX > 0)
            {
                transform.GetComponent<SpriteRenderer>().flipX = false;
                m_facingDirection = 1;
            }

            else if (inputX < 0)
            {
                transform.GetComponent<SpriteRenderer>().flipX = true;
                m_facingDirection = -1;
            }

            m_body2d.velocity = new Vector2(inputX * m_speed, inputY * m_speed);

            if (Mathf.Abs(inputX) + Mathf.Abs(inputY) > Mathf.Epsilon)
                m_animator.SetBool("isMoving", true);
            else
                m_animator.SetBool("isMoving", false);
        }
    }
}
