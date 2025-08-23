﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DreadnaughtRear : MonoBehaviour
{
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private float _rotationDistance = 4.0f;
    private bool _sweepAttack = false;
    private DreadnaughtMissileLauncher[] _missileLaunchers;
    [SerializeField] private float _minSweepAngle = 20;
    [SerializeField] private float _maxSweepAngle = 40;
    private float _eulerZ;
    [SerializeField] private float _rotationSpeed = 50f;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _fireDelayTime;
    private bool _delayFire;
    private Boss _bossScript;
    void Start()
    {
        _missileLaunchers = transform.GetComponentsInChildren<DreadnaughtMissileLauncher>();
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
        _delayFire = false;
        while (_sweepAttack)
        {
            _missileLaunchers = transform.GetComponentsInChildren<DreadnaughtMissileLauncher>();
            _eulerZ = Mathf.PingPong(Time.time * _rotationSpeed, _minSweepAngle + _maxSweepAngle) - (_minSweepAngle + _maxSweepAngle) / 2;
            if (_missileLaunchers.Length > 0)
            {
                foreach (DreadnaughtMissileLauncher dml in _missileLaunchers)
                {
                    if (_delayFire)
                    {
                        dml.RotateMissileLauncher(_eulerZ, _fireRate, _fireDelayTime);
                    }
                    else
                    {
                        dml.RotateMissileLauncher(_eulerZ, _fireRate, 0);
                    }
                    _delayFire = !_delayFire;
                }
            }
            else
            {
                _sweepAttack = false;
                _bossScript.DreadnaughtGunsDead("Rear");
            }
            yield return null;
        }
    }
}
