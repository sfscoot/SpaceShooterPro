using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HorizonalEnemy : MonoBehaviour
{
    [Header("Related Game Objects")]
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private GameObject _explosionPreFab;
    private GameObject _explosion;
    [SerializeField] private GameObject _shieldVisualizer;
    SpriteRenderer _shieldRenderer;

    [SerializeField]
    private float _enemySpeed = 2.50f;

    private int _direction = 1;

    [Header("Screen Boundaries")]
    [SerializeField] private float _rightBound = 11f;
    [SerializeField] private float _leftBound = -11f;
    [SerializeField] private float _topBound = 5f;
    [SerializeField] private float _bottomBound = -10.5f;
    [SerializeField] private float _topBoundOffset = 8;


    private float _randomY;

    // create handle to animator component
    private Animator _enemyAnimator;
    private AudioSource _explosionAudioSource;
    [SerializeField] private AudioSource _shieldDownAudioSource;

    [SerializeField]
    private Player _player;

    [SerializeField]
    private GameObject _laserPrefab; // leaving this in for later implementation of this enemy destroying powerups.
    private float _fireRate = 3.0f;
    private float _canFire = -1;
    private Transform _enemyShield;
    [SerializeField]  private GameObject _leftScanner;
    [SerializeField]  private GameObject _rightScanner;

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
        if (other.tag == "Player" || other.tag == "PlayerLaser" || other.tag == "Missile" || other.tag == "Mine")
        {
            {

            }
            if (_shieldRenderer.isVisible)
            {
                _shieldRenderer.enabled = false;
                _shieldDownAudioSource.Play();
            } else
            {
                _spawnManager.GetComponent<SpawnManager>().WaveEnemyDefeated();
                Player player = other.transform.GetComponent<Player>();

                if (player != null)
                {
                    player.Damage();
                }
                PlayEnemyDeathSequence();
            }
        }

        if (other.tag == "PlayerLaser" || other.tag == "Missile" || other.tag == "Mine")
        {
            other.gameObject.SetActive(false);
            Destroy(other.gameObject);
        }

        }

    void PlayEnemyDeathSequence()
    {
        Instantiate(_explosionPreFab, transform.position, Quaternion.identity);
        _explosion.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        Destroy(gameObject);

    }

    void FireLaser()
    {
        _fireRate = Random.Range(3f, 7f);
        _canFire = Time.time + _fireRate;
        GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity); // spawn outside the collider
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
