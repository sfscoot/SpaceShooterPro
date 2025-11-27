using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HorizonalEnemy : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] private Player _player;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private GameObject _explosionPreFab;
    [SerializeField] private AudioSource _shieldDownAudioSource;

    [SerializeField] private GameObject _shieldVisualizer;
    [SerializeField] private GameObject _leftScanner;
    [SerializeField] private GameObject _rightScanner;
    [SerializeField] private int _enemyPointsValue;
    [SerializeField] private float _enemySpeed = 2.50f;

    SpriteRenderer _shieldRenderer;
    private GameObject _explosion;

    private int _direction = 1;

    [Header("Screen Boundaries")]
    [SerializeField] private float _rightBound = 11f;
    [SerializeField] private float _leftBound = -11f;
    [SerializeField] private float _topBound = 5f;
    [SerializeField] private float _bottomBound = -10.5f;
    [SerializeField] private float _topBoundOffset = 8;

    private float _randomY;
    private AudioSource _explosionAudioSource;

    private float _fireRate = 3.0f;
    private float _canFire = -1;
    private Transform _enemyShield;
    [SerializeField] private float _laserOffset = -0.025f;



    private void Start()
    {
        _explosionAudioSource = GetComponent<AudioSource>();
        if (_explosionAudioSource == null ) Debug.LogError("explosion audio source not found");

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null) Debug.LogError("Horiconal Enemy - spawn manager not assigned");


        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null) Debug.Log("Horizonal enemy - player object not assigned");

        if (_shieldVisualizer == null)
        {
            Debug.LogError("Horizonal Enemy - shield visualizer not assigned");
        } else
        {
            _shieldRenderer = _shieldVisualizer.GetComponent<SpriteRenderer>();
            _shieldRenderer.enabled = true;
        }
        _enemyShield = transform.Find("Model/Shield");
        _shieldDownAudioSource = _enemyShield.GetComponent<AudioSource>();
        if (_shieldDownAudioSource == null) Debug.LogError("Horizonal enemy - shield audio source missing");
        this.transform.Find("Model/Left_Scanner");

        _leftScanner.SetActive(true);
        _rightScanner.SetActive(false);
        _canFire = Time.time + Random.Range(0.25f, 1.0f); // add 1 second delay before enemy can fire, otherwise they fire as soon as they're spawned
    }

    void Update()
    {
        CalculateMovement();
        if (Time.time > _canFire)
        {
            FireLaser();
        }
    }
    void CalculateMovement()
    {
        // transform.Translate(Vector3.right * (2 * Time.deltaTime));

        transform.Translate(Vector3.right * (_enemySpeed * Time.deltaTime * _direction));
        if (transform.position.x <= _leftBound)
        {
            _randomY = Random.Range((_topBound - _topBoundOffset), 0.0f);
            transform.position = new Vector3(_leftBound, _randomY, 0);
            _direction = 1;
            _leftScanner.SetActive(true);
            _rightScanner.SetActive(false);

        }
        else if (transform.position.x >= _rightBound)
        {
            _randomY = Random.Range((_topBound - _topBoundOffset), 0.0f);
            transform.position = new Vector3(_rightBound, _randomY, 0);
            _direction = -1;
            _leftScanner.SetActive(false);
            _rightScanner.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" || other.name == "Player") Debug.Break();
        Debug.Log($"horizonal enemy hit by {other.tag} and {other.name}");
        if (other.tag != "Player" &&  other.tag != "PlayerWeapon") return;

        // code for when enemy shield is active

        if (_shieldRenderer.enabled == true && other.tag == "PlayerWeapon")
        {
            _shieldRenderer.enabled = false;
            _shieldDownAudioSource.Play();
            return;
        }

        if (_shieldRenderer.enabled == true && other.tag == "Player")
        {
            Debug.Log($"rammed by player, shield should be down");
            _shieldRenderer.enabled = false;
            _shieldDownAudioSource.Play();
            _player.Damage();
            return;
        }

        // code for when enemy shield is inactive
        if (other.tag == "PlayerWeapon")
        {
            gameObject.GetComponent<Collider2D>().enabled = false; // this prevents a tripleshot from getting double credit for destroying this
            _player.AddToScore(_enemyPointsValue);
            _spawnManager.GetComponent<SpawnManager>().WaveEnemyDefeated();
            Destroy(other.gameObject);
        }

        if (other.tag == "Player")
        {
            Debug.Log("player hit the horizonal enemy");
            _player.AddToScore(_enemyPointsValue);
            _spawnManager.GetComponent<SpawnManager>().WaveEnemyDefeated();
            _player.Damage();
        }

        PlayEnemyDeathSequence();
    }

    void PlayEnemyDeathSequence()
    {
        Debug.Log("should be in horizonal enemy death sequencce");
        _explosion = Instantiate(_explosionPreFab, transform.position, Quaternion.identity);
        _explosion.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        Destroy(gameObject);
    }

    void FireLaser()
    {
        _fireRate = Random.Range(3f, 7f);
        _canFire = Time.time + _fireRate;
        GameObject enemyLaser = Instantiate(_laserPrefab, transform.position+  new Vector3(0, _laserOffset, 0), Quaternion.identity); // spawn outside the collider
        enemyLaser.tag = "EnemyLaser";
        enemyLaser.transform.parent = this.transform;

        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
    }

    public void SetPlayerDestroyed()
    {
        // _playerDestroyed = true;
    }
}
