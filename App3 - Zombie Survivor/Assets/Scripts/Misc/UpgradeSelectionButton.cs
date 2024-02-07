using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSelectionButton : MonoBehaviour
{
    private int m_weaponNo;
    private bool m_isMeleeWeapon;
    private bool m_isHealth;

    public void initialize(int weaponNo, bool isMeleeWeapon = true, bool isHealth = false)
    {
        m_weaponNo = weaponNo;
        m_isMeleeWeapon = isMeleeWeapon;
        m_isHealth = isHealth;
    }

    private void notify()
    {
        ProgressTracker.Instance.makeSelection(m_weaponNo, m_isMeleeWeapon, m_isHealth);
    }
}
