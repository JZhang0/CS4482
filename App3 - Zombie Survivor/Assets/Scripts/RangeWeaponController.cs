using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeWeaponController : MonoBehaviour
{
    public static RangeWeaponController Instance { get; private set; }

    [SerializeField] Sprite m_rangeWeaponSprite0;
    [SerializeField] Sprite m_rangeWeaponSprite1;
    [SerializeField] Sprite m_rangeWeaponSprite2;
    [SerializeField] GameObject m_rangeBullets;
    [SerializeField] GameObject m_rangeWeaponPrefab0;
    [SerializeField] GameObject m_rangeWeaponPrefab1;
    [SerializeField] GameObject m_rangeWeaponPrefab2;

    private readonly Vector3 m_rangeWeaponLeftPosition = new Vector3(-0.25f, -0.2f, 0);
    private readonly Vector3 m_rangeWeaponRightPosition = new Vector3(0.25f, -0.2f, 0);

    private Sprite[] m_rangeWeaponSprites;
    private int m_weaponNo = -1;
    private int m_weaponLevel = 1;
    private int m_facingDirection = 1;
    private float m_shootInterval;
    private float m_timeBeforeShoot;
    private float m_RB1ShootingAngle = 0.0f;

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
        m_rangeWeaponSprites = new Sprite[3] { m_rangeWeaponSprite0, m_rangeWeaponSprite1, m_rangeWeaponSprite2 };
    }

    // Update is called once per frame
    void Update()
    {
        if (m_timeBeforeShoot >= 0)
        {
            m_timeBeforeShoot -= Time.deltaTime;
        }
        else if(m_weaponNo != -1)
        {
            shootBullets();
            m_timeBeforeShoot = m_shootInterval;
        }

        m_RB1ShootingAngle += 90 * Time.deltaTime;

        if(m_weaponNo == 2)
        {
            Vector3 pos = Input.mousePosition;
            pos.z = 10.0f;
            Vector2 mouseScreenPosition = Camera.main.ScreenToWorldPoint(pos);
            Vector2 direction = mouseScreenPosition - (Vector2)Player.Instance.transform.position;
            transform.right = direction;

            if(transform.rotation.eulerAngles.z >= 90.0f && transform.rotation.eulerAngles.z <= 270.0f)
            {
                transform.GetComponent<SpriteRenderer>().flipY = true;
                transform.position = m_rangeWeaponLeftPosition + Player.Instance.transform.position;
            }
            else
            {
                transform.GetComponent<SpriteRenderer>().flipY = false;
                transform.position = m_rangeWeaponRightPosition + Player.Instance.transform.position;
            }
        }
    }

    public void changeWeapon(int weaponNo)
    {
        m_weaponNo = weaponNo;
        transform.GetComponent<SpriteRenderer>().sprite = m_rangeWeaponSprites[weaponNo];
        switch (weaponNo)
        {
            case 0:
                m_shootInterval = 1.0f;
                break;
            case 1:
                m_shootInterval = 1.0f;
                break;
            case 2:
                m_shootInterval = 1.5f;
                break;
        }
        m_timeBeforeShoot = m_shootInterval;
    }

    public void changeDirection(bool isFacingLeft)
    {
        if (m_weaponNo != 2)
        {
            transform.GetComponent<SpriteRenderer>().flipX = isFacingLeft;
        }
        
        m_facingDirection = isFacingLeft ? -1 : 1;
        if (isFacingLeft)
            transform.position = m_rangeWeaponLeftPosition + Player.Instance.transform.position;
        else
            transform.position = m_rangeWeaponRightPosition + Player.Instance.transform.position;
    }

    public void upgradeWeapon()
    {
        if (m_weaponLevel < 8)
        {
            m_weaponLevel++;
            switch (m_weaponNo)
            {
                case 0:
                    m_shootInterval -= 1.0f / 8;
                    break;
                case 1:
                    m_shootInterval = 1.0f / m_weaponLevel;
                    break;
                case 2:
                    m_shootInterval = 1.5f / m_weaponLevel;
                    break;
            }
            m_timeBeforeShoot = m_shootInterval;
        }
    }

    private void shootBullets()
    {
        //float angleIncrement = 360.0f / m_weaponLevel;
        switch (m_weaponNo)
        {
            case 0:
                GameObject bullet0 = Instantiate(m_rangeWeaponPrefab0,
                                                new Vector3(m_facingDirection * 1.5f, -0.1f, 0.0f) + transform.position,
                                                Quaternion.Euler(0, 0, -m_facingDirection * 90.0f));
                bullet0.transform.SetParent(m_rangeBullets.transform);
                break;
            case 1:
                float angleIncrement = 360.0f / m_weaponLevel;
                for (int i = 0; i < m_weaponLevel + 2; i++)
                {
                    GameObject bullet1 = Instantiate(m_rangeWeaponPrefab1);
                    bullet1.transform.position = new Vector3(1.5f * Mathf.Sin(Mathf.Deg2Rad * (i * angleIncrement + m_RB1ShootingAngle)), 1.5f * Mathf.Cos(Mathf.Deg2Rad * (i * angleIncrement + m_RB1ShootingAngle)) - 0.1f, 0.0f) + transform.position;
                    bullet1.transform.rotation = Quaternion.Euler(0, 0, -(i * angleIncrement + m_RB1ShootingAngle));
                    bullet1.transform.SetParent(m_rangeBullets.transform);
                }
                break;
            case 2:
                float angleBetweenBullets = 15.0f - (m_weaponLevel - 1);
                float scatterAngle = (m_weaponLevel + 2) * angleBetweenBullets;
                float scatterStartAngle = transform.rotation.eulerAngles.z + scatterAngle / 2;
                /**
                 *  Level:      1   2   3   4   5   6   7   8
                 *  Bullets:    3   4   5   6   7   8   9   10
                 *  ABB:        15  14  13  12  11  10  9   8
                 *  SA:         30  42  52  60  66  70  72  70
                 */
                for (int i = 0; i < m_weaponLevel + 2; i++)
                {
                    GameObject bullet2 = Instantiate(m_rangeWeaponPrefab2);
                    bullet2.transform.position = new Vector3(1.5f * Mathf.Cos(Mathf.Deg2Rad * (scatterStartAngle - i * angleBetweenBullets)), 1.5f * Mathf.Sin(Mathf.Deg2Rad * (scatterStartAngle - i * angleBetweenBullets)) - 0.1f, 0.0f) + transform.position;
                    bullet2.transform.up = bullet2.transform.position - transform.position;
                    bullet2.transform.SetParent(m_rangeBullets.transform);
                }
                break;
        }
    }
}
