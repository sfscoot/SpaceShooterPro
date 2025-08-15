using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorkscrewEnemy : MonoBehaviour
{
    [SerializeField] private float _enemySpeed = 2.50f;
    private Player _player;
    private SpawnManager _spawnManager;
    private AudioSource _explosionAudioSource;
    // create handle to animator component
    private Animator _enemyAnimator; // clean this up? 

    [SerializeField] private GameObject _laserPrefab;
    private float _fireRate = 3.0f;
    private float _canFire = -1;

    private bool _playerDestroyed = false;
    [SerializeField] private GameObject _explosionPreFab;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null) Debug.LogError("Error: player object not found");

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null) Debug.LogError("Error: SpawnManager not assigned");

        _explosionAudioSource = GetComponent<AudioSource>();
        if (_explosionAudioSource == null) Debug.LogError("Error: Explosion Audio Source not found");

        // _enemyAnimator = GetComponent<Animator>();
        // if (_enemyAnimator == null) Debug.LogError("Error: Enemy Animator Audio Source not found");
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
        // move  _enemySpeed meters per second
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
        if (transform.position.y <= -5.44f) // if moving off the bottom of screen, respawn at random x pos at top 
        {
            float randomX = Random.Range(-9.5f, 9.5f);
            transform.position = new Vector3(randomX, 7.5f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _explosionAudioSource.Play();
        if (other.tag == "Player")
        {
            _spawnManager.GetComponent<SpawnManager>().WaveEnemyDefeated();
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
        Instantiate(_explosionPreFab, transform.position, Quaternion.identity);
        // _enemyAnimator.SetTrigger("OnEnemyDeath");
        _enemySpeed /= 1.25f;
        Destroy(GetComponent<Collider2D>());
        Destroy(GetComponent<Rigidbody2D>());
        _canFire = Time.time + 2.8f; // make it so enemy can't fire after they've been destroyed.
        Destroy(gameObject, 0.25f);
    }

    void FireLaser()
    {
        
        _fireRate = Random.Range(3f, 7f);
        _canFire = Time.time + _fireRate;
        GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity); // spawn outside the collider
        enemyLaser.transform.parent = this.transform;

        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
        
    }
    public void SetPlayerDestroyed()
    {
        _playerDestroyed = true;
    }
}
