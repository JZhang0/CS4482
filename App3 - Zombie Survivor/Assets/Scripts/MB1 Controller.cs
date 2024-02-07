using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MB1Controller : MonoBehaviour
{
    public float m_damageToEnemy;

    private Rigidbody2D     m_body2d;
    private float           m_pointingDirection;
    private readonly float  m_maxSpeed = 12.5f;
    private float           m_curretSpeed;

    // Start is called before the first frame update
    void Start()
    {
        m_pointingDirection = transform.localEulerAngles.z + 90.0f;
        m_body2d = GetComponent<Rigidbody2D>();

        m_curretSpeed = m_maxSpeed;
        float horizontalV = m_curretSpeed * Mathf.Cos(Mathf.Deg2Rad * m_pointingDirection);
        float verticalV = m_curretSpeed * Mathf.Sin(Mathf.Deg2Rad * m_pointingDirection);
        m_body2d.velocity = new Vector2(horizontalV, verticalV);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_curretSpeed >= 1.0f)
            m_curretSpeed -= 0.1f * m_curretSpeed / m_maxSpeed;
        else
            Destroy(gameObject);

        float horizontalV = m_curretSpeed * Mathf.Cos(Mathf.Deg2Rad * m_pointingDirection);
        float verticalV = m_curretSpeed * Mathf.Sin(Mathf.Deg2Rad * m_pointingDirection);
        m_body2d.velocity = new Vector2(horizontalV, verticalV);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemy>().takeDamage(m_damageToEnemy);
        }
    }
}
