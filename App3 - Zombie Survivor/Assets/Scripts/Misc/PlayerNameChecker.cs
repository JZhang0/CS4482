using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameChecker : MonoBehaviour
{
    [SerializeField] GameObject m_InputNameMenu;

    void validateName()
    {
        string characterName = GetComponent<InputField>().text;
        if (characterName.Length >= 3)
        {
            m_InputNameMenu.transform.Find("SubmitNameButton").GetComponent<Button>().interactable = true;
        }
        else
        {
            m_InputNameMenu.transform.Find("SubmitNameButton").GetComponent<Button>().interactable = false;
        }
    }
}
