using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private float _enemySpeed = 2.50f;
    private Player _player;

    [SerializeField] private SpawnManager _spawnManager;

    // create handle to animator component
    private Animator _bossAnimator;
    private AudioSource _explosionAudioSource;
    [SerializeField] private GameObject _explosionPreFab;

    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _homingMissilePrefab;
    private GameObject _enemyWeapon;
    private float _fireRate = 3.0f;
    private float _canFire = -1;

    private bool _playerDestroyed;

    private void Awake()
    {
        
    }

    private void Start()
    {
        transform.position = new Vector3(17.5f, 5.5f, 0f);
        _player = GameObject.Find("Player").GetComponent<Player>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_player == null)
        {
            Debug.LogError("Player not assigned " + this.tag + "  " + this.gameObject.name);
        }
        if (_spawnManager == null)
        {
            Debug.LogError("SpawnManager not assigned");
        }
        _explosionAudioSource = GetComponent<AudioSource>();
        if (_explosionAudioSource == null)
        {
            Debug.LogError("Error: Explosion Audio Source not found");
        }
        _bossAnimator = GetComponent<Animator>();
        if (_bossAnimator == null) Debug.LogError("Error: Enemy Animator Audio Source not found");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _bossAnimator.SetTrigger("BossEnters");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            _bossAnimator.SetTrigger("BossSeparates");
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            _bossAnimator.SetTrigger("BossRejoins");
        }
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
        if (transform.position.y <= -5.44f) // if moving off the bottom of screen, respawn at random x pos at top 
        {
            float randomX = Random.Range(-9.5f, 9.5f);
            transform.position = new Vector3(randomX, 7.5f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _spawnManager.GetComponent<SpawnManager>().WaveEnemyDefeated();
            _explosionAudioSource.Play();
            Player player = other.transform.GetComponent<Player>();

            if (_player != null)
            {
                _player.Damage();
            }

            PlayEnemyDeathSequence();
        }

        if (other.tag == "PlayerLaser")
        {
            _spawnManager.GetComponent<SpawnManager>().WaveEnemyDefeated();
            _explosionAudioSource.Play();
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddToScore(10);
            }

            PlayEnemyDeathSequence();
        }

        if (other.tag == "Missile")
        {
            _spawnManager.GetComponent<SpawnManager>().WaveEnemyDefeated();
            _explosionAudioSource.Play();
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddToScore(10);
            }

            PlayEnemyDeathSequence();
        }

        if (other.tag == "Mine")
        {
            _spawnManager.GetComponent<SpawnManager>().WaveEnemyDefeated();
            _explosionAudioSource.Play();
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddToScore(10);
            }

            PlayEnemyDeathSequence();
        }
    }

    void PlayEnemyDeathSequence()
    {
        _bossAnimator.SetTrigger("OnEnemyDeath");
        // Instantiate(_explosionPreFab, transform.position, Quaternion.identity);
        _enemySpeed /= 1.25f;
        Destroy(GetComponent<Collider2D>());
        Destroy(GetComponent<Rigidbody2D>());
        _canFire = Time.time + 2.8f; // make it so enemy can't fire after they've been destroyed.
        Destroy(gameObject, 2.8f);
    }

    void FireLaser()
    {
        _fireRate = Random.Range(3f, 7f);
        _canFire = Time.time + _fireRate;

        if (gameObject.name == "Homing_Missile_Enemy")
        {
            _enemyWeapon = Instantiate(_homingMissilePrefab, transform.position, Quaternion.identity); // spawn outside the collider
            _enemyWeapon.tag = "EnemyMissile";
        }
        else
        {
            _enemyWeapon = Instantiate(_laserPrefab, transform.position, Quaternion.identity); // spawn outside the collider
            _enemyWeapon.tag = "EnemyLaser";
            _enemyWeapon.transform.parent = this.transform;
            Laser[] lasers = _enemyWeapon.GetComponentsInChildren<Laser>();
            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
    }

    public void SetPlayerDestroyed()
    {
        _playerDestroyed = true;
    }
}
