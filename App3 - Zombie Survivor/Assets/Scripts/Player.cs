using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] float              m_speed = 6.0f;
    [SerializeField] Sprite             m_rangeWeaponSprite0;
    [SerializeField] Sprite             m_rangeWeaponSprite1;
    [SerializeField] Sprite             m_rangeWeaponSprite2;

    private Animator        m_animator;
    private Rigidbody2D     m_body2d;
    private int             m_facingDirection = 1;
    private float           m_maxHealth = 100.0f;
    private float           m_currentHealth;
    private float           m_damageInterval;


    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        m_body2d = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();

        m_currentHealth = m_maxHealth;
        ProgressTracker.Instance.changeHealth(m_currentHealth / m_maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ProgressTracker.Instance.pauseGame();
        }

        if (m_currentHealth > 0.0f && !ProgressTracker.Instance.m_paused)
        {
            if (m_damageInterval >= 0)
            {
                transform.GetComponent<SpriteRenderer>().color = new Color(1f, 150f / 255f, 150f / 255f);
                m_damageInterval -= Time.deltaTime;
            }
            else
                transform.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);

            // -- Handle input and movement --
            float inputX = Input.GetAxis("Horizontal");
            float inputY = Input.GetAxis("Vertical");

            // Swap direction of sprite depending on walk direction
            if (inputX > 0 && m_facingDirection == -1)
            {
                m_facingDirection = 1;
                changeDirection(false);
            }

            else if (inputX < 0 && m_facingDirection == 1)
            {
                m_facingDirection = -1;
                changeDirection(true);
            }

            m_body2d.velocity = new Vector2(inputX * m_speed, inputY * m_speed);

            if (Input.GetKeyDown(KeyCode.Q))
            {
                ProgressTracker.Instance.GainXp(100);
            }

            if (Mathf.Abs(inputX) + Mathf.Abs(inputY) > Mathf.Epsilon)
                m_animator.SetBool("isMoving", true);
            else
                m_animator.SetBool("isMoving", false);
        }
        else
        {
            m_body2d.velocity = Vector3.zero;
        }
    }

    private void changeDirection(bool isFacingLeft)
    {
        transform.GetComponent<SpriteRenderer>().flipX = isFacingLeft;
        transform.Find("MeleeWeapon").GetComponent<MeleeWeaponController>().changeDirection(isFacingLeft);
        transform.Find("RangeWeapon").GetComponent<RangeWeaponController>().changeDirection(isFacingLeft);
    }

    public void takeDamage(float damage)
    {
        if(m_damageInterval < 0.0f && m_currentHealth > 0.0f)
        {
            m_currentHealth -= damage;
            m_damageInterval = 0.1f;
            ProgressTracker.Instance.changeHealth(m_currentHealth / m_maxHealth);
            if (m_currentHealth <= 0.0f)
            {
                transform.GetComponent<BoxCollider2D>().enabled = false;
                m_animator.SetBool("Death", true);
            }
        }
    }

    public void addHealth(float healAmount)
    {
        if(m_currentHealth + healAmount <= m_maxHealth)
        {
            m_currentHealth += healAmount;
            ProgressTracker.Instance.changeHealth(m_currentHealth / m_maxHealth);
        }
    }

    public void changeSpeed(float newSpeed)
    {
        m_speed = newSpeed;
    }

    public void destroyEvent()
    {
        float animTime = m_animator.GetCurrentAnimatorStateInfo(0).length;

        Destroy(gameObject, animTime);


    }
}
