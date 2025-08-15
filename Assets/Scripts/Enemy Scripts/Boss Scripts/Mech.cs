using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mech : MonoBehaviour
{

    private Vector3 _transformVertPos;
    private Vector3 _transformHorPos;
    private float _tmpYPos;
    private float _tmpXPos;
    [SerializeField] private float _vertSpeed = 1;
    [SerializeField] private float _horSpeed = .5f;

    private bool _inPosition = false;
    private bool _mechAttack = false;
    private void Start()
    {
        //transform.position = new Vector3(0f, 0f, 0f);
        // Debug.Log("put mech on screen");
    }
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.D))
        {
            _mechAttack = true;
        }



        if (_mechAttack == true)
        {
            transform.position = Vector3.down * Time.deltaTime;
        }

    }

    public void MechAppear(float hTarget, float vTarget)
    {
        StartCoroutine(MechsLineup(hTarget, vTarget));
    }
    IEnumerator MechsLineup(float hTarget, float vTarget)
    {
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
                        // _tmpXPos = transform.position.x - _horShift;
                        _transformHorPos = new Vector3(hTarget, transform.position.y, 0);
                        transform.position = Vector3.Lerp(transform.position, _transformHorPos, Time.deltaTime * _horSpeed);
                        if (transform.position.x <= (hTarget + .1f))
                        {
                            Debug.Log("mech in position");
                            _inPosition = true;
                        }
                    }
                } else
                {
                    if (transform.position.x <= hTarget && _inPosition == false)
                    {
                        //_tmpXPos = transform.position.x - _horShift;
                        _transformHorPos = new Vector3(hTarget, transform.position.y, 0);
                        transform.position = Vector3.Lerp(transform.position, _transformHorPos, Time.deltaTime * _horSpeed);
                        if (transform.position.x >= (hTarget - .1f))
                        {
                            Debug.Log("mech in position");
                            _inPosition = true;
                        }
                    }
                }
            }
            yield return null;
        }
    }

    public void MechAttack()
    {
        _mechAttack = true;
    }

/*
    public void MechsAppear(float hTarget)
    {
        if (transform.position.y >= _vertTarget)
        {
            _tmpYPos = transform.position.y - _vertDrop;
            _transformVertPos = new Vector3(transform.position.x, _tmpYPos, 0);
            transform.position = Vector3.Lerp(transform.position, _transformVertPos, Time.deltaTime * _vertSpeed);
        }

        if (transform.position.y <= _vertTarget)
        {
            Debug.Log($"htarget is {hTarget} and transform position x is {transform.position.x}");
            if (transform.position.x >= hTarget)
            {
                _tmpXPos = transform.position.x - _horShift;
                _transformHorPos = new Vector3(_tmpXPos, transform.position.y, 0);
                transform.position = Vector3.Lerp(transform.position, _transformHorPos, Time.deltaTime * _horSpeed);
                _inPosition = true;
            }
        }
    }

*/
}
