using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField]
    private float _missileSpeed = 8;
    [SerializeField]
    private float _missileDistance;
    [SerializeField]
    private float _missileTimeDuration = 4.0f;
    [SerializeField]
    private float _missileBlastRadius = 9;

    [SerializeField]
    private GameObject _explosionPreFab;

    private float _missileTimer;

    // Start is called before the first frame update
    void Start()
    {
        _missileTimer = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveUp();
        CheckTimer();
    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * _missileSpeed * Time.deltaTime);
        if (transform.position.y > 8.0f)  Destroy(gameObject);
    }

    void CheckTimer()
    {
        _missileTimer++;
        if (_missileTimer > _missileTimeDuration)
        {
            transform.GetComponent<CircleCollider2D>().enabled = true;
            transform.GetComponent<CircleCollider2D>().radius = _missileBlastRadius;
            Instantiate(_explosionPreFab, transform.position, Quaternion.identity);
            Destroy(gameObject,0.5f);
        }
    }
}
