using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float m_damageToPlayer;
    public float m_health;
    public bool m_disableMovement = false;

    [SerializeField] GameObject m_Exp0Prefab;
    [SerializeField] GameObject m_Exp1Prefab;
    [SerializeField] GameObject m_Exp2Prefab;
    [SerializeField] GameObject m_HealthPrefab;
    [SerializeField] GameObject m_MagPrefab;
    [SerializeField] float m_speed;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private float m_damageInterval;

    // Start is called before the first frame update
    void Start()
    {
        m_body2d = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_damageInterval > 0.0f)
        {
            m_damageInterval -= Time.deltaTime;
        }

        if (m_health > 0.0f && !m_disableMovement && !ProgressTracker.Instance.m_paused)
        {
            Vector3 direction = Player.Instance.transform.position - transform.position;
            float distanceToPlayer = Vector3.Distance(Player.Instance.transform.position, transform.position);
            float horizontalV = m_speed * (direction.x / distanceToPlayer);
            float verticalV = m_speed * (direction.y / distanceToPlayer);
            m_body2d.velocity = new Vector2(horizontalV, verticalV);
        }
        else
            m_body2d.velocity = Vector3.zero;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().takeDamage(m_damageToPlayer);
        }
    }

    private void dropReward()
    {
        float val = Random.value;

        GameObject drops = GameObject.Find("Drops");

        if(val <= 0.9f)
        {
            GameObject exp0 = Instantiate(m_Exp0Prefab, drops.transform);
            exp0.transform.position = transform.position;
        }
        else if (val <= 0.95f)
        {
            GameObject exp1 = Instantiate(m_Exp1Prefab, drops.transform);
            exp1.transform.position = transform.position;
        }
        else if (val <= 0.97f)
        {
            GameObject exp2 = Instantiate(m_Exp2Prefab, drops.transform);
            exp2.transform.position = transform.position;
        }
        else if (val <= 0.99f)
        {
            GameObject Heal = Instantiate(m_HealthPrefab, drops.transform);
            Heal.transform.position = transform.position;
        }
        else
        {
            GameObject mag = Instantiate(m_MagPrefab, drops.transform);
            mag.transform.position = transform.position;
        }
    }

    public void takeDamage(float damage)
    {
        if (m_damageInterval <= 0.0f && m_health > 0.0f)
        {
            m_animator.SetTrigger("Hit");
            m_health -= damage;
            m_damageInterval = 0.1f;
            if(m_health <= 0.0f)
            {
                m_body2d.velocity = Vector3.zero;
                transform.GetComponent<BoxCollider2D>().enabled = false;
                m_animator.SetBool("Dead", true);

                ProgressTracker.Instance.enemyDie();
                dropReward();
            }
        }
    }

    public void destroyEvent()
    {
        float animTime = m_animator.GetCurrentAnimatorStateInfo(0).length;

        Destroy(gameObject, animTime);
    }
}
