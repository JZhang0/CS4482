using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : MonoBehaviour
{
    [SerializeField] int m_expAmount;
    [SerializeField] float m_healAmount;
    [SerializeField] bool m_isMagnet;

    private Rigidbody2D m_body2d;
    private float m_magnetPeriod = 0.0f;

    private void Start()
    {
        m_body2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_magnetPeriod > 0.0f)
        {
            m_magnetPeriod -= Time.deltaTime;

            Vector3 direction = Player.Instance.transform.position - transform.position;
            float distanceToPlayer = Vector3.Distance(Player.Instance.transform.position, transform.position);
            float horizontalV = 5.0f * (direction.x / distanceToPlayer);
            float verticalV = 5.0f * (direction.y / distanceToPlayer);
            m_body2d.velocity = new Vector2(horizontalV, verticalV);
        }
    }

    public void findPlayer()
    {
        m_magnetPeriod = 10.0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerSensor")
        {
            Player.Instance.addHealth(m_healAmount);
            ProgressTracker.Instance.GainXp(m_expAmount);

            if (m_isMagnet)
            {
                Transform drops = GameObject.Find("Drops").transform;

                foreach(Transform drop in drops)
                {
                    drop.GetComponent<Pickable>().findPlayer();
                }
            }

            Destroy(gameObject);
        }
    }
}
