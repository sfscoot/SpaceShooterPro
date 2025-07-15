using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class Dreadnaught_Laser_Cannon : MonoBehaviour
{

    private float _zRotation = 1;
    [SerializeField] private float _zRotationIncrement = .025f;
    [SerializeField] private float _rotationSpeed = 1;
    [SerializeField] private float _fireRate = 1.25f;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private float _rotationInterval;
    private float _canFire = -1f;

    private Laser _laser;
    private bool _fireLaser = false;
    private GameObject _enemyWeapon;
    private bool _sweepAttack = false;
    Vector3 _newRotation;
    private bool _tmpFire;
    private float _tmpRotationDistance;
    // Start is called before the first frame update
    void Start()
    {
        _sweepAttack = true;
        StartCoroutine(SweepAndShoot());

    }

    // Update is called once per frame
    void Update()
    {
        /*
        transform.Rotate(new Vector3(0,0,_zRotation) * (_rotationSpeed * Time.deltaTime)); // this works - save it
        Debug.Log("z rotation " + _zRotation + " rotation distance " + _rotationSpeed * Time.deltaTime); 
        if (Time.time > _canFire)
        {
            // Debug.Log("rotation z " + transform.eulerAngles.z);
            _canFire = Time.time + _fireRate;
            _enemyWeapon = Instantiate(_laserPrefab, transform.position, Quaternion.identity); // spawn outside the collider
            // _enemyWeapon.transform.eulerAngles = Vector3.forward * transform.eulerAngles.z;
            _enemyWeapon.transform.eulerAngles = new Vector3(0,0, transform.eulerAngles.z - 90);
        }

        if (transform.eulerAngles.z > 120f)
        {
            _zRotation = -1;
        }
        if (transform.eulerAngles.z < 60)
        {
            _zRotation = 1;
        }
        */
    }

    IEnumerator SweepAndShoot()
    {
        _tmpRotationDistance = 5f;
        while (_sweepAttack)
        {
            transform.Rotate(new Vector3(0, 0, _tmpRotationDistance * _zRotation));
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
            yield return new WaitForSeconds(0.5f);
        }

    }

    private void FireLaser(float zRotation)
    {
        Debug.Log("z rotation is " +  zRotation);
        _canFire = Time.time + _fireRate;
        _enemyWeapon = Instantiate(_laserPrefab, transform.position, Quaternion.identity); // spawn outside the collider
        _enemyWeapon.transform.eulerAngles = Vector3.forward * zRotation;
    }

    /*
    IEnumerator FireLaser()
    {
        while (_fireLaser)
        {
            Debug.Log("firing the laser");
            yield return new WaitForSeconds(_fireRate);
            _enemyWeapon = Instantiate(_laserPrefab, transform.position, Quaternion.identity); // spawn outside the collider
            _enemyWeapon.tag = "EnemyLaser";
            _enemyWeapon.transform.parent = this.transform;
            Laser[] lasers = _enemyWeapon.GetComponentsInChildren<Laser>();
            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }

    }
    */

    public void StartSweepAttack()
    {
        StartCoroutine(SweepAttack());
    }

    IEnumerator SweepAttack2()
    {


        _tmpFire = true;
        while (_sweepAttack)
        {

            transform.Rotate(new Vector3(0, 0, _zRotation) * _rotationSpeed * Time.deltaTime);
            //  Debug.Log("z rotation value " + transform.eulerAngles.z);
            yield return new WaitForSeconds(_rotationInterval);

            FireLaser(_zRotation);
          

            /*
        //_enemyWeapon.transform.parent = this.transform;
        Laser[] lasers = _enemyWeapon.GetComponentsInChildren<Laser>();
        for (int j = 0; j < lasers.Length; j++)
        {
            lasers[j].AssignEnemyLaser();
        }
        */
            if (transform.eulerAngles.z > 110f)
           {
               _zRotation = -1;
            }
            if (transform.eulerAngles.z < 70)
            {
                _zRotation = 1;
            }
        }
    }


    IEnumerator SweepAttack()
    {
        _zRotation = 60.0f;
        Debug.Log("zrotation increment " + _zRotationIncrement);
        _newRotation = new Vector3(0f, 0f, _zRotation);
        

        for (int i = 0; i < 100; i++)
        {
            _zRotation = _zRotation + i;
            Debug.Log("z rotation is " + _zRotation);
            yield return new WaitForSeconds(2f);
            Debug.Log("z = " + _zRotation + " with an increment of " + _zRotationIncrement + i);

            transform.Rotate(_newRotation, Space.Self);
            _newRotation = new Vector3(0f, 0f, _zRotation + _zRotationIncrement);

            //transform.Rotate(0, 0, _zRotation, Space.Self);
            /* _zRotation += 2f;
            if (_zRotation > 110.0f || _zRotation < 60.0f) 
            { 
                
                _zRotationIncrement *= -1;
                Debug.Log("flipping the increment " + _zRotationIncrement);
            }
            */
        }
        yield return new WaitForSeconds(3.5f);

    }
}
