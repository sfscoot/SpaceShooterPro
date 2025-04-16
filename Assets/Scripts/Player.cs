﻿using System.Collections;
// using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour

{
    // player related variables
    [SerializeField]
    private GameObject _rightEngine;
    [SerializeField]
    private GameObject _leftEngine;

    private UIManager _uiManager;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _defaultSpeed = 4.0f;
    [SerializeField]
    private float _speedMultiplier = 2;

    // laser variables
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotLaserPreFab;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField]
    private float _laserOffset;

    [SerializeField]
    private int _ammoCount = 15;

    // laser audio
    [SerializeField]
    private AudioClip _fireLaserAudio;
    [SerializeField]
    private AudioClip _explosion;
    [SerializeField]
    private AudioClip _powerUpAquiredAudio;

    [SerializeField]
    private AudioClip _ammoCountLow;
    private AudioSource _audioSource;

    [SerializeField]
    private int _lives = 3;

    // screen bounding variables
    private float _rightBound = 11.3f;
    private float _leftBound = -11.44f;
    private float _upBound = 0f;
    private float _downBound = -3.8f;

    private SpawnManager _spawnManager;

    float _horizontalInput;
    float _verticalInput;
    private Vector3 _direction;

    // powerup variables
    [SerializeField]
    bool _tripleShotActive = false;
    [SerializeField]
    bool _shieldPowerupActive = false;

    [SerializeField]
    int _shieldMaxLevel = 4;
    int _shieldLevel;

    [SerializeField]
    private StatusEffectManager _statusEffectManager;

    // variable reference to the shield object
    [SerializeField]
    private GameObject _shieldVisualizer;
    SpriteRenderer _shieldRenderer;
    private Vector3 _shield100pct = new Vector3(2, 2, 2);
    private Vector3 _shield75pct = new Vector3(1.75f, 1.75f, 1.75f);
    private Vector3 _shield50pct = new Vector3(1.5f, 1.5f, 1.5f);
    private Vector3 _shield25pct = new Vector3(1.25f, 1.25f, 1.25f);


    [SerializeField]
    private int _score;

    // explosion audio
    [SerializeField]
    AudioSource _externalAudioSource;
    [SerializeField]
    private AudioClip _explosionClip;


    // thruster vars
    // progress bar variables

    [SerializeField]
    private GameObject _thrusterVisualizer;

    private RadialProgressBar _radialProgressBar;
    private Image _thrusterProgressBarImage;
    // private TMP_Text _thrusterProgressBarText;
    private bool _canActivateThruster = true;
    private bool _thrusterActive = false;


    [SerializeField]
    private float _thrusterMaxChargeLevel;
    [SerializeField]
    private float _thrusterMinChargeLevel;
    [SerializeField]
    private float _thrusterDischargeRate = 1.0f;
    [SerializeField]
    private float _thrusterRechargeRate = 0.5f;
    private float _thrusterChargeLevel;

    private float _canOverrideSpeed = -1;
    [SerializeField]
    private float _thrusterDuration = 5.0f;
    [SerializeField]
    private float _thrusterReloadDuration = 10.0f;

    private float _timeLeftOnThruster;

    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        _thrusterProgressBarImage = GameObject.Find("RadialProgressBar").GetComponent<Image>();
        _shieldRenderer = _shieldVisualizer.GetComponent<SpriteRenderer>();

        InitializeThrusters();

        if (_spawnManager == null)
        {
            Debug.Log("spawn manager reference failed");
        }

        if (_uiManager == null)
        {
            Debug.Log("UI manager not found");
        }
        
        if ( _audioSource == null)
        {
            Debug.LogError("Player Audio Source is missing");
        } else
        {
            _audioSource.clip = _fireLaserAudio;
        }
        if (_shieldVisualizer == null)
        {
            Debug.LogError("shield visualizer missing");
        }

        if (_thrusterProgressBarImage == null) 
        {
            Debug.LogError("Thruster progress bar not found");
        }
        if (_thrusterVisualizer == null) 
        {
            Debug.LogError("Thruster visualizer not found");
        }
    }

    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire && _ammoCount > 0)
        {
            FireLaser();
        }

        CheckForThruster();
    }
    //***************************************************************************
    // Player Movement Code
    //***************************************************************************
    void CalculateMovement()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        _direction = new Vector3(_horizontalInput, _verticalInput, 0);
        transform.Translate(_direction * _speed * Time.deltaTime);

        // check horizontal boundaries - wrap player
        if (transform.position.x > _rightBound)
        {
            transform.position = new Vector3(_leftBound, transform.position.y, 0);
        }
        else if (transform.position.x < _leftBound)
        {
            transform.position = new Vector3(_rightBound, transform.position.y, 0);
        }

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, _downBound, _upBound), 0);
    }

    //***************************************************************************
    // Laser Code
    //***************************************************************************
    void FireLaser()
    {
        _canFire = Time.time + _fireRate;
        if (_tripleShotActive == true && _ammoCount >= 3)
        {
            Instantiate(_tripleShotLaserPreFab, transform.position, Quaternion.identity);
            _ammoCount -= 3;
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, _laserOffset, 0), Quaternion.identity);
            _ammoCount--;
        }
        _uiManager.UpdateAmmoCount(_ammoCount);
        _audioSource.Play();
        if (_ammoCount <= 3)
        {
            Debug.Log("playing low ammo count warning");
            _externalAudioSource.clip = _ammoCountLow;
            _externalAudioSource.Play();
            _uiManager.SetLowAmmoWarning(true);
        } else
        {
            _uiManager.SetLowAmmoWarning(false);
        }
    }

    public void TripleShotActive()
    {
        _tripleShotActive = true;
        StartCoroutine(TripleShotPowerDown());
    }

    IEnumerator TripleShotPowerDown()
    {
        yield return new WaitForSeconds(5);
        _tripleShotActive = false;
    }

    public void AmmoReload()
    {
        _ammoCount += 15;
        _uiManager.UpdateAmmoCount(_ammoCount);
        _uiManager.SetLowAmmoWarning(false);
    }
    //***************************************************************************
    // Thruster Code
    //***************************************************************************
    void CheckForThruster()
    {
        // activate & deactivate Thruster

        if (Input.GetKey(KeyCode.LeftShift) && _canActivateThruster)
        {
            _thrusterActive = true;
            _thrusterVisualizer.SetActive(true);
            _speed = _defaultSpeed * _speedMultiplier;     
            
            if (_thrusterChargeLevel <= 0)
            {
                _thrusterActive = false;
                _thrusterVisualizer.SetActive(false);
                _canActivateThruster = false;
                _speed = _defaultSpeed;
            }
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) && _thrusterActive)
        {
            _thrusterActive = false;
            _thrusterVisualizer.SetActive(false);
            _speed = _defaultSpeed;
        }


        // manage Thruster burndown and recharge

        if (_thrusterActive == true && _thrusterMaxChargeLevel > 0) 
        {
            _thrusterChargeLevel -= _thrusterDischargeRate * Time.deltaTime;
        } else if (_thrusterActive == false &&  _thrusterChargeLevel < _thrusterMaxChargeLevel)
        {
            _thrusterChargeLevel += _thrusterRechargeRate * Time.deltaTime;
        }

        // check to see if Thrusters can be activated
        if (_canActivateThruster == false && _thrusterChargeLevel >= _thrusterMinChargeLevel)
        {
            _canActivateThruster = true;
        }

        // update the thruster visual
        _thrusterProgressBarImage.fillAmount = _thrusterChargeLevel/_thrusterMaxChargeLevel;

        _uiManager.UpdateThrusterPowerLevel(_thrusterChargeLevel, _thrusterActive, _thrusterMaxChargeLevel);
    }

    void InitializeThrusters()
    {
        _thrusterChargeLevel = _thrusterMaxChargeLevel;
    }

    //**********************************************************************************
    //*  Damage/Lives management
    //**********************************************************************************
    public void Damage()
    {
        _externalAudioSource.clip = _explosionClip;
        _externalAudioSource.Play();

        if (_shieldPowerupActive == true)
        {
            _shieldLevel--;
            switch (_shieldLevel)
            {
                case 3:
                    _shieldVisualizer.transform.localScale = _shield75pct;
                    break;
                case 2:
                    _shieldVisualizer.transform.localScale = _shield50pct;
                    break;
                case 1:
                    //_shieldRenderer.color = Color.red;
                    _shieldVisualizer.transform.localScale = _shield25pct;
                    break;
                case 0:
                    _shieldPowerupActive = false;
                    _shieldVisualizer.SetActive(false);
                    break;
                default:
                    Debug.LogError("unknown shield level encountered" + _shieldLevel);
                    break;
            }
            return;
        }

        _lives--;

        if (_lives == 2)
        {
            _rightEngine.SetActive(true);
        } else if (_lives == 1)
        {
            _leftEngine.SetActive(true);
        }

        _uiManager.UpdateLivesImage(_lives);

        if (_lives == 0)
        {
            _spawnManager.OnPlayerDeath();
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            Destroy(gameObject, 0.1f);
        }
    }

    public void SpeedPowerupActive()
    {
        _speed = _defaultSpeed * _speedMultiplier; 
        StartCoroutine(SpeedPowerupPowerDown());
    }

    IEnumerator SpeedPowerupPowerDown()
    {
        yield return new WaitForSeconds(5);
        _speed = _defaultSpeed;
    }

    public void ShieldPowerupActive()
    {
        _shieldPowerupActive = true;
        _shieldVisualizer.SetActive(true);
        _shieldLevel = _shieldMaxLevel;
        _shieldVisualizer.transform.localScale = _shield100pct;
    }

    public void AddToScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
}
