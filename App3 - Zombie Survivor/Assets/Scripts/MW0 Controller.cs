using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MW0Controller : MonoBehaviour
{
    [SerializeField] GameObject m_player;

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(m_player.transform.position, Vector3.forward, 90 * Time.deltaTime);
    }
}
