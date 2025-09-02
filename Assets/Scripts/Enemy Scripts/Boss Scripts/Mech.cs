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
    [SerializeField] private float _vertSpeed = 1;
    [SerializeField] private float _horSpeed = .5f;
    [SerializeField] private float _attackSpeed = 1f;
    [SerializeField] private int _mechNumber;
    private float _horizonalStartPos;
    private float _verticalStartPos;


    private bool _inPosition = false;
    private bool _mechAttack1 = false;
    private void Start()
    {
        //transform.position = new Vector3(0f, 0f, 0f);
        // Debug.Log("put mech on screen");
    }
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.D))
        {
            // _mechAttack1 = true;
        }

        if (_mechAttack1 == true)
        {
            transform.Translate(Vector3.down * Time.deltaTime * _attackSpeed);
            if (transform.position.y < -13.0f)
            {
                transform.position = new Vector3(_inPosX, 6.5f, 0);  // - 2.8 -9
                _mechAttack1 = false;
            }
        }
    }

    public void MechSpreadToPosition(float xTarget)
    {
        StartCoroutine(MechSpreadToPostionCoroutine(xTarget));
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
    public void MechLineUp(float hTarget, float vTarget)
    {
        StartCoroutine(MechsLineup(hTarget, vTarget));
    }


    IEnumerator MechSpreadToPostionCoroutine(float hTarget)
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
                            _inPosX = transform.position.x;
                            _inPosY = transform.position.y;
                        }
                    }
                }
            yield return null;
        }
    }
    IEnumerator MechsLineup(float hTarget, float vTarget)
    {
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


    public void MechDrop(float vTarget)
    {
        StartCoroutine(MechDropToPosition(vTarget));
    }
    IEnumerator MechDropToPosition(float vTarget)
    {
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
    public void MechAttack1(float attackSpeed)
    {
        _mechAttack1 = true;
        _attackSpeed = attackSpeed;
    }
}
