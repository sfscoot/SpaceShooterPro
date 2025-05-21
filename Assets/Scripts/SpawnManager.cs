using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    
    [SerializeField]
    private PowerUpClass[] _powerUps;

    [SerializeField]
    private List<PowerUpClass> _commonPowerupsList;
    [SerializeField]
    private List<PowerUpClass> _rarePowerupsList;
    [SerializeField]
    private List<PowerUpClass> _epicPowerupsList;

    [SerializeField]
    private PowerUpTypeClass[] _powerUpTypes;


    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerups;

    private bool _stopSpawning = false;


    // random powerup variables
    private PowerUpType _tmpPowerUpType;

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemy());
       // StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(NewSpawnPowerupRoutine());
    }

    public void Start()
    {
        LevelStartup(1);
    }
    public void ReStart()
    {
        StartCoroutine(SpawnEnemy());
        StartCoroutine(SpawnPowerupRoutine());
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

    IEnumerator SpawnPowerupRoutine()
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

    IEnumerator NewSpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        while (_stopSpawning == false)
        {
            _tmpPowerUpType = PowerUpPicker();
            Vector3 powerupSpawnPosition = new Vector3(Random.Range(-9.5f, 9.5f), 7.0f, 0.0f);
            switch (_tmpPowerUpType)
            {
                case PowerUpType.common:
                    Instantiate(_commonPowerupsList[Random.Range (0,_commonPowerupsList.Count)].powerupPrefab, powerupSpawnPosition, Quaternion.identity);
                    break;
                case PowerUpType.rare:
                    Instantiate(_rarePowerupsList[Random.Range(0, _rarePowerupsList.Count)].powerupPrefab, powerupSpawnPosition, Quaternion.identity);
                    break;
                case PowerUpType.epic:
                    Instantiate(_epicPowerupsList[Random.Range(0, _epicPowerupsList.Count)].powerupPrefab, powerupSpawnPosition, Quaternion.identity);
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(5);
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
        /*
        foreach (Transform child in _enemyContainer.transform)
        {
            Destroy(child.gameObject);
        }
        */
    }
    //
    // Powerup Availability System
    //
    public void LevelStartup(int currentWave)
    {
        _commonPowerupsList.Clear();
        _rarePowerupsList.Clear();
        _epicPowerupsList.Clear();


        for (int i = 0; i < _powerUps.Length; i++)
        {
            switch ((_powerUps[i].powerUpType).ToString()) 
            {
                case "common":
                    if (_powerUps[i].waveAvailable <= currentWave)
                    {
                        _commonPowerupsList.Add(_powerUps[i]);
                    }
                    break;
                case "rare":
                    if (_powerUps[i].waveAvailable <= currentWave)
                    {
                        //   _rarePowerups[_rareIdx] = _powerUps[i];
                        _rarePowerupsList.Add(_powerUps[i]);
                    }
                    break;
                case "epic":
                    if (_powerUps[i].waveAvailable <= currentWave)
                    {
                        _epicPowerupsList.Add(_powerUps[i]);
                    }
                    break;
                default:
                    Debug.LogError("unknown powerup type found: " +  _powerUps[i].name);
                    break;
            }
        }
    }
    private PowerUpType PowerUpPicker()
    {
        float RNG = Random.value;
        float runningTotal = 0;

        for (int i = 0; i < _powerUpTypes.Length; i++)
        {
            runningTotal += _powerUpTypes[i].frequency;
            if (RNG <= runningTotal)
            {
                return _powerUpTypes[i].powerUpType;
            }
                
        }

        return _powerUpTypes[0].powerUpType;
    }
}
