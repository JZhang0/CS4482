using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponController : MonoBehaviour
{
    public static MeleeWeaponController Instance { get; private set; }

    [SerializeField] Sprite m_meleeWeaponSprite0;
    [SerializeField] Sprite m_meleeWeaponSprite1;
    [SerializeField] Sprite m_meleeWeaponSprite2;
    [SerializeField] GameObject m_meleeBullets;
    [SerializeField] GameObject m_meleeWeaponPrefab0;
    [SerializeField] GameObject m_meleeWeaponPrefab1;
    [SerializeField] GameObject m_meleeWeaponPrefab2;

    private readonly Vector3 m_meleeWeaponLeftPosition = new Vector3(0.2f, -0.33f, 0);
    private readonly Vector3 m_meleeWeaponRightPosition = new Vector3(-0.2f, -0.33f, 0);

    private Sprite[] m_meleeWeaponSprites;
    private int m_weaponNo = -1;
    private int m_weaponLevel = 1;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        m_meleeWeaponSprites = new Sprite[3] { m_meleeWeaponSprite0, m_meleeWeaponSprite1, m_meleeWeaponSprite2 };
    }

    // Update is called once per frame
    void Update()
    {
        shootBullets();
    }

    public void changeWeapon(int weaponNo)
    {
        if (m_weaponNo == -1)
        {
            m_weaponNo = weaponNo;
            transform.GetComponent<SpriteRenderer>().sprite = m_meleeWeaponSprites[weaponNo];
        }
    }

    public void changeDirection(bool isFacingLeft)
    {
        transform.GetComponent<SpriteRenderer>().flipX = isFacingLeft;

        if (isFacingLeft)
        {
            transform.rotation = Quaternion.Euler(0, 0, 30.0f);
            transform.position = m_meleeWeaponLeftPosition + Player.Instance.transform.position;
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, -30.0f);
            transform.position = m_meleeWeaponRightPosition + Player.Instance.transform.position;
        }
    }

    public void upgradeWeapon()
    {
        if(m_weaponLevel < 8)
        {
            m_weaponLevel++;

            foreach (Transform child in m_meleeBullets.transform)
                GameObject.Destroy(child.gameObject);
        }
    }

    private void shootBullets()
    {
        if (m_meleeBullets.transform.childCount == 0)
        {
            float angleIncrement = 360.0f / m_weaponLevel;
            switch (m_weaponNo)
            {
                case 0:
                    for (int i = 0; i < m_weaponLevel; i++)
                    {
                        GameObject bullet0 = Instantiate(m_meleeWeaponPrefab0,
                                                        new Vector3(2.0f * Mathf.Cos(Mathf.Deg2Rad * (90.0f - i * angleIncrement)), 2.0f * Mathf.Sin(Mathf.Deg2Rad * (90.0f - i * angleIncrement)), 0) + Player.Instance.transform.position,
                                                        Quaternion.Euler(0, 0, i * -angleIncrement));
                        bullet0.transform.SetParent(m_meleeBullets.transform);
                    }
                    break;
                case 1:
                    angleIncrement = angleIncrement / 2;
                    for (int i = 0; i < m_weaponLevel * 2; i++)
                    {
                        GameObject bullet1 = Instantiate(m_meleeWeaponPrefab1,
                                                        new Vector3(1.0f * Mathf.Cos(Mathf.Deg2Rad * (90.0f - i * angleIncrement)), 1.0f * Mathf.Sin(Mathf.Deg2Rad * (90.0f - i * angleIncrement)), 0) + Player.Instance.transform.position,
                                                        Quaternion.Euler(0, 0, i * -angleIncrement));
                        bullet1.transform.SetParent(m_meleeBullets.transform);
                    }
                    break;
                case 2:
                    for (int i = 0; i < m_weaponLevel; i++)
                    {
                        GameObject bullet2 = Instantiate(m_meleeWeaponPrefab2,
                                                        new Vector3(4.0f * Mathf.Cos(Mathf.Deg2Rad * (90.0f - i * angleIncrement)), 4.0f * Mathf.Sin(Mathf.Deg2Rad * (90.0f - i * angleIncrement)), 0) + Player.Instance.transform.position,
                                                        Quaternion.Euler(0, 0, i * -angleIncrement));
                        bullet2.transform.SetParent(m_meleeBullets.transform);
                    }
                    break;
            }
        }
    }
}
