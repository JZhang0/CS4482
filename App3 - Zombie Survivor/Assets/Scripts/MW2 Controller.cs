using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MW2Controller : MonoBehaviour
{
    [SerializeField] GameObject m_player;

    private Rigidbody2D m_body2d;
    private float m_startDistanceToPlayer;
    private float m_pointingDirection;
    private readonly float m_maxSpeed = 10.0f;
    private float m_curretSpeed;

    // Start is called before the first frame update
    void Start()
    {
        m_body2d = GetComponent<Rigidbody2D>();
        m_startDistanceToPlayer = Vector3.Distance(transform.position, m_player.transform.position);

        transform.up = m_player.transform.position - transform.position;
        m_pointingDirection = transform.localEulerAngles.z + 90.0f;

        m_curretSpeed = m_maxSpeed;
        float horizontalV = m_curretSpeed * Mathf.Cos(Mathf.Deg2Rad * m_pointingDirection);
        float verticalV = m_curretSpeed * Mathf.Sin(Mathf.Deg2Rad * m_pointingDirection);
        m_body2d.velocity = new Vector2(horizontalV, verticalV);
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, m_player.transform.position) >= m_startDistanceToPlayer)
        {
            transform.up = m_player.transform.position - transform.position;
            m_pointingDirection = transform.localEulerAngles.z + 90.0f;
        }

        float horizontalV = m_curretSpeed * Mathf.Cos(Mathf.Deg2Rad * m_pointingDirection);
        float verticalV = m_curretSpeed * Mathf.Sin(Mathf.Deg2Rad * m_pointingDirection);
        m_body2d.velocity = new Vector2(horizontalV, verticalV);
    }
}
