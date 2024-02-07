using UnityEngine;
using System.Collections;

public class HeroKnight : MonoBehaviour {

    [SerializeField] float      m_speed = 4.0f;
    [SerializeField] float      m_jumpForce = 7.5f;
    [SerializeField] float      m_rollForce = 6.0f;
    [SerializeField] GameObject m_slideDust;
    [SerializeField] GameObject m_fireballPrefab;
    [SerializeField] GameObject m_instructionPanel;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private Sensor_HeroKnight   m_groundSensor;
    private Sensor_HeroKnight   m_wallSensorR1;
    private Sensor_HeroKnight   m_wallSensorR2;
    private bool                m_isWallSliding = false;
    private bool                m_grounded = false;
    private bool                m_rolling = false;
    private bool                m_isblocking = false;
    private bool                m_isdead = false;
    private bool                m_wasSliding = false;
    private bool                m_canJumpOnWall = false;
    private int                 m_facingDirection = 1;
    private int                 m_currentAttack = 0;
    private float               m_timeSinceAttack = 0.0f;
    private float               m_delayToIdle = 0.0f;
    private float               m_rollDuration = 8.0f / 14.0f;
    private float               m_rollCurrentTime;
    private float               m_fallHeight;


    // Use this for initialization
    void Start ()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
    }

    // Update is called once per frame
    void Update ()
    {    
        if (!m_isdead)
        {
            // Increase timer that controls attack combo
            m_timeSinceAttack += Time.deltaTime;

            // Increase timer that checks roll duration
            if (m_rolling)
                m_rollCurrentTime += Time.deltaTime;

            // Disable rolling if timer extends duration
            if (m_rollCurrentTime > m_rollDuration)
                m_rolling = false;

            // -- Handle Animations --
            //Wall Slide
            m_isWallSliding = (m_wallSensorR1.State() || m_wallSensorR2.State()) && !m_grounded;
            if (m_isWallSliding)
            {
                if (!m_wasSliding)
                {
                    m_body2d.velocity = new Vector2(m_body2d.velocity.x, 0.0f);
                    m_wasSliding = true;
                }
                m_body2d.gravityScale = 0.2f;
            }
            else
            {
                if (m_wasSliding)
                {
                    m_wasSliding = false;
                }
                m_body2d.gravityScale = 1.0f;
            }
            m_animator.SetBool("WallSlide", m_isWallSliding);

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
                m_fallHeight = transform.position.y;
            }

            if (!m_grounded && m_fallHeight - transform.position.y >= 30.0f)
                die();

            // -- Handle input and movement --
            float inputX = Input.GetAxis("Horizontal");

            // Swap direction of sprite depending on walk direction
            if (inputX > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                if(m_facingDirection == -1 && m_isWallSliding)
                    m_canJumpOnWall = true;
                m_facingDirection = 1;
            }

            else if (inputX < 0)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                if (m_facingDirection == 1 && m_isWallSliding)
                    m_canJumpOnWall = true;
                m_facingDirection = -1;
            }

            // Move
            if (!m_rolling && (!m_isWallSliding || m_grounded) && !m_isblocking)
                m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

            //Set AirSpeed in animator
            m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

            //Pause game
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                transform.Find("ProgressTracker").GetComponent<ProgressTracker>().pauseGame();
            }

            //Fire ball
            else if (Input.GetKeyDown("q") && !m_isblocking)
            {
                GameObject fireBall = Instantiate(m_fireballPrefab);
                fireBall.transform.position = new Vector2(transform.position.x + m_facingDirection * 0.8f, transform.position.y + 1.0f);
                fireBall.transform.rotation = transform.rotation;
                fireBall.GetComponent<Rigidbody2D>().velocity = new Vector2(m_facingDirection * 5.0f, 0.0f);
            }

            //Attack
            else if (Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_rolling)
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

            // Block
            else if (Input.GetMouseButtonDown(1) && !m_rolling)
            {
                m_isblocking = true;
                m_animator.SetBool("IdleBlock", true);
            }

            else if (Input.GetMouseButtonUp(1))
            {
                m_isblocking = false;
                m_animator.SetBool("IdleBlock", false);
            }

            // Roll
            else if (Input.GetKeyDown("left shift") && !m_rolling && !m_isWallSliding)
            {
                m_rolling = true;
                m_animator.SetTrigger("Roll");
                m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
            }


            //Jump
            else if (Input.GetKeyDown("w"))
            {
                if(m_grounded && !m_rolling)
                {
                    if(m_wallSensorR1.State() || m_wallSensorR2.State())
                    {
                        m_wallSensorR1.Disable(0.5f);
                        m_wallSensorR2.Disable(0.5f);
                    }
                    m_animator.SetTrigger("Jump");
                    m_grounded = false;
                    m_animator.SetBool("Grounded", m_grounded);
                    m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
                    m_groundSensor.Disable(0.2f);
                }
                else if (m_canJumpOnWall && !m_rolling)
                {
                    m_canJumpOnWall = false;
                    m_animator.SetTrigger("Jump");
                    m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
                }
            }

            //Run
            else if (Mathf.Abs(inputX) > Mathf.Epsilon && (!m_isWallSliding || m_grounded))
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
    }

    // Animation Events
    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        spawnPosition = m_wallSensorR2.transform.position;

        if (m_slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "EnemySword" && !m_isdead)
        {
            if (!m_isblocking)
            {
                die();
            }
            else {
                m_animator.SetBool("Blocked", true);
            }
            
        }
        else if(collider.tag == "Finish")
            transform.Find("ProgressTracker").GetComponent<ProgressTracker>().PlayerWin();
        else if(collider.tag == "Instruction" && GetComponent<BoxCollider2D>().IsTouching(collider))
        {
            m_instructionPanel.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Instruction" && !GetComponent<BoxCollider2D>().IsTouching(collision))
        {
            m_instructionPanel.SetActive(false);
        }
    }

    private void endBlock()
    {
        m_animator.SetBool("Blocked", false);
    }

    private void die()
    {
        m_isdead = true;
        m_body2d.velocity = Vector3.zero;
        m_body2d.isKinematic = true;
        GetComponent<BoxCollider2D>().enabled = false;
        transform.Find("ProgressTracker").GetComponent<ProgressTracker>().PlayerDie();
        m_animator.SetTrigger("Death");
    }

    private void respawnPlayer()
    {
        m_isdead = false;
        transform.position = new Vector3(-7.0f, -2.0f, transform.position.z);
        m_body2d.isKinematic = false;
        GetComponent<BoxCollider2D>().enabled = true;
    }
}
