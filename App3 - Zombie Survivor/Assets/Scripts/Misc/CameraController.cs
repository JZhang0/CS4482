using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject m_player;

    void Update()
    {
        transform.position = new Vector3(m_player.transform.position.x, m_player.transform.position.y, transform.position.z);
    }
}
