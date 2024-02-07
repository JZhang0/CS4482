using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MB2Controller : MonoBehaviour
{
    public float m_damageToEnemy;

    private Rigidbody2D m_body2d;
    private float m_startDistanceToPlayer;
    private float m_pointingDirection;
    private float m_speed = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_body2d = GetComponent<Rigidbody2D>();
        m_startDistanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);

        transform.up = Player.Instance.transform.position - transform.position;
        m_pointingDirection = transform.localEulerAngles.z + 90.0f;

        float horizontalV = m_speed * Mathf.Cos(Mathf.Deg2Rad * m_pointingDirection);
        float verticalV = m_speed * Mathf.Sin(Mathf.Deg2Rad * m_pointingDirection);
        m_body2d.velocity = new Vector2(horizontalV, verticalV);
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, Player.Instance.transform.position) >= m_startDistanceToPlayer)
        {
            transform.up = Player.Instance.transform.position - transform.position;
            m_pointingDirection = transform.localEulerAngles.z + 90.0f;
        }

        float horizontalV = m_speed * Mathf.Cos(Mathf.Deg2Rad * m_pointingDirection);
        float verticalV = m_speed * Mathf.Sin(Mathf.Deg2Rad * m_pointingDirection);
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
