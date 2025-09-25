using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Mech : MonoBehaviour
{

    private Vector3 _transformVertPos;
    private Vector3 _transformVertResetPos;
    private Vector3 _transformHorPos;
    private float _tmpYPos;
    private float _inPosX;
    private float _inPosY;
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

    private float _horizonalStartPos;
    private float _verticalStartPos;


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
    public void MechReset()
    {
        StartCoroutine(MechReLineup());
    }
    IEnumerator MechReLineup()  // just the vertical alignment - horizonal has already been done
    {
        _inPosition = false;
        _tmpYPos = _inPosY * -1;
        _transformVertResetPos = new Vector3(transform.position.x, _tmpYPos, 0);
        while (_inPosition == false)
        {

            if (this._mechNumber == 3) 
            {
                Debug.Log($"mech number {this._mechNumber}");
                Debug.Log($"_inPosY is {_inPosY}");
                Debug.Log($"transform.position.y is {transform.position.y}");
                Debug.Log($"vertspeed is {_vertSpeed}");
                //_tmpYPos = transform.position.y - _inPosY;
                Debug.Log($"tmpYPos is {_tmpYPos}");
            } 

            if (transform.position.y >= _inPosY)
            {
                // _tmpYPos = transform.position.y - _inPosY;
                // Debug.Log("tmpYPos is " + _tmpYPos);
                // _transformVertPos = new Vector3(transform.position.x, _tmpYPos, 0);
                transform.position = Vector3.Lerp(transform.position, _transformVertResetPos, Time.deltaTime * (_vertSpeed/3));
            }
            if (transform.position.y <= _inPosY) _inPosition = true;
            
        }
        yield return null;
    }
    /*
    public void MechLineUp(float hTarget, float vTarget)
    {
        StartCoroutine(MechsLineup(hTarget, vTarget));
    }
    */
    
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
                            _inPosX = transform.position.x;
                            _inPosY = transform.position.y;
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
                            _inPosX = transform.position.x;
                            _inPosY = transform.position.y;
                            _inPosition = true;
                            _mechAttackRoutine.CountMechsFinishedSpreading();
                    }
                    }
                }
            yield return null;
        }
    }
    
    IEnumerator MechsLineup(float hTarget, float vTarget)
    {
        Debug.Log("Mechs.cs mechslineup");
        _horizonalStartPos = hTarget;
        while (_inPosition == false)
        {
            if (transform.position.y >= vTarget)
            {
                _tmpYPos = transform.position.y - vTarget;
                _transformVertPos = new Vector3(transform.position.x, _tmpYPos, 0);
                transform.position = Vector3.Lerp(transform.position, _transformVertPos, Time.deltaTime * _vertSpeed);
            }

            if (transform.position.y <= vTarget)
            {
                if (hTarget <= 0)
                {
                    if (transform.position.x >= hTarget && _inPosition == false)
                    {
                        _transformHorPos = new Vector3(hTarget, transform.position.y, 0);
                        transform.position = Vector3.Lerp(transform.position, _transformHorPos, Time.deltaTime * _horSpeed);
                        if (transform.position.x <= (hTarget + .1f))
                        {
                            _inPosX = transform.position.x;
                            _inPosY = transform.position.y;
                            _inPosition = true;
                        }
                    }
                } else
                {
                    if (transform.position.x <= hTarget && _inPosition == false)
                    {
                        _transformHorPos = new Vector3(hTarget, transform.position.y, 0);
                        transform.position = Vector3.Lerp(transform.position, _transformHorPos, Time.deltaTime * _horSpeed);
                        if (transform.position.x >= (hTarget - .1f))
                        {
                            _inPosition = true;
                            _inPosX = transform.position.x;
                            _inPosY = transform.position.y;
                        }
                    }
                }
            }
            yield return null;
        }
    }

    IEnumerator MechDropToPosition(float vTarget)
    {
        Debug.Log("Mechs.cs mechdrop coroutine");
        while (_inPosition == false)
        {
            if (transform.position.y >= vTarget)
            {
                _tmpYPos = transform.position.y - vTarget;
                _transformVertPos = new Vector3(transform.position.x, _tmpYPos, 0);
                transform.position = Vector3.Lerp(transform.position, _transformVertPos, Time.deltaTime * _vertSpeed);
            }
            yield return null;
            if (transform.position.y <= vTarget) _inPosition = true;
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
