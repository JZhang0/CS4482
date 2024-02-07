using UnityEngine;
using System.Collections;

public class Warrior : MonoBehaviour
{

    [SerializeField] float m_speed = 2.0f;
    [SerializeField] float m_jumpForce = 7.5f;
    [SerializeField] float m_rollForce = 6.0f;
    [SerializeField] float m_climbForce = 6.0f;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_Warrior m_groundSensor;
    private Sensor_Warrior m_wallSensorR1;
    private Sensor_Warrior m_wallSensorR2;
    private Sensor_Warrior m_wallSensorL1;
    private Sensor_Warrior m_wallSensorL2;
    private int m_health = 100;
    private float m_maxSpeed = 10.0f;
    private bool m_isWallHanging = false;
    private bool m_grounded = false;
    private int m_facingDirection = 1;
    private int m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;
    private float m_delayToIdle = 0.0f;
    private float m_floatingTime = 0.0f;

    private bool m_rolling = false;
    private float m_rollDuration = 8.0f / 14.0f;
    private float m_rollCurrentTime;

    private bool m_dashing = false;
    private float m_dashDuration = 8.0f / 14.0f;
    private float m_dashCurrentTime;

    private bool m_sliding = false;
    private float m_slideDuration = 8.0f / 14.0f;
    private float m_slideCurrentTime;


    // Use this for initialization
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Warrior>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_Warrior>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_Warrior>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_Warrior>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_Warrior>();
    }

    // Update is called once per frame
    void Update()
    {
        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;

        // Increase timer that checks roll duration
        if (m_rolling)
            m_rollCurrentTime += Time.deltaTime;

        // Disable rolling if timer extends duration
        if (m_rollCurrentTime > m_rollDuration)
            m_rolling = false;

        if (!m_grounded && !m_groundSensor.State())
        {
            m_floatingTime += Time.deltaTime;
            if (m_floatingTime >= 3.0f)
            {
                RespawnPlayer();
                m_floatingTime = 0.0f;
            }
        }

        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
            m_floatingTime = 0.0f;
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        // -- Handle Animations --
        //Wall Slide
        m_isWallHanging = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
        if (m_isWallHanging)
        {
            m_floatingTime = 0.0f;
        }
        m_animator.SetBool("WallHang", m_isWallHanging);

        checkPlayerInput();
    }

    private void checkPlayerInput()
    {
        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }

        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        // Move
        if (!m_rolling && (Mathf.Abs(inputX) > Mathf.Epsilon))
        {
            if(m_body2d.velocity.x + inputX * m_speed <= m_maxSpeed)
            {
                m_body2d.velocity = new Vector2(m_body2d.velocity.x + inputX * m_speed, m_body2d.velocity.y);
            }

            PlayerMove(m_body2d.velocity.x);
        }

        //Hit
        if (Input.GetKeyDown("y") && !m_rolling)
        {
            PlayerHit(50);
        }

        //Attack
        else if (Input.GetMouseButtonDown(0))
        {
            PlayerAttack();
        }

        // Block
        else if (Input.GetMouseButtonDown(1) && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
            PlayerBlock(true, true);
        }

        else if (Input.GetMouseButtonUp(1))
        {
            PlayerBlock(false, false);
        }

        // Roll
        else if (Input.GetKeyDown("left shift") && !m_rolling && !m_isWallHanging)
        {
            PlayerRoll();
        }

        //Jump
        else if ((Input.GetKeyDown("w") && m_grounded && !m_rolling) || (Input.GetKeyDown("w") && m_isWallHanging && !m_rolling))
        {
            PlayerJump();
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0)
                m_animator.SetInteger("AnimState", 0);
        }
    }

    private void PlayerMove(float speedX)
    {
        m_animator.SetFloat("GroundSpeedX", speedX);
    }

    private void PlayerHit(int damage)
    {
        m_health -= 25;
        m_animator.SetTrigger("Hit");
    }

    private void PlayerBlock(bool pressed, bool blocked)
    {
        if (pressed)
        {
            m_animator.SetBool("IdleBlock", pressed);
            if(blocked)
            {
                m_animator.SetTrigger("Block");
            }
        }
    }

    private void PlayerRoll()
    {
        m_rolling = true;
        m_animator.SetTrigger("Roll");
        m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
    }

    private void PlayerJump()
    {
        if (m_isWallHanging)
        {
            if (m_wallSensorR1.State() && m_wallSensorR2.State())
            {
                m_body2d.velocity = new Vector2(-2.0f, m_body2d.velocity.y);
                m_wallSensorR1.Disable(0.2f);
                m_wallSensorR2.Disable(0.2f);
            }
            else if (m_wallSensorL1.State() && m_wallSensorL2.State())
            {
                m_body2d.velocity = new Vector2(2.0f, m_body2d.velocity.y);
                m_wallSensorL1.Disable(0.2f);
                m_wallSensorL2.Disable(0.2f);
            }
            m_isWallHanging = false;
            m_animator.SetBool("WallSlide", m_isWallHanging);
        }
        m_animator.SetTrigger("Jump");
        m_grounded = false;
        m_animator.SetBool("Grounded", m_grounded);
        m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
        m_groundSensor.Disable(0.2f);
    }

    private void PlayerAttack()
    {
        //Attack
        if(m_timeSinceAttack > 0.25f && !m_rolling)
        {
            m_currentAttack++;

            // Loop back to one after third attack
            if (m_currentAttack > 3)
                m_currentAttack = 1;

            // Reset Attack combo if time since last attack is too large
            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            m_animator.SetTrigger("Attack" + m_currentAttack);

            // Reset timer
            m_timeSinceAttack = 0.0f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Finish")
        {
            RespawnPlayer();
        }
        if (collision.gameObject.tag == "Enemy")
        {
            RespawnPlayer();
        }
    }

    private void RespawnPlayer()
    {
        transform.position = new Vector3(-7, -1.1f, transform.position.z);
    }
}
