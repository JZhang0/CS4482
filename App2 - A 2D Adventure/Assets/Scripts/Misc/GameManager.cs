using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager   Instance;
    [SerializeField] GameObject m_LeaderBoardEntryPrefab;

    private Dictionary<string, float>   m_playerCompletionTime;
    private string                      m_currentPlayerName;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        m_playerCompletionTime = new Dictionary<string, float>();
    }

    public void StartLevel(string playerName = null)
    {
        if(playerName != null)
            m_currentPlayerName = playerName;
        SceneManager.LoadScene("Level1");
    }

    public void ChangeMenuScene(float completionTime = -1.0f)
    {
        if(completionTime != -1.0f)
            m_playerCompletionTime[m_currentPlayerName] = completionTime;
        SceneManager.LoadScene("Menu");
    }
    
    public void exitGame()
    {
        Application.Quit();
    }

    public void displayScore()
    {

        GameObject leaderboardContent = GameObject.Find("LeaderBoardContent");
        foreach (Transform child in leaderboardContent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        leaderboardContent.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(leaderboardContent.transform.GetComponent<RectTransform>().sizeDelta.x, 40);

        m_playerCompletionTime = m_playerCompletionTime.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

        int i = 0;
        foreach (KeyValuePair<string, float> pair in m_playerCompletionTime)
        {
            GameObject entry = Instantiate(m_LeaderBoardEntryPrefab);
            entry.transform.SetParent(leaderboardContent.transform, false);
            entry.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, entry.GetComponent<RectTransform>().localPosition.y - i * 40);
            
            entry.transform.Find("NameEntry").GetComponent<UnityEngine.UI.Text>().text = pair.Key;

            TimeSpan completionTime = TimeSpan.FromSeconds(pair.Value);
            entry.transform.Find("TimeEntry").GetComponent<UnityEngine.UI.Text>().text = completionTime.ToString("hh':'mm':'ss");
            leaderboardContent.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(leaderboardContent.transform.GetComponent<RectTransform>().sizeDelta.x, leaderboardContent.transform.GetComponent<RectTransform>().sizeDelta.y + 40);
            i++;
        }
    }
}
