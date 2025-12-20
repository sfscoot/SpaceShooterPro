using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private CameraShake _cameraShake;

    [Header ("Player Variables")]
    // player related variables
    [SerializeField] private GameObject _rightEngine;
    [SerializeField] private GameObject _leftEngine;
    [SerializeField] private float _speed;
    [SerializeField] private float _defaultSpeed = 4.0f;
    
    
    private Animator _playerAnimator;
    private UIManager _uiManager;


    // Weapon variables
    [Header("Weapon Variables")]
    [SerializeField] private char _activeWeapon;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShotLaserPreFab;
    [SerializeField] private GameObject _playerHomingMissilePreFab;
    private GameObject _tmpLaser;
    [SerializeField] private float _laserOffset;
    [SerializeField] private int _ammoCount = 16;
    private bool _weaponsEnabled = true;
    [SerializeField] private float _fireRate = 0.5f;
    private float _baseFireRate;
    private float _canFire = -1f;
    [SerializeField] int _ammoReloadAmount;

    // Missile variables
    [Header("Missile and Mine Variables")]
    [SerializeField] private GameObject _missilePreFab;
    [SerializeField] private float _missileOffset;
    [SerializeField] private float _missilePowerupDuration;
    bool _missilePowerupActive = false;
    bool _homingMissileActive = false;
    Collider2D _tmp2DCollider;

    // space mine variables
    [SerializeField] private GameObject _spaceMinePreFab;
    [SerializeField] private int _fireAngle;

    private bool _mineLauncherActive = false;
    private GameObject[] _activePowerups;



    [SerializeField] private float _canCollectPowerupsCoolDown = 20.0f;
    private float _canCollectPowerups = 12f;

    // laser audio
    [Header("Audio Objects and Variables")]
    [SerializeField] private AudioClip _fireLaserAudio;
    [SerializeField] private AudioClip _explosion;
    [SerializeField] private AudioClip _ammoCountLow;
    [SerializeField] private AudioClip _homingMissileMisfireAudio;
    private AudioSource _audioSource; // scj

    [SerializeField] private int _lives = 3;
    private GameObject[] _activeEnemies;
    private GameObject _closestEnemy;
    private float _distanceToClosestEnemy = 100;
    private float _enemyDistanceFromPlayer;
    private Transform _enemyTransform;
    private GameObject _homingMissile;

    // screen bounding variables
    private float _rightBound = 11.3f;
    private float _leftBound = -11.44f;
    private float _upBound = 0f;
    private float _downBound = -3.8f;

    private SpawnManager _spawnManager;

    float _horizontalInput;
    float _verticalInput;
    private Vector3 _direction;
    private int _directionModifier;

    // powerup variables
    [SerializeField] bool _tripleShotActive = false;
    [SerializeField] bool _shieldPowerupActive = false;
    [SerializeField] int _leftRightSwapDuration;
    private bool _speedPowerupOn = false;
    
    private Transform[] _laserTransforms;


    [SerializeField] int _shieldMaxLevel = 4;
    int _shieldLevel;

    [SerializeField]
    private StatusEffectManager _statusEffectManager;

    // variable reference to the shield object
    [SerializeField]
    private GameObject _shieldVisualizer;
    SpriteRenderer _shieldRenderer;
    private Vector3 _shield100pct = new Vector3(2, 2, 2);
    private Vector3 _shield75pct = new Vector3(1.65f, 1.65f, 1.65f);
    private Vector3 _shield50pct = new Vector3(1.5f, 1.5f, 1.5f);
    private Vector3 _shield25pct = new Vector3(1.25f, 1.25f, 1.25f);


    [SerializeField] private int _score;

    // explosion audio
    [SerializeField] AudioSource _externalAudioSource;
    [SerializeField]
    private AudioClip _explosionClip;


    // thruster vars
    // progress bar variables

    [Header("Thruster Variables")]

    [SerializeField] private GameObject _thrusterVisualizer;

    private RadialProgressBar _radialProgressBar;
    private Image _thrusterProgressBarImage;
    // private TMP_Text _thrusterProgressBarText;
    private bool _canActivateThruster = true;
    private bool _thrusterActive = false;


    [SerializeField] private float _thrusterMaxChargeLevel;
    [SerializeField] private float _thrusterMinChargeLevel;
    [SerializeField] private float _thrusterDischargeRate = 1.0f;
    [SerializeField] private float _thrusterRechargeRate = 0.5f;
    [SerializeField] private float _speedMultiplier = 2;
    private float _thrusterChargeLevel;


    [SerializeField] private float _thrusterReloadDuration = 10.0f;

    // private float _timeLeftOnThruster;

    void Start()
    {
        CheckObjectAssignments();
        transform.position = new Vector3(0, -3, 0);
        _directionModifier = 1;
        _baseFireRate = _fireRate;

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null) Debug.LogError("Player - spawnmanager not assigned");

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null) Debug.LogError("Player - UIManager not assigned");

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null) Debug.LogError("Player - audio source not assigned");

        _thrusterProgressBarImage = GameObject.Find("RadialProgressBar").GetComponent<Image>();
        if (_thrusterProgressBarImage == null) Debug.LogError("Player - thruster progress bar not assigned");

        _shieldRenderer = _shieldVisualizer.GetComponent<SpriteRenderer>();
        if (_shieldRenderer == null) Debug.LogError("Player - shield renderer not assigned");

        _playerAnimator = GetComponent<Animator>();
        if (_playerAnimator == null) Debug.LogError("Player - player animator not assigned");

        _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        if (_cameraShake == null) Debug.LogError("Player - Camera shake not assigned");

        _audioSource.clip = _fireLaserAudio;

        InitializeThrusters();
    }

    void Update()
    {
        CalculateMovement();
        CheckForWeaponFire();
        CheckForThruster();
        CheckForPowerupCollection();
        if (Input.GetKeyDown(KeyCode.L))
        {
            _lives = 3;
            _uiManager.UpdateLivesImage(_lives);
            _rightEngine.SetActive(false);
            _leftEngine.SetActive(false);
        } 
    }

    private void CheckObjectAssignments()
    {
        if (_thrusterVisualizer == null) Debug.LogError("Thruster visualizer not found");
        if (_missilePreFab == null) Debug.LogError("Missile prefab not found or assigned");
        if (_laserPrefab == null) Debug.LogError("Player - missile prefab not assigned");
        if (_tripleShotLaserPreFab == null) Debug.LogError("Player - tripleshotlaserprefab not assigned");
        if (_playerHomingMissilePreFab == null) Debug.LogError("Player - playerhomingmissileprefab not assigned");
        if (_spaceMinePreFab == null) Debug.LogError("Player -  spacemineprefab not assigned");

    }

    //***************************************************************************
    // Player Movement Code
    //***************************************************************************
    void CalculateMovement()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        _direction = new Vector3(_horizontalInput * _directionModifier, _verticalInput, 0);
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
    private void CheckForPowerupCollection()
    {
        if (Time.time > _canCollectPowerups) 
        { 
        _uiManager.PowerupCollectActive();
        }

        if (Input.GetKeyDown(KeyCode.C) && Time.time > _canCollectPowerups)
        {
            _canCollectPowerups = Time.time + _canCollectPowerupsCoolDown;
            _activePowerups = GameObject.FindGameObjectsWithTag("Powerup");
            for (int i=0; i < _activePowerups.Length; i++)
            {
                _activePowerups[i].transform.GetComponent<Powerup>().SetMoveTowardPlayer();
            }
            _uiManager.PowerupCollectInactive();
        }
    }

    //***************************************************************************
    // Check for weapon fire
    //***************************************************************************
    private void CheckForWeaponFire()
    {
        if (_weaponsEnabled == true)
        {
            if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
            {
                if (_missilePowerupActive == true) FireMissile();
                else if (_mineLauncherActive == true) DeployMines();
                else if (_ammoCount > 0) FireLaser();
            }

            if (Input.GetKeyDown(KeyCode.H) && _homingMissileActive)
            {
                FireHomingMissile();
                _homingMissileActive = false;
            }
        } else if (_weaponsEnabled == false && Input.GetKeyDown(KeyCode.Space))
        {
            // _uiManager.
        }
    }

    public void HomingMissileActive() // scj
    {
        _homingMissileActive = true;
        _uiManager.HomingMissileMessage("Homing Missile is Active (H)", true);
        // _uiManager.HomingMissileActive();
    }
    void FireHomingMissile()
    {
        _uiManager.HomingMissileInactive();
        _distanceToClosestEnemy = 100;
        _activeEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        for (int i = 0; i < _activeEnemies.Length; i++)
        {
            _tmp2DCollider = _activeEnemies[i].GetComponent<Collider2D>();
            _enemyTransform = _activeEnemies[i].transform;
            _enemyDistanceFromPlayer = Vector3.Distance(_enemyTransform.position, transform.position);
            if (_enemyDistanceFromPlayer < _distanceToClosestEnemy && _tmp2DCollider != null) 
            {
                _closestEnemy = _activeEnemies[i];
                _distanceToClosestEnemy = _enemyDistanceFromPlayer;
            }
        }

        if (_closestEnemy == null)
        {
            _audioSource.clip = _homingMissileMisfireAudio;
            _audioSource.Play();
            _uiManager.HomingMissileMessage("Homing Missile Misfire", false);
            return;
        }

        _homingMissile = Instantiate(_playerHomingMissilePreFab, transform.position + new Vector3(0, _laserOffset, 0), Quaternion.identity);
        _homingMissile.GetComponent<HomingMissile_Player>().MissileTarget(_closestEnemy);

    }

    public void DisablePowerupWeapons()
    {
        _tripleShotActive = false;
        _missilePowerupActive  = false;
        _mineLauncherActive = false;
        _homingMissileActive = false;
    }
    public void DisableWeapons()
    {
        _weaponsEnabled = false;
    }
    public void EnableWeapons() 
    {
        _weaponsEnabled = true;
    }

    public void IncreaseFireRate()
    {
        _fireRate = _fireRate / 2f;
    }

    public void ResetFireRate()
    {
        _fireRate = _baseFireRate;
    }

    //***************************************************************************
    // Laser Code
    //***************************************************************************
    void FireLaser()
    {
        _canFire = Time.time + _fireRate;
        if (_tripleShotActive == true && _ammoCount >= 3)
        {
            _tmpLaser = Instantiate(_tripleShotLaserPreFab, transform.position, Quaternion.identity);
            _tmpLaser.tag = "PlayerWeapon";
            _laserTransforms = _tmpLaser.GetComponentsInChildren<Transform>();
            for (int i=0; i< _laserTransforms.Length; i++)
            {
                _laserTransforms[i].tag = "PlayerWeapon";
            }

            _ammoCount -= 3;
        }
        else
        {
            _tmpLaser = Instantiate(_laserPrefab, transform.position + new Vector3(0, _laserOffset, 0), Quaternion.identity);
            _tmpLaser.tag = "PlayerWeapon";
            _ammoCount--;
        }
        _uiManager.UpdateAmmoCount(_ammoCount);

        if (_ammoCount <= 3)
        {
            _audioSource.clip = _ammoCountLow;
            _audioSource.Play();
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
        _ammoCount += _ammoReloadAmount;
        _uiManager.UpdateAmmoCount(_ammoCount);
        _uiManager.SetLowAmmoWarning(false);
    }
    public void AmmoForBossLevel(int AmmoBonus)
    {
        _ammoCount += AmmoBonus;
        _uiManager.UpdateAmmoCount(_ammoCount);
        _uiManager.SetLowAmmoWarning(false);
    }

    //***************************************************************************
    // Missile Code
    //***************************************************************************
    void FireMissile()
    {

        for (int fireAngle = 0; fireAngle < 360; fireAngle+= 30)
        {
            var newSpaceMissile = Instantiate(_missilePreFab, transform.position, Quaternion.identity);
            newSpaceMissile.transform.eulerAngles = Vector3.forward * fireAngle;

        }
    }

    public void MissilePowerupActive()
    {
        _missilePowerupActive = true;
        StartCoroutine(MissilePowerupPowerdown());
    }

    IEnumerator MissilePowerupPowerdown()
    {
        yield return new WaitForSeconds(_missilePowerupDuration);
        _missilePowerupActive = false;
    }

    //***************************************************************************
    // Mine Code
    //***************************************************************************
    void DeployMines()
    {

        StartCoroutine(DeployMinesTimed());
        _mineLauncherActive = false;
    }

    public void MineLauncherActive()
    {
        _mineLauncherActive = true;
    }

    IEnumerator DeployMinesTimed()
    {
        for (int i = 1; i < 6; i++)
        {
            var newSpaceMine = Instantiate(_spaceMinePreFab, transform.position, Quaternion.identity);
            switch (i)
            {
                case 1:
                    newSpaceMine.transform.eulerAngles = Vector3.forward * 0;
                    break;
                case 2:
                    newSpaceMine.transform.eulerAngles = Vector3.forward * 30;
                    break;
                case 3:
                    newSpaceMine.transform.eulerAngles = Vector3.forward * 60;
                    break;
                case 4:
                    newSpaceMine.transform.eulerAngles = Vector3.forward * 330;
                    break;
                case 5:
                    newSpaceMine.transform.eulerAngles = Vector3.forward * 300;
                    break;
            }
            yield return new WaitForSeconds(Random.Range(0.0f, 1.0f));
            // newSpaceMine.transform.eulerAngles = Vector3.forward * fireAngle;
        }
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
        if (Input.GetKeyUp(KeyCode.LeftShift) && _thrusterActive && _speedPowerupOn == false)
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
    // Negative Powerups
    //**********************************************************************************
    public void LeftRightSwapActive()
    {
        _directionModifier = -1;
        Debug.Log("direction modifier is now " +  _directionModifier);
        StartCoroutine(LeftRightSwapPowerDown());
    }

    IEnumerator LeftRightSwapPowerDown()
    {
        yield return new WaitForSeconds(_leftRightSwapDuration);
        _directionModifier = 1;
    }

    //**********************************************************************************
    //*  Damage/Lives management
    //**********************************************************************************
    public void Damage()
    {
        _externalAudioSource.clip = _explosionClip;
        _externalAudioSource.Play();
        _cameraShake.ShakeCamera();

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
        if (_lives < 0) 
        {
            _lives = 0;
        }

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
            _playerAnimator.SetTrigger("OnPlayerDeath");
            _spawnManager.OnPlayerDeath();
            _rightEngine.SetActive(false);
            _leftEngine.SetActive(false);
            gameObject.GetComponent<Collider2D>().enabled = false;
            _speed /= 1.25f;
            Destroy(gameObject, 2.4f);
        }
    }

    public void LivesPowerup()
    {
        switch (_lives)
        {
            case 1:
                _leftEngine.SetActive(false);
                break;
            case 2:
                _rightEngine.SetActive(false);
                break;
            default :
                break;
        }
        _lives++;
        if (_lives > 3) _lives = 3;
        _uiManager.UpdateLivesImage(_lives);
    }

    public void LivesReset()
    {
        _lives = 3;
        _uiManager.UpdateLivesImage(_lives);
        _rightEngine.SetActive(false);
        _leftEngine.SetActive(false);
        //scj - need "lives reset message
    }

    public void SpeedPowerupActive()
    {
        _speed = _defaultSpeed * _speedMultiplier;
        _speedPowerupOn = true;
        StartCoroutine(SpeedPowerupPowerDown());
    }

    IEnumerator SpeedPowerupPowerDown()
    {
        yield return new WaitForSeconds(5);
        _speedPowerupOn = false;
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
