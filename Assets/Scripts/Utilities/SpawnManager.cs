using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
// using System.Diagnostics;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private WaveClass[] _waves;
    [SerializeField] private int _waveTransitionDelay = 10;

    [SerializeField] private GameObject _killCountDown;
    private int _currentWave;
    private int _currentWaveIndex;
    private int _waveEnemiesToSpawn = 99;
    private int _waveEnemiesSpawned = 0;
    private int _waveEnemiesDefeated = 0;
    

    [SerializeField] private PowerUpTypeClass[] _powerUpTypes;
    [SerializeField] private PowerUpClass[] _powerUps;
    [SerializeField] private List<PowerUpClass> _commonPowerUpsList;
    [SerializeField] private List<PowerUpClass> _rarePowerUpsList;
    [SerializeField] private List<PowerUpClass> _epicPowerUpsList;
    private PowerUpType _tmpPowerUpType;


    [SerializeField] private EnemyTypeClass[] _enemyTypes;
    [SerializeField] private EnemyClass[] _enemies;
    [SerializeField] private List <EnemyClass> _normalEnemyList;
    [SerializeField] private List<EnemyClass> _aggressiveEnemyList;
    [SerializeField] private List<EnemyClass> _evilEnemyList;
    private EnemyType _tmpEnemyType;
    private GameObject _tmpNewEnemy;
    private EnemyClass _tmpRandomEnemy;
    private Vector3 _enemySpawnPosition;

    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] _powerups;


    [Header("Boss Level Variables")]
    [SerializeField] private GameObject _bossGameObject;
    [SerializeField] private GameObject _mechAttack;
    [SerializeField] private MechAttack _mechAttackController;
    [SerializeField] private GameObject _dreadnaughtFront;
    private DreadnaughtFront _dreadnaughtFrontController;


    [SerializeField] private GameObject _dreadnaughtRear;
    private DreadnaughtRear _dreadnaughtRearController;
    [SerializeField] private GameObject _bossShield;
    private Boss _boss;
    private bool _bossLevelActive = false;
    private bool _bossWaveAttackActive = false;
    private int _bossWave = 1;
    private Transform[] _dreadnaughtFrontWeapons;
    private Transform[] _dreadnaughtRearWeapons;
    [SerializeField] private int _bossLevel = 4;
    [SerializeField] private int _bossLevel1AmmoBonus = 50;
    [SerializeField] private float _mechLToRAttackSpeed = 5f;
    [SerializeField] private float _mechOutsideInAttackSpeed = 8f;

    private bool _stopSpawning = false;
    private bool _gameOver = false;

    private UIManager _uiManager;

    // random powerup variables


    private float _enemySpawnRate;
    private float _powerUpSpawnRate;
    private float _leftOrRight;
    private float _xSpawnValue;
    private int _powerupIndex;
    int _wavePowerupTypesAvailable;
    int _waveEnemyTypesAvailable;
    [SerializeField] private float _mechVdropTarget;

    private Player _player;
    private GameObject[] _rogueEnemies;
    private GameObject _explosion;
    [SerializeField] private GameObject _explosionPreFab;
    private Collider2D _tmp2DCollider;
    public void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _stopSpawning = true;
        _currentWave = 1;
        _uiManager.DisplayWaveOn(_currentWave);
        _boss = _bossGameObject.GetComponent<Boss>(); 

        if (_boss == null) Debug.LogError("boss missing");
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null) Debug.LogError("Player missing");

        if (_dreadnaughtFront == null) Debug.LogError("DreadnaughtFront gameobject unassigned");
        if (_dreadnaughtRear == null)  Debug.LogError("DreadnaughtFront gameobject unassigned");
        _dreadnaughtFrontController = _dreadnaughtFront.GetComponent<DreadnaughtFront>();
        _dreadnaughtRearController = _dreadnaughtRear.GetComponent<DreadnaughtRear>();
    }
    public void StartSpawning()
    {
        _uiManager.DisplayWaveOff();
        WaveInitialize();
        _stopSpawning = false;
        StartCoroutine(SpawnEnemy(_waveEnemiesToSpawn));
        StartCoroutine("SpawnPowerUp");
        if (_currentWave == 1)
        {
            _uiManager.UpdateKills(_waveEnemiesDefeated, _waveEnemiesToSpawn);
        }
    }

    public void Update()
    {
        if (_bossLevelActive == true)
            BossLevelController();
        else
            LevelController();
    }

    private void LevelController()
    {
        if (_waveEnemiesDefeated == _waveEnemiesToSpawn)
        {
            KillRogueEnemies();
            _stopSpawning = true;
            _currentWave++;

            if (_currentWave < _bossLevel)
            {
                WaveInitialize();
                StartCoroutine("WaveTransition");
            }


            if (_currentWave == _bossLevel)
            {
                _bossLevelActive = true;
                _bossWave = 1;
            }
        }
    }

    private void KillRogueEnemies()
    {
        _rogueEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < _rogueEnemies.Length; i++)
        {
            _tmp2DCollider = _rogueEnemies[i].GetComponent<Collider2D>();
            if (_tmp2DCollider != null)
            {
                _explosion = Instantiate(_explosionPreFab, transform.position, Quaternion.identity);
                _explosion.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                Destroy(_rogueEnemies[i]);
            }
        }
    }
    public void BossLevelController()
    {
        if (_bossLevelActive == true && _bossWaveAttackActive == false)
        {
            _bossWaveAttackActive = true;
            switch (_bossWave)
            {
                case 1:
                    StartCoroutine(BossWave1());
                    break;
                case 2:
                    StartCoroutine(BossWave2());
                    break;
                case 3:
                    StartCoroutine(BossWave3());
                    break;
                case 4:
                    StartCoroutine(BossWave4());
                    break;
                case 5:
                    StartCoroutine(BossWave5());
                    break;
                case 6:
                    StartCoroutine(GameOverPlayerWins());
                    break;
                default:
                    Debug.LogWarning("bad switch case in boss controller");
                    break;
            }
        }
    }
    public void WaveInitialize()
    {
        _currentWaveIndex = _currentWave - 1;
        _waveEnemiesToSpawn = _waves[_currentWaveIndex].enemyCount;

        _enemySpawnRate = _waves[_currentWaveIndex].enemySpawnRate;
        _powerUpSpawnRate = _waves[_currentWaveIndex].powerUpSpawnRate;
        _waveEnemiesDefeated = 0;

        InitializeWavePowerUps();
        InitializeWaveEnemies();
    }
    IEnumerator WaveTransition()
    {
        yield return new WaitForSeconds(2);
        _uiManager.DisplayWaveOn(_currentWave);
        yield return new WaitForSeconds(_waveTransitionDelay);

        _stopSpawning = false;
        StartCoroutine(SpawnEnemy(_waveEnemiesToSpawn));
        StartCoroutine("SpawnPowerUp");
        _uiManager.DisplayWaveOff();
        _uiManager.UpdateKills(_waveEnemiesDefeated, _waveEnemiesToSpawn);
    }

    IEnumerator BossWave1()
    {
        _player.DisableWeapons();
        _player.DisablePowerupWeapons();
        yield return new WaitForSeconds(1); // pause for dramatic effect
        _boss.ClearSceneOfPlayerWeapons();
        _uiManager.DisplayBossWaveOn(1);  // turn on and bring in the boss
        _uiManager.TurnOffScore();
        _uiManager.TurnOffEnemyDeathCount();
        _player.LivesReset(); // reset lives to full capacity
        _player.AmmoForBossLevel(_bossLevel1AmmoBonus);
        _boss.ActivateBoss();
        yield return new WaitForSeconds(14);
        _uiManager.DisplayWaveOff();
        _boss.StartSweepAndShoot();
        _player.EnableWeapons();
        _stopSpawning = false;
        _powerUpSpawnRate = _waves[_currentWave - 1].powerUpSpawnRate;
        StartCoroutine("SpawnPowerUp");
    }

    IEnumerator BossWave2()
    {
        _stopSpawning = true;
        _player.DisableWeapons();
        yield return new WaitForSeconds(2);
        _uiManager.DisplayBossWaveOn(2);  // turn on and bring in the boss
        _mechAttack.SetActive(true);
        _boss.BossSeparates();

        yield return new WaitForSeconds(2);

        _uiManager.DisplayWaveOff();
        yield return StartCoroutine(_mechAttackController.MechsDropToPositionCoroutine(_mechVdropTarget, false));
        yield return StartCoroutine(_mechAttackController.MechsSpreadToPositionCoroutine());
        yield return new WaitForSeconds(4);

        _mechAttackController.StartMechAttackLToR(_mechLToRAttackSpeed);
        _uiManager.GameBroadcastMessage("Weapons offline, take evasive action", 1);
        yield return new WaitForSeconds(2);
    }

    IEnumerator BossWave3()
    {
        _mechAttackController.MechsPositionsReset();
        yield return StartCoroutine(_mechAttackController.MechsDropToPositionCoroutine(_mechVdropTarget, false));
        yield return StartCoroutine(_mechAttackController.MechsSpreadToPositionCoroutine());

        yield return new WaitForSeconds(4);
        _mechAttackController.StartMechAttackOutsideIn(_mechOutsideInAttackSpeed);
        yield return new WaitForSeconds(2);
    }

    IEnumerator BossWave4()
    {
        _mechAttackController.MechsPositionsReset();
        
        yield return StartCoroutine(_mechAttackController.MechsDropToPositionCoroutine(_mechVdropTarget, false));
        yield return StartCoroutine(_mechAttackController.MechsSpreadToPositionCoroutine());
        yield return new WaitForSeconds(4);


        yield return StartCoroutine(_mechAttackController.StartMechBounceAttackCoroutine(_mechOutsideInAttackSpeed));
        _player.EnableWeapons();
        _uiManager.GameBroadcastMessageOff();
        yield return new WaitForSeconds(0.25f);
        _uiManager.GameBroadcastMessage("Weapons back online, fire at will", 2);
        yield return new WaitForSeconds(4);
        _uiManager.GameBroadcastMessageOff();
    }
    IEnumerator BossWave5() // final wave
    {
        yield return new WaitForSeconds(3);
        _boss.BossRejoins();
        yield return new WaitForSeconds(3);
        _boss.BossLeaves();
        yield return new WaitForSeconds(3);
        _boss.BossEnters();
        yield return new WaitForSeconds(3);
        _uiManager.DisplayBossWaveOn(_bossWave);  // turn on and bring in the boss
        yield return new WaitForSeconds(5);

        ActivateBossWeapons();
        ActivateBossShield();
        _boss.StartSweepAndShoot();
        _uiManager.DisplayWaveOff();
        _player.EnableWeapons();
        _stopSpawning = false;
    }

    private void ActivateBossWeapons()
    {
        _dreadnaughtFrontWeapons = _dreadnaughtFront.GetComponentsInChildren<Transform>(true);
        _dreadnaughtRearWeapons = _dreadnaughtRear.GetComponentsInChildren<Transform>(true);

        for (int i = 0; i < _dreadnaughtFrontWeapons.Length; i++)
        {
            _dreadnaughtFrontWeapons[i].gameObject.SetActive(true);
        }

        foreach (Transform t in _dreadnaughtRearWeapons)
        {
            t.gameObject.SetActive(true);
        }
    }

    private void ActivateBossShield() 
    { 
        _bossShield.SetActive(true);
        for (int i = 0; i < _bossShield.transform.childCount; i++)
        {
            _bossShield.transform.GetChild(i).gameObject.SetActive(true);
        }
        _boss.ActivateBossShield();
    
    }

    IEnumerator GameOverPlayerWins()
    {
        yield return new WaitForSeconds(1); // pause for dramatic effect
        _dreadnaughtFrontController.DreadnaughtDestruction();
        yield return new WaitForSeconds(1);
        _dreadnaughtRearController.DreadnaughtDestruction();
        _uiManager.DisplayBossWaveOn(_bossWave);  
    }
    public void BossWaveComplete()
    {
        _bossWave++;
        _bossWaveAttackActive = false;
    }
    public void ReStart()
    {
        _stopSpawning = false;
    }

    IEnumerator SpawnEnemy(int waveEnemiesToSpawn)
    {
        _waveEnemiesSpawned = 0;
        yield return new WaitForSeconds(_enemySpawnRate);

        for (int i = 1; i <= waveEnemiesToSpawn; i++) 
        {
            _tmpEnemyType = EnemyTypePicker();

            switch (_tmpEnemyType)
            {
                case EnemyType.normal:
                    _tmpRandomEnemy = _normalEnemyList[Random.Range(0, _normalEnemyList.Count)];
                    break;
                case EnemyType.aggressive:
                    _tmpRandomEnemy = _aggressiveEnemyList[Random.Range(0, _aggressiveEnemyList.Count)];
                    break;
                case EnemyType.evil:
                    _tmpRandomEnemy = _evilEnemyList[Random.Range(0, _evilEnemyList.Count)];
                    break;
                default:
                    break;
            }

            if (_tmpRandomEnemy._enemyMovementType == EnemyMovementType.vertical)
            {
                Vector3 enemySpawnPosition = new Vector3(Random.Range(-9.5f, 9.5f), 7.0f, 0f);
                _tmpNewEnemy = Instantiate(_tmpRandomEnemy.enemyPrefab, enemySpawnPosition, Quaternion.identity);
                _waveEnemiesSpawned++;
            } else
            {
                _leftOrRight = Random.Range(0f, 1f);

                if (_leftOrRight < 0.5f)
                {
                    _xSpawnValue = -11.25f;
                }
                else
                {
                    _xSpawnValue =11.25f;
                }

                //Vector3 enemySpawnPosition = new Vector3(_xSpawnValue, Random.Range(-1.5f, 3.5f), 0f);
                _enemySpawnPosition = new Vector3(_xSpawnValue, Random.Range(-1.5f, 3.5f), 0f);
                _tmpNewEnemy = Instantiate(_tmpRandomEnemy.enemyPrefab, _enemySpawnPosition, Quaternion.identity);
                _waveEnemiesSpawned++;
            }

            _tmpNewEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(_enemySpawnRate);
        }
    }

    IEnumerator SpawnPowerUp()
    {
        yield return new WaitForSeconds(_powerUpSpawnRate);
        while (_stopSpawning == false)
        {
            _tmpPowerUpType = PowerUpTypePicker();
            Vector3 powerupSpawnPosition = new Vector3(Random.Range(-9.5f, 9.5f), 7.0f, 0.0f);
            switch (_tmpPowerUpType)
            {
                case PowerUpType.common:
                    Instantiate(CommonPowerUpPicker(), powerupSpawnPosition, Quaternion.identity);
                    break;
                case PowerUpType.rare:
                    Instantiate(RarePowerUpPicker(), powerupSpawnPosition, Quaternion.identity);
                    // Instantiate(_rarePowerUpsList[Random.Range(0, _rarePowerUpsList.Count)].powerupPrefab, powerupSpawnPosition, Quaternion.identity);
                    break;
                case PowerUpType.epic:
                    Instantiate(EpicPowerUpPicker(), powerupSpawnPosition, Quaternion.identity);
                    // Instantiate(_epicPowerUpsList[Random.Range(0, _epicPowerUpsList.Count)].powerupPrefab, powerupSpawnPosition, Quaternion.identity);
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(_powerUpSpawnRate);
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;        
        foreach (Transform child in _enemyContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }
    private void InitializeWavePowerUps()
    {
        _commonPowerUpsList.Clear();
        _rarePowerUpsList.Clear();
        _epicPowerUpsList.Clear();

        for (int i = 0; i < _powerUps.Length; i++)
        {
            switch ((_powerUps[i].powerUpType).ToString())
            {
                case "common":
                    if (_powerUps[i].waveAvailable <= _currentWave)
                    {
                        _commonPowerUpsList.Add(_powerUps[i]);
                    }
                    break;
                case "rare":
                    if (_powerUps[i].waveAvailable <= _currentWave)
                    {
                        _rarePowerUpsList.Add(_powerUps[i]);
                    }
                    break;
                case "epic":
                    if (_powerUps[i].waveAvailable <= _currentWave)
                    {
                        _epicPowerUpsList.Add(_powerUps[i]);
                    }
                    break;
                default:
                    Debug.LogError("unknown powerup type found: " + _powerUps[i].name);
                    break;
            }
        }

        _wavePowerupTypesAvailable = 0;
        if (_commonPowerUpsList.Count > 0) _wavePowerupTypesAvailable++;
        if (_rarePowerUpsList.Count > 0) _wavePowerupTypesAvailable++;
        if (_epicPowerUpsList.Count > 0) _wavePowerupTypesAvailable++;
    }
    private PowerUpType PowerUpTypePicker()
    {
        if (_currentWave == 6) // boss level - only shield and ammo available
        {
            return _powerUpTypes[0].powerUpType;
        }
        float RNG = Random.value;
        float runningTotal = 0;

        for (int i = 0; i < _wavePowerupTypesAvailable; i++)
        {
            runningTotal += _powerUpTypes[i].frequency;
            if (RNG <= runningTotal)
            {
                return _powerUpTypes[i].powerUpType;
            } 
        }
        return _powerUpTypes[0].powerUpType;
    }

    private GameObject CommonPowerUpPicker() // scj
    {
        float RNG = Random.value;
        float runningTotal = 0;

        for (int i = 0; i < _commonPowerUpsList.Count; i++)
        {
            runningTotal += _commonPowerUpsList[i].frequency;
            if (RNG <= runningTotal)
            {
                return _commonPowerUpsList[i].powerupPrefab;
            }
        }

        return _commonPowerUpsList[0].powerupPrefab;
    }

    private GameObject RarePowerUpPicker()
    {
        float RNG = Random.value;
        float runningTotal = 0;

        for (int i = 0; i < _rarePowerUpsList.Count; i++)
        {
            runningTotal += _rarePowerUpsList[i].frequency;
            if (RNG <= runningTotal)
            {
                return _rarePowerUpsList[i].powerupPrefab;
            }
        }
        return _rarePowerUpsList[0].powerupPrefab;
    }

    private GameObject EpicPowerUpPicker() 
    {
        float RNG = Random.value;
        float runningTotal = 0;

        for (int i = 0; i < _epicPowerUpsList.Count; i++)
        {
            runningTotal += _epicPowerUpsList[i].frequency;
            if (RNG <= runningTotal)
            {
                return _epicPowerUpsList[i].powerupPrefab;
            }
        }

        return _epicPowerUpsList[0].powerupPrefab;
    }
    private void InitializeWaveEnemies()
    {
        _normalEnemyList.Clear();
        _aggressiveEnemyList.Clear();
        _evilEnemyList.Clear();


        for (int i = 0; i < _enemies.Length; i++)
        {
            switch ((_enemies[i]._enemyType).ToString())
            {
                case "normal":
                    if (_enemies[i].waveAvailable <= _currentWave)
                    {
                        _normalEnemyList.Add(_enemies[i]);
                    }
                    break;
                case "aggressive":
                    if (_enemies[i].waveAvailable <= _currentWave)
                    {
                        _aggressiveEnemyList.Add(_enemies[i]);
                    }
                    break;
                case "evil":
                    if (_enemies[i].waveAvailable <= _currentWave)
                    {
                        _epicPowerUpsList.Add(_powerUps[i]);
                    }
                    break;
                case "boss": break; // ignore the boss - always a solid idea
                default:
                    Debug.LogError("unknown powerup type found: " + _powerUps[i].name);
                    break;
            }
        }
        _waveEnemyTypesAvailable = 0;
        if (_normalEnemyList.Count > 0) _waveEnemyTypesAvailable++;
        if (_aggressiveEnemyList.Count > 0) _waveEnemyTypesAvailable++;
        if (_evilEnemyList.Count > 0) _waveEnemyTypesAvailable++;
    }

    private EnemyType EnemyTypePicker()
    {
        float RNG = Random.value;
        float runningTotal = 0;

        for (int i = 0; i < _waveEnemyTypesAvailable; i++)
        {
            runningTotal += _enemyTypes[i].frequency;
            if (RNG <= runningTotal)
            {
                return _enemyTypes[i].enemyType;
            }
        }

        return _enemyTypes[0].enemyType;
    }

    public void WaveEnemyDefeated()
    {
        _waveEnemiesDefeated++;
        // Debug.Log($"wave enemies defeated {_waveEnemiesDefeated} of {_waveEnemiesToSpawn} with {_waveEnemiesSpawned} already spawned");
        if (_waveEnemiesDefeated > _waveEnemiesSpawned) Debug.Break();
        _uiManager.UpdateKills(_waveEnemiesDefeated, _waveEnemiesToSpawn);
    }
}
