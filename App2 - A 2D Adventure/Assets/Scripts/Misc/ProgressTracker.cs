using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ProgressTracker : MonoBehaviour
{
    [SerializeField] GameObject             m_GravestonePrefab;
    [SerializeField] TMPro.TextMeshProUGUI  m_GameTimeDisplay;
    [SerializeField] GameObject             m_PauseMenu;

    private int     m_deathCount = 0;
    private float   m_gameDuration = 0.0f;
    private bool    m_paused;

    // Update is called once per frame
    void Update()
    {
        if (!m_paused)
        {
            m_gameDuration += Time.deltaTime;
            TimeSpan timeSoFar = TimeSpan.FromSeconds(m_gameDuration);
            m_GameTimeDisplay.text = timeSoFar.ToString("hh':'mm':'ss");
        }  
    }

    public void pauseGame()
    {
        m_paused = !m_paused;
        m_PauseMenu.SetActive(m_paused);
    }

    public void restartGame()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().StartLevel();
    }

    public void exitMenu()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().ChangeMenuScene();
    }

    public void PlayerDie()
    {
        GameObject grave = Instantiate(m_GravestonePrefab, new Vector2(-7 + m_deathCount * 0.5f, -2), Quaternion.identity);
        grave.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        m_deathCount += 1;
    }

    public void PlayerWin()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().ChangeMenuScene(m_gameDuration);
    }
}
