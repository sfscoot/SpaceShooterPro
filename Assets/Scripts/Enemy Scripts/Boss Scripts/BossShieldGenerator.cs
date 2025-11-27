using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShieldGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _explosionPreFab;
    private GameObject _explosion;
    private Boss _boss;
    private int _shieldGeneratorPowerLevel = 3;

    void Start()
    {
        _boss = GameObject.Find("Boss").GetComponent<Boss>();
        if (_boss == null) Debug.LogError("boss not found");
        if (_explosionPreFab == null) Debug.LogError("explosion prefab missing");
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "PlayerWeapon")
        {
            _boss.DamageBossShield();
            Destroy(other.gameObject);
            _shieldGeneratorPowerLevel--;
            if (_shieldGeneratorPowerLevel <= 0)
            {
                _explosion = Instantiate(_explosionPreFab, transform.position, Quaternion.identity);
                _explosion.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                Destroy(gameObject);
            }
        }
    }
}
