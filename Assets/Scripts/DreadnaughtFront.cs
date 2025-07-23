using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreadnaughtFront : MonoBehaviour
{
    [SerializeField] private GameObject _laserPrefab;
    private GameObject _enemyWeapon;
    [SerializeField] private float _rotationDistance = 5.0f;
    private bool _sweepAttack = false;
    private float _zRotation = 1;
    private DreadnaughtLaserCannon[] _laserCannons;
    void Start()
    {
        _laserCannons = transform.GetComponentsInChildren<DreadnaughtLaserCannon>();
        _sweepAttack = true;

    }

    // Update is called once per frame
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
            foreach (DreadnaughtLaserCannon dlc in _laserCannons)
            {
                dlc.RotateLaserCannon(_rotationDistance * _zRotation, _zRotation);
                // dlc.transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - 90);
                if (dlc.transform.eulerAngles.z > 120f)
                {
                    foreach(DreadnaughtLaserCannon tmpdlc in _laserCannons)
                    {
                        tmpdlc.ResetLaserCannonRotation(120f);
                    }
                    _zRotation = -1;
                }
                if (dlc.transform.eulerAngles.z < 60)
                {
                    foreach (DreadnaughtLaserCannon tmpdlc in _laserCannons)
                    {
                        tmpdlc.ResetLaserCannonRotation(60f);
                    }
                    _zRotation = 1;
                }
                yield return null;
            }

             yield return new WaitForSeconds(0.1f);
        }


        /*
        _rotationDistance = 5f;
        while (_sweepAttack)
        {
            transform.Rotate(new Vector3(0, 0, _rotationDistance * _zRotation));
            _enemyWeapon = Instantiate(_laserPrefab, transform.position, Quaternion.identity); // spawn outside the collider
            // _enemyWeapon.transform.eulerAngles = Vector3.forward * transform.eulerAngles.z;
            _enemyWeapon.transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - 90);

            if (transform.eulerAngles.z > 120f)
            {
                _zRotation = -1;
            }
            if (transform.eulerAngles.z < 60)
            {
                _zRotation = 1;
            }
        }
        */

    }
}
