using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine.UI;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private WaveClass[] _waves;
    [SerializeField] private int _waveTransitionDelay = 10;
    [SerializeField] private int _bossLevel = 4;
    [SerializeField] private int _bossLevel1AmmoBonus = 50;
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

    [SerializeField] private GameObject _bossGameObject;
    private Boss _boss;
    [SerializeField] GameObject _mechAttack;
    [SerializeField] private MechAttack _mechAttackController;
    private bool _bossLevelActive = false;
    private bool _bossWaveAttackActive = false;
    private int _bossWave = 1;

    private bool _stopSpawning = false;

    private UIManager _uiManager;

    // random powerup variables


    private float _enemySpawnRate;
    private float _powerUpSpawnRate;
    private float _leftOrRight;
    private float _xSpawnValue;
    private int _powerupIndex;
    int _wavePowerupTypesAvailable;

    private Player _player;

    public void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _stopSpawning = true;
        _currentWave = 1;
        _uiManager.DisplayWaveOn(_currentWave);
        _boss = _bossGameObject.GetComponent<Boss>();  // scj - change this to the "try" format
        if (_boss == null) Debug.LogError("boss missing");
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null) Debug.LogError("Player missing");
    }
    public void StartSpawning()
    {
        _uiManager.DisplayWaveOff();
        WaveInitialize();
        _stopSpawning = false;
        StartCoroutine("SpawnEnemy");
        StartCoroutine("SpawnPowerUp");
    }

    public void Update()
    {
        if (_waveEnemiesDefeated == _waveEnemiesToSpawn && _bossLevelActive == false)
        {
            _stopSpawning = true;

            WaveInitialize();
            _currentWave++;
            if (_currentWave == _bossLevel)
            {
                _bossLevelActive = true;
                _bossWave = 1;
            }
            else StartCoroutine("WaveTransition");
        }

        if (_bossLevelActive == true && _bossWaveAttackActive == false)
        {
            _bossWaveAttackActive = true;
            switch (_bossWave)
            {
                case 1:
                    StartCoroutine("BossWave1");
                    break;
                case 2:
                    StartCoroutine("BossWave2");
                    break;
                case 3:
                    StartCoroutine("BossWave3");
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
        _uiManager.UpdateKills(_waveEnemiesDefeated, _waveEnemiesToSpawn);

        InitializeWavePowerUps();
        InitializeWaveEnemies();
    }
    IEnumerator WaveTransition()
    {
        yield return new WaitForSeconds(1);
        _uiManager.DisplayWaveOn(_currentWave);
        yield return new WaitForSeconds(_waveTransitionDelay);

        _stopSpawning = false;
        StartCoroutine("SpawnEnemy");
        StartCoroutine("SpawnPowerUp");
        _uiManager.DisplayWaveOff();

    }

    IEnumerator BossWave1()
    {
        yield return new WaitForSeconds(1); // pause for dramatic effect
        _uiManager.DisplayBossWaveOn(1);  // turn on and bring in the boss
        _uiManager.TurnOffScore();
        _uiManager.TurnOffEnemyDeathCount();
        _player.LivesReset(); // reset lives to full capacity
        _player.AmmoForBossLevel(_bossLevel1AmmoBonus);
        _boss.ActivateBoss();
        yield return new WaitForSeconds(14);
        _uiManager.DisplayWaveOff();
    }

    IEnumerator BossWave2()
    {
        yield return new WaitForSeconds(1); // pause for dramatic effect
        _uiManager.DisplayBossWaveOn(2);  // turn on and bring in the boss

        _player.LivesReset(); // reset lives to full capacity
        _player.AmmoForBossLevel(_bossLevel1AmmoBonus);

        // drop the mechs
        // start the mech attach waves
        _mechAttack.SetActive(true);
        _boss.BossSeparates();

        yield return new WaitForSeconds(3);

        _uiManager.DisplayWaveOff();
        _mechAttackController.BringOnTheMechs();
        yield return new WaitForSeconds(10);

        _mechAttackController.StartCoroutine("MechAttackLToR");
        yield return new WaitForSeconds(5);
        Debug.Log("resetting the mechs");
        _mechAttackController.ResetTheMechs();
    }

    IEnumerator BossWave3()
    {
        yield return new WaitForSeconds(1); // pause for dramatic effect
        _uiManager.DisplayBossWaveOn(3);  // turn on and bring in the boss

        _player.LivesReset(); // reset lives to full capacity
        _player.AmmoForBossLevel(_bossLevel1AmmoBonus);

        // drop the mechs
        // start the mech attach waves
        _boss.ActivateBoss();

        yield return new WaitForSeconds(14);
        _uiManager.DisplayWaveOff();
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

    IEnumerator SpawnEnemy()
    {
        _waveEnemiesSpawned = 0;
        yield return new WaitForSeconds(_enemySpawnRate);

        for (int i = 1; i <= _waveEnemiesToSpawn; i++) 
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
        StopAllSpawning();

        foreach (Transform child in _enemyContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    //
    // Powerup Availability System
    //

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
        float RNG = Random.value;
        float runningTotal = 0;

        //        for (int i = 0; i < _powerUpTypes.Length; i++)
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

    private GameObject RarePowerUpPicker() // scj
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

    private GameObject EpicPowerUpPicker() // scj
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
    }

    private EnemyType EnemyTypePicker()
    {
        float RNG = Random.value;
        float runningTotal = 0;

        for (int i = 0; i < _enemyTypes.Length; i++)
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
        _uiManager.UpdateKills(_waveEnemiesDefeated, _waveEnemiesToSpawn);
    }

    public void StopAllSpawning()
    {
        _stopSpawning = true;
    }
}
