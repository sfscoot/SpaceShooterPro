using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialProgressBar : MonoBehaviour
{
    private bool _powerDown = false;
    private bool _powerUp    = false;
    private float _indicatorTimer;
    private float _maxIndicatorTimer;

    private Image _radialProgressBar;
    private float _thrusterChargeLevel;

    private void Awake()
    {
        _radialProgressBar = GetComponent<Image>();
    }
    void Update()
    {

        if (_powerDown == true)
        {
            _indicatorTimer -= Time.deltaTime;
            _thrusterChargeLevel = _indicatorTimer / _maxIndicatorTimer;
            // Debug.Log("Thruster charge level: " + _thrusterChargeLevel);
            _radialProgressBar.fillAmount = _indicatorTimer / _maxIndicatorTimer;
            if (_indicatorTimer <= 0)
            {
                StopCountdown();
            }
        }
        if (_powerUp == true) 
        { 
            _indicatorTimer += Time.deltaTime;
            _thrusterChargeLevel = _indicatorTimer / _maxIndicatorTimer;
            // Debug.Log("Thruster charge level: " + _thrusterChargeLevel);
            _radialProgressBar.fillAmount = _indicatorTimer / _maxIndicatorTimer;
            if (_indicatorTimer >= _maxIndicatorTimer)
            {
                StopPowerup();
            }
        }

    }

    public void scjProgBarTest (float _progress)
    {
        _radialProgressBar.fillAmount = _progress;
    }
    public void ActivateCountdown(float _countdownTime)
    {
        if (_powerDown != true)
        {
            _powerDown = true;
            _maxIndicatorTimer = _countdownTime;
            _indicatorTimer = _countdownTime;
        }
    }

    public void StopCountdown ()
    {
        _powerDown = false;
    }

    public void ActivatePowerup(float _powerupTime)
    {
        if (_powerUp != true)
        {
            _powerUp = true;
            _maxIndicatorTimer = _powerupTime;
            if ( _indicatorTimer <= 0)
            {
                _indicatorTimer = 0;
            }
        }
    }

    public float TimeLeftOnPowerDown()
    {
        return _indicatorTimer;
    }

    public void StopPowerup()
    {
        _powerUp = false;
    }

    public void InitializeThrusterChargeLevel(float _initCharge)
    {
       _thrusterChargeLevel = _initCharge;
        // Debug.Log("initializing the thruster in the progress bar " +  _thrusterChargeLevel);
    }
    public float ThrusterChargeLevel()
    {
        return _thrusterChargeLevel;
    }
}
