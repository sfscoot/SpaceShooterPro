using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    Vector3 _originalPosition;
    [SerializeField] float _shakeAmount = 0.7f;
    [SerializeField] float _shakeDuration = 0.0f;
    [SerializeField] float _decreaseFactor = 1.0f;
    bool _shakeCamera;

    void Start()
    {
        _originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if ( _shakeCamera )
        {
            if (_shakeDuration > 0.0f )
            {
                transform.localPosition = _originalPosition + Random.insideUnitSphere * _shakeAmount;
                _shakeDuration -= Time.deltaTime * _decreaseFactor;
            } else
            {
                _shakeDuration = 1.0f;
                transform.localPosition = _originalPosition;
                _shakeCamera = false;
            }
        }
    }

    public void ShakeCamera()
    {
        _originalPosition = transform.position;
        _shakeCamera = true;
    }
}
