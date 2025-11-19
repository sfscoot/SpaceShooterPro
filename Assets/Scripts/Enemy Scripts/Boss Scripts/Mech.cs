using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Mech : MonoBehaviour
{

    private Vector3 _transformHorPos;
    private int _attackDirection = 1;

    [SerializeField] private float _vertSpeed = 1;
    [SerializeField] private float _horSpeed = .5f;
    [SerializeField] private float _attackSpeed = 1f;
    [SerializeField] private int _mechNumber;
    [SerializeField] private float _screenBottom = -5.0f;
    [SerializeField] private float _screenTop = 5.0f;
    [SerializeField] private GameObject _explosionPreFab;
    [SerializeField] private int _damagePoints = 5;

    private MechAttack _mechAttackRoutine;
    private bool _inPosition = false;
    private bool _mechAttack = false;
    private bool _mechBounceAttack = false;
    private Boss _boss;
    private GameObject _explosion;

    private void Start()
    {
        _boss = GameObject.Find("Boss").GetComponent<Boss>();
        if (_boss == null) Debug.LogError("boss not found");

        _mechAttackRoutine = GameObject.Find("MechAttack").GetComponent<MechAttack>();
        if (_mechAttackRoutine == null) Debug.LogError("MechAttack program not found");
        if (_explosionPreFab == null) Debug.LogError("Mech - explosion prefab not assigned");
    }
    private void Update()
    {
        if (_mechAttack == true)
        {
            transform.Translate(Vector3.down * Time.deltaTime * _attackSpeed);
            if (transform.position.y < -7.0f)
            {
                _mechAttack = false;
                _mechAttackRoutine.CountMechsFinishedAttacking();
            }
        }

        if (_mechBounceAttack == true) 
        {
            transform.Translate(Vector3.down * Time.deltaTime * _attackSpeed * _attackDirection);
            if (transform.position.y < _screenBottom)
            {
                _attackDirection = -1;
            }
            if (transform.position.y > _screenTop)
            {
                _attackDirection = 1;
            }
        }

    }
    public void MechMoveToPosition(float xTarget)
    {
        StartCoroutine(MechMoveToPositionCoroutine(xTarget));
    }

    IEnumerator MechMoveToPositionCoroutine(float hTarget)
    {
        _inPosition = false;
        while (_inPosition == false)
        {
                if (hTarget <= 0)
                {
                    if (transform.position.x >= hTarget && _inPosition == false)
                    {
                        _transformHorPos = new Vector3(hTarget, transform.position.y, 0);
                        transform.position = Vector3.Lerp(transform.position, _transformHorPos, Time.deltaTime * _horSpeed);
                        if (transform.position.x <= (hTarget + .1f))
                        {
                            _inPosition = true;
                            _mechAttackRoutine.CountMechsFinishedSpreading();
                        }
                    }
                }
                else
                {
                    if (transform.position.x <= hTarget && _inPosition == false)
                    {
                        _transformHorPos = new Vector3(hTarget, transform.position.y, 0);
                        transform.position = Vector3.Lerp(transform.position, _transformHorPos, Time.deltaTime * _horSpeed);
                        if (transform.position.x >= (hTarget - .1f))
                        {
                            _inPosition = true;
                            _mechAttackRoutine.CountMechsFinishedSpreading();
                    }
                    }
                }
            yield return null;
        }
    }

    public void MechAttack(float attackSpeed)
    {
        _mechAttack = true;
        _attackSpeed = attackSpeed;
    }

    public void MechBounceAttack(float attackSpeed) 
    {
        _mechBounceAttack = true;
        _mechAttack = false;
        _attackSpeed = attackSpeed;
    
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "PlayerLaser")
        {
            Destroy(other.gameObject);
            _explosion = Instantiate(_explosionPreFab, transform.position, Quaternion.identity);
            _explosion.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            _boss.BossDamage(_damagePoints);
            // _explosionAudioSource.Play();

            Destroy(this.gameObject);
        }
    }
}
