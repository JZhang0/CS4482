using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB1Controller : MonoBehaviour
{
    public float m_damageToEnemy;

    [SerializeField] float m_timeBeforeDestroy;
    [SerializeField] float m_speed;

    private Rigidbody2D m_body2d;

    // Start is called before the first frame update
    void Start()
    {
        float direction = transform.localEulerAngles.z + 90.0f;
        m_body2d = GetComponent<Rigidbody2D>();

        float horizontalV = m_speed * Mathf.Cos(Mathf.Deg2Rad * direction);
        float verticalV = m_speed * Mathf.Sin(Mathf.Deg2Rad * direction);
        m_body2d.velocity = new Vector2(horizontalV, verticalV);
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
