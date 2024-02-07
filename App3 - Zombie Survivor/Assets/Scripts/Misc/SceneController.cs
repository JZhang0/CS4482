using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField] GameObject m_MainMenu;
    [SerializeField] GameObject m_InputNameMenu;
    [SerializeField] GameObject m_LeaderBoardMenu;
    [SerializeField] GameObject m_CreditMenu;
    
    void viewMainMenu()
    {
        m_MainMenu.SetActive(true);
        m_InputNameMenu.SetActive(false);
        m_LeaderBoardMenu.SetActive(false);
        m_CreditMenu.SetActive(false);
    }

    void viewInputNameMenu()
    {
        m_MainMenu.SetActive(false);
        m_InputNameMenu.SetActive(true);
        m_LeaderBoardMenu.SetActive(false);
        m_CreditMenu.SetActive(false);
    }

    void viewLeaderBoardMenu()
    {
        m_MainMenu.SetActive(false);
        m_InputNameMenu.SetActive(false);
        m_LeaderBoardMenu.SetActive(true);
        m_CreditMenu.SetActive(false);
        GameObject.Find("GameManager").GetComponent<GameManager>().displayScore();
    }

    void viewCreditMenu()
    {
        m_MainMenu.SetActive(false);
        m_InputNameMenu.SetActive(false);
        m_LeaderBoardMenu.SetActive(false);
        m_CreditMenu.SetActive(true);
    }

    void startGame()
    {
        string playerName = GameObject.Find("NameInputText").GetComponent<UnityEngine.UI.Text>().text;
        GameObject.Find("GameManager").GetComponent<GameManager>().StartLevel(playerName);
    }

    void ExitGame()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().exitGame();
    }
}
