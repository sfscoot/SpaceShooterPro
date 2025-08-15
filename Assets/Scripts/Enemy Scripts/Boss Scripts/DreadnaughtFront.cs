using System.Collections;
using System.Collections.Generic;
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
    void Start()
    {
        _laserCannons = transform.GetComponentsInChildren<DreadnaughtLaserCannon>();
        _sweepAttack = true;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartCoroutine(SweepAndShoot());
        }
    }
    IEnumerator SweepAndShoot()
    {
        while (_sweepAttack)
        {
            _eulerZ = Mathf.PingPong(Time.time * _rotationSpeed, _minSweepAngle + _maxSweepAngle) - (_minSweepAngle + _maxSweepAngle) / 2;
            foreach (DreadnaughtLaserCannon dlc in _laserCannons)
            {

                // scj -good dlc.transform.eulerAngles = new Vector3(0,0,_eulerZ);
                dlc.NewRotateLaserCannon(_eulerZ);
            }
            yield return null;
        }
    }
}
