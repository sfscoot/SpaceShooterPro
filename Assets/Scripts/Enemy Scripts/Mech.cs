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
    [SerializeField] private float _vertDrop = 3f;
    [SerializeField] private float _horShift = 8f;
    [SerializeField] private float _horTarget = -9f;
    [SerializeField] private float _vertTarget = 3f;
    private bool _mechAppearOn = false;

    private void Start()
    {
        //transform.position = new Vector3(0f, 5.8f, 0f);
        // Debug.Log("put mech on screen");
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _mechAppearOn = true;    
        }

        if (_mechAppearOn) 
        { 
            MechsAppear();
        }

    }
    public void MechsAppear()
    {
        if (transform.position.y >= _vertTarget)
        {
            _tmpYPos = transform.position.y - _vertDrop;
            _transformVertPos = new Vector3(transform.position.x, _tmpYPos, 0);
            transform.position = Vector3.Lerp(transform.position, _transformVertPos, Time.deltaTime * _vertSpeed);
        }
        
        if (transform.position.y <= _vertTarget)
        {
            if (transform.position.x >= _horTarget)
            {
                _tmpXPos = transform.position.x - _horShift;
                _transformHorPos = new Vector3(_tmpXPos, transform.position.y, 0);
                transform.position = Vector3.Lerp(transform.position, _transformHorPos, Time.deltaTime * _horSpeed);
            }
        }
    }

    public void SetVertHorTargets(float vertTarget, float horTarget)
    {
        _vertTarget = vertTarget;
        _horTarget = horTarget;
    }
}
