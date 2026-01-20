using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DreadnaughtRear : MonoBehaviour
{
    [SerializeField] private GameObject _explosionPreFab;
    private GameObject _explosion;
    private Vector3 _explosionLocation;

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
        if (_explosionPreFab == null) Debug.LogError("Explosion prefab missing in Dreadnaught Rear");
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

    public void StopSweepAttack()
    {
        _sweepAttack = false;
        StopCoroutine(SweepAndShoot());
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
    public void DreadnaughtDestruction()
    {
        StartCoroutine(DreadnaughtDestructionSequence());
    }

    IEnumerator DreadnaughtDestructionSequence()
    {
        _explosionLocation = transform.position;
        _explosionLocation.x += 0.5f;
        _explosion = Instantiate(_explosionPreFab, _explosionLocation, Quaternion.identity);
        _explosion.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
        yield return new WaitForSeconds(0.25f);

        _explosionLocation = transform.position;
        _explosionLocation.x -= 0.5f;
        _explosion = Instantiate(_explosionPreFab, _explosionLocation, Quaternion.identity);

        gameObject.SetActive(false);
        Destroy(this);
    }
}
