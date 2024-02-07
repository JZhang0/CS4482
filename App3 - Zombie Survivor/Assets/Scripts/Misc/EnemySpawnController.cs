using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    [SerializeField] GameObject m_enemy0prefab;
    [SerializeField] GameObject m_enemy1prefab;
    [SerializeField] GameObject m_enemy2prefab;
    [SerializeField] GameObject m_enemy3prefab;
    [SerializeField] GameObject m_enemy4prefab;

    private float m_spawnFrequency = 5.0f;
    private float m_timeTillSpawn;

    // Start is called before the first frame update
    void Start()
    {
        m_timeTillSpawn = m_spawnFrequency;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_timeTillSpawn >= 0.0f)
        {
            m_timeTillSpawn -= Time.deltaTime;
        }
        else if(!ProgressTracker.Instance.m_paused)
        {
            float val = Random.value;

            if (val <= 0.4f)
            {
                GameObject enemy0 = Instantiate(m_enemy0prefab);
                enemy0.transform.position = new Vector3(20.0f * Mathf.Sin(Mathf.Deg2Rad * (Random.value * 360.0f)), 20.0f * Mathf.Cos(Mathf.Deg2Rad * (Random.value * 360.0f)) - 0.1f, 0.0f);
                enemy0.transform.SetParent(transform);
            }
            else if (val <= 0.8f)
            {
                GameObject enemy4 = Instantiate(m_enemy1prefab);
                enemy4.transform.position = new Vector3(20.0f * Mathf.Sin(Mathf.Deg2Rad * (Random.value * 360.0f)), 20.0f * Mathf.Cos(Mathf.Deg2Rad * (Random.value * 360.0f)) - 0.1f, 0.0f);
                enemy4.transform.SetParent(transform);
            }
            else if (val <= 0.95f)
            {
                GameObject enemy1 = Instantiate(m_enemy1prefab);
                enemy1.transform.position = new Vector3(20.0f * Mathf.Sin(Mathf.Deg2Rad * (Random.value * 360.0f)), 20.0f * Mathf.Cos(Mathf.Deg2Rad * (Random.value * 360.0f)) - 0.1f, 0.0f);
                enemy1.transform.SetParent(transform);
            }
            else if (val <= 0.98f)
            {
                GameObject enemy2 = Instantiate(m_enemy2prefab);
                enemy2.transform.position = new Vector3(20.0f * Mathf.Sin(Mathf.Deg2Rad * (Random.value * 360.0f)), 20.0f * Mathf.Cos(Mathf.Deg2Rad * (Random.value * 360.0f)) - 0.1f, 0.0f);
                enemy2.transform.SetParent(transform);
            }
            else
            {
                GameObject enemy3 = Instantiate(m_enemy3prefab);
                enemy3.transform.position = new Vector3(20.0f * Mathf.Sin(Mathf.Deg2Rad * (Random.value * 360.0f)), 20.0f * Mathf.Cos(Mathf.Deg2Rad * (Random.value * 360.0f)) - 0.1f, 0.0f);
                enemy3.transform.SetParent(transform);
            }

            m_timeTillSpawn = m_spawnFrequency;
        }

        m_spawnFrequency = 5.0f / Mathf.Floor(((600.0f - ProgressTracker.Instance.gameDuration) / 2));
    }
}
