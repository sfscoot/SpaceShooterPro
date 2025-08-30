using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DreadnaughtFront : MonoBehaviour
{
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private float _rotationDistance = 5.0f;
    private bool _sweepAttack = false;
    private DreadnaughtLaserCannon[] _laserCannons;
    [SerializeField] private float _minSweepAngle = 30;
    [SerializeField] private float _maxSweepAngle = 60;
    private float _eulerZ;
    [SerializeField] private float _rotationSpeed = .25f;
    private Boss _bossScript;

    void Start()
    {
        _laserCannons = transform.GetComponentsInChildren<DreadnaughtLaserCannon>();
        _sweepAttack = true;
        _bossScript = GetComponentInParent<Boss>();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartCoroutine(SweepAndShoot());
        }
    }

    public void SweepAttack()
    {
        _sweepAttack = true;
        StartCoroutine(SweepAndShoot());
    }
    IEnumerator SweepAndShoot()
    {
        while (_sweepAttack)
        {
            _laserCannons = transform.GetComponentsInChildren<DreadnaughtLaserCannon>();
            _eulerZ = Mathf.PingPong(Time.time * _rotationSpeed, _minSweepAngle + _maxSweepAngle) - (_minSweepAngle + _maxSweepAngle) / 2;
            if (_laserCannons.Length > 0)
            {
                foreach (DreadnaughtLaserCannon dlc in _laserCannons)
                {
                    dlc.NewRotateLaserCannon(_eulerZ);
                }
                yield return null;
            }
            else
            {
                _sweepAttack = false;
                _bossScript.DreadnaughtGunsDead("Front");
                
            }
        }
    }
}
