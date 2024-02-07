using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB0Controller : MonoBehaviour
{
    public float m_damageToEnemy;

    [SerializeField] float m_timeBeforeDestroy;
    [SerializeField] float m_speed;

    private GameObject m_closestEnemy = null;
    private Rigidbody2D m_body2d;

    // Start is called before the first frame update
    void Start()
    {
        m_body2d = GetComponent<Rigidbody2D>();

        GameObject m_enemies = GameObject.Find("Enemy");

        foreach (Transform enemy in m_enemies.transform)
        {
            if (m_closestEnemy == null)
                m_closestEnemy = enemy.gameObject;
            else
            {
                if (Vector3.Distance(transform.position, enemy.transform.position) < Vector3.Distance(transform.position, m_closestEnemy.transform.position))
                {
                    m_closestEnemy = enemy.gameObject;
                }
            }
        }

        if (m_closestEnemy == null)
        {
            float direction = transform.localEulerAngles.z + 90.0f;

            float horizontalV = m_speed * Mathf.Cos(Mathf.Deg2Rad * direction);
            float verticalV = m_speed * Mathf.Sin(Mathf.Deg2Rad * direction);
            m_body2d.velocity = new Vector2(horizontalV, verticalV);
        }
        else
        {
            transform.up = m_closestEnemy.transform.position - transform.position;
            float direction = transform.localEulerAngles.z + 90.0f;
            float horizontalV = m_speed * Mathf.Cos(Mathf.Deg2Rad * direction);
            float verticalV = m_speed * Mathf.Sin(Mathf.Deg2Rad * direction);
            m_body2d.velocity = new Vector2(horizontalV, verticalV);
        }
    }

    private void Update()
    {
        if (m_timeBeforeDestroy >= 0.0f)
            m_timeBeforeDestroy -= Time.deltaTime;
        else
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemy>().takeDamage(m_damageToEnemy);
            Destroy(gameObject);
        }
    }
}
