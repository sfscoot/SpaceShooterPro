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
    private int _currentWave;
    private int _currentWaveIndex;
    private int _waveEnemiesToSpawn = 0;
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

    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] _powerups;

    private bool _stopSpawning = false;

    private UIManager _uiManager;

    // random powerup variables


    private float _enemySpawnRate;
    private float _powerUpSpawnRate;
    private float _leftOrRight;
    private float _xSpawnValue;
    private int _powerupIndex;
    int _wavePowerupTypesAvailable;

    public void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _stopSpawning = true;
        _currentWave = 1;
        _uiManager.DisplayWaveOn(_currentWave);
    }
    public void StartSpawning()
    {

        WaveInitialize();
        _stopSpawning = false;
        StartCoroutine("NewSpawnEnemy");
        StartCoroutine("NewSpawnPowerUp");
        _uiManager.DisplayWaveOff();

    }

    public void Update()
    {
        if (_stopSpawning == false)
        {
            if (_waveEnemiesDefeated >= _waveEnemiesToSpawn)
            {
                _currentWave++;
                WaveInitialize();
                StartCoroutine("WaveTransition");
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
        _stopSpawning = true;
        _uiManager.DisplayWaveOn(_currentWave);
        yield return new WaitForSeconds(10);
        _stopSpawning = false;
        StartCoroutine("NewSpawnEnemy");
        StartCoroutine("NewSpawnPowerUp");
        _uiManager.DisplayWaveOff();

    }
    public void ReStart()
    {
        _stopSpawning = false;
    }
    IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(3.0f);
        while (_stopSpawning == false)
        {
            Vector3 enemyPosition = new Vector3(Random.Range(-9.5f, 9.5f), 7.0f, 0f);
            GameObject newEnemy = Instantiate(_enemyPrefab, enemyPosition, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(5);
        }
    }

    IEnumerator NewSpawnEnemy()
    {
        _waveEnemiesSpawned = 0;
        yield return new WaitForSeconds(_enemySpawnRate);
        while (_waveEnemiesSpawned < _waveEnemiesToSpawn && _stopSpawning == false)
        {
            _waveEnemiesSpawned++;
            _tmpEnemyType = EnemyTypePicker();

            switch (_tmpEnemyType)
            {
                case EnemyType.normal:
                    _tmpRandomEnemy = _normalEnemyList[Random.Range(0, _normalEnemyList.Count)];

                    // _tmpNewEnemy = Instantiate(_normalEnemyList[Random.Range(0, _normalEnemyList.Count)].enemyPrefab, enemySpawnPosition, Quaternion.identity);
                    // _tmpNewEnemy.transform.parent = _enemyContainer.transform;
                    break;
                case EnemyType.aggressive:
                    _tmpRandomEnemy = _aggressiveEnemyList[Random.Range(0, _aggressiveEnemyList.Count)];

                    // _tmpNewEnemy = Instantiate(_aggressiveEnemyList[Random.Range(0, _aggressiveEnemyList.Count)].enemyPrefab, enemySpawnPosition, Quaternion.identity);
                    // _tmpNewEnemy.transform.parent = _enemyContainer.transform;
                    break;
                case EnemyType.evil:
                    _tmpRandomEnemy = _evilEnemyList[Random.Range(0, _evilEnemyList.Count)];
                    // _tmpNewEnemy = Instantiate(_evilEnemyList[Random.Range(0, _evilEnemyList.Count)].enemyPrefab, enemySpawnPosition, Quaternion.identity);
                    // _tmpNewEnemy.transform.parent = _enemyContainer.transform;
                    break;
                default:
                    break;
            }
            // GameObject newEnemy = Instantiate(_enemyPrefab, enemySpawnPosition, Quaternion.identity);

            if (_tmpRandomEnemy._enemyMovementType == EnemyMovementType.vertical)
            {
                Vector3 enemySpawnPosition = new Vector3(Random.Range(-9.5f, 9.5f), 7.0f, 0f);
                _tmpNewEnemy = Instantiate(_tmpRandomEnemy.enemyPrefab, enemySpawnPosition, Quaternion.identity);
            } else
            {
                _leftOrRight = Random.Range(0, 1);
                if (_leftOrRight < 0.5f)
                {
                    _xSpawnValue = -7;
                }
                else
                {
                    _xSpawnValue = 7;
                }

                Vector3 enemySpawnPosition = new Vector3(_xSpawnValue, Random.Range(0.0f, 3.5f), 0f);
                _tmpNewEnemy = Instantiate(_tmpRandomEnemy.enemyPrefab, enemySpawnPosition, Quaternion.identity);
            }

            _tmpNewEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(_enemySpawnRate);

        }
    }
    IEnumerator SpawnPowerUp()
    {
        yield return new WaitForSeconds(3.0f);
        while (_stopSpawning == false)
        {
            Vector3 powerupSpawnPosition = new Vector3(Random.Range(-9.5f, 9.5f), 7.0f, 0.0f);
            int _powerupToSpawn = Random.Range(0, 7);
            Instantiate(_powerups[_powerupToSpawn], powerupSpawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(5);
        }
    }

    IEnumerator NewSpawnPowerUp()
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
        Debug.Log("enemies defeated this wave " +  _waveEnemiesDefeated);
    }

    public void StopAllSpawning()
    {
        _stopSpawning = true;
    }
}
