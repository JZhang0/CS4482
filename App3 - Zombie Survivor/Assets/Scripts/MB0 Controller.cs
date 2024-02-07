using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MB0Controller : MonoBehaviour
{
    public float m_damageToEnemy;

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(Player.Instance.transform.position, Vector3.forward, 180.0f * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemy>().takeDamage(m_damageToEnemy);
        }
    }
}
