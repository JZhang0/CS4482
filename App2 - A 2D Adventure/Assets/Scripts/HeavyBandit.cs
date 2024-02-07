using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyBandit : MonoBehaviour
{
    [SerializeField] bool m_disableMovement = false;
    [SerializeField] bool m_canRecover = true;

    private Animator                m_animator;
    private Rigidbody2D             m_body2d;
    private Sensor_Bandit           m_groundSensor;
    private Sensor_PlayerDetection  m_enemySensor;
    private Sensor_PlayerDetection  m_enemyNearbySensor;
    private GameObject              m_player;
    private bool                    m_isdead = false;
    private bool                    m_grounded = false;
    private bool                    m_sawPlayer = false;
    private bool                    m_isPlayerNearby = false;
    private bool                    m_shouldTurn = false;
    private bool                    m_irritated = false;
    private int                     m_health = 2;
    private float                   m_delayToTurn = 0.5f;
    private float                   m_speed = 3.0f;
    private float                   m_jumpForce = 7.5f;
    private float                   m_attackDuration = 1.5f;
    private float                   m_attackTimeLeft = 1.5f;
    private float                   m_takeDamageInterval = 0.0f;

    // Use this for initialization
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Bandit>();
        m_enemySensor = transform.Find("EnemySensor").GetComponent<Sensor_PlayerDetection>();
        m_enemyNearbySensor = transform.Find("EnemyNearbySensor").GetComponent<Sensor_PlayerDetection>();
        m_player = GameObject.Find("HeroKnight");
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_isdead)
        {
            //Check if character just landed on the ground
            if (!m_grounded && m_groundSensor.State())
            {
                m_grounded = true;
                m_animator.SetBool("Grounded", m_grounded);
            }

            //Check if character just started falling
            if (m_grounded && !m_groundSensor.State())
            {
                m_grounded = false;
                m_animator.SetBool("Grounded", m_grounded);
            }

            if (!m_shouldTurn)
                run(m_speed);
            else
            {
                if (m_delayToTurn <= 0.0f)
                {
                    run(m_speed, m_shouldTurn);
                    m_shouldTurn = false;
                    m_delayToTurn = 0.5f;
                }
                else
                {
                    m_animator.SetInteger("AnimState", 0);
                    m_delayToTurn -= Time.deltaTime;
                }
            }

            if (m_takeDamageInterval > 0)
            {
                m_takeDamageInterval -= Time.deltaTime;
            }

            //Set AirSpeed in animator
            m_animator.SetFloat("AirSpeed", m_body2d.velocity.y);

            m_sawPlayer = m_enemySensor.State();
            m_isPlayerNearby = m_enemyNearbySensor.State();

            if (m_sawPlayer)
            {
                m_irritated = true;
                m_speed = 5.0f;
                if (m_isPlayerNearby)
                {
                    run(0.5f);
                    if (m_attackTimeLeft == m_attackDuration)
                    {
                        m_attackTimeLeft -= Time.deltaTime;
                        m_animator.SetTrigger("Attack");
                    }
                    else if (m_attackTimeLeft < m_attackDuration && m_attackTimeLeft > 0)
                    {
                        m_attackTimeLeft -= Time.deltaTime;
                    }
                    else
                    {
                        m_attackTimeLeft = m_attackDuration;
                    }
                }
            }

            if (m_irritated)
            {
                if (transform.position.x - m_player.transform.position.x > 0.0f && transform.localScale.x == -1)
                {
                    m_shouldTurn = true;
                }

                else if (transform.position.x - m_player.transform.position.x < 0.0f && transform.localScale.x == 1)
                {
                    m_shouldTurn = true;
                }

                else if (transform.position.y - m_player.transform.position.y < 0.0f && m_grounded && m_health == 0 && m_takeDamageInterval <= 0)
                {
                    m_animator.SetTrigger("Jump");
                    m_grounded = false;
                    m_animator.SetBool("Grounded", m_grounded);
                    m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
                    m_groundSensor.Disable(0.2f);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Spell" && m_takeDamageInterval <= 0)
            takeHit();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "PlayerSword" && GetComponent<BoxCollider2D>().IsTouching(collider) && m_takeDamageInterval <= 0)
            takeHit();

        else if (collider.tag == "WallBlock" || (collider.tag == "Block" && !m_irritated))
        {
            if (!m_disableMovement && !m_sawPlayer)
            {
                m_shouldTurn = true;
            }
        }
    }

    private void takeHit()
    {
        if (m_health == 0)
        {
            m_body2d.velocity = Vector3.zero;
            m_body2d.isKinematic = true;
            GetComponent<BoxCollider2D>().enabled = false;
            m_sawPlayer = false;
            m_isPlayerNearby = false;
            m_irritated = false;
            m_enemySensor.Disable(1.0f);
            m_enemyNearbySensor.Disable(1.0f);
            m_disableMovement = true;
            m_isdead = true;
            m_animator.SetTrigger("Death");
        }
        else
        {
            m_takeDamageInterval = 0.2f;
            m_health -= 1;
            m_animator.SetTrigger("Hurt");
        }
    }

    public void endRecover()
    {
        m_health = 2;
        m_speed = 3.0f;
        m_attackDuration = 0.5f;
        m_canRecover = false;
        m_isdead = false;
        m_body2d.isKinematic = false;
        GetComponent<BoxCollider2D>().enabled = true;
        m_sawPlayer = true;
        m_isPlayerNearby = true;
        m_disableMovement = false;
    }

    private void run(float speed, bool changeDirection = false)
    {
        if (!m_disableMovement)
        {
            int direction = changeDirection ? -1 : 1;
            transform.localScale = new Vector3(direction * transform.localScale.x, 1.0f, 1.0f);
            m_body2d.velocity = new Vector2(-transform.localScale.x * speed, m_body2d.velocity.y);
            m_animator.SetInteger("AnimState", 2);
        }
    }

    public void destroyEvent()
    {
        if (!m_canRecover)
        {
            float animTime = m_animator.GetCurrentAnimatorStateInfo(0).length + 0.5f;
            Destroy(gameObject, animTime);
        }
        else{
            m_animator.SetTrigger("Recover");
        }
    }
}
