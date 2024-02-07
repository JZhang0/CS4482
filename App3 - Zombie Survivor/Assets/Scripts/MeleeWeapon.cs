using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [SerializeField] int m_weaponNo = 1;
    [SerializeField] GameObject m_player;

    private int m_weaponLevel = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void switchWeapon(int weaponNo)
    {
        m_weaponNo = weaponNo;
    }
}
