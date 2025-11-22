using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissileEnemy : MonoBehaviour
{
    [SerializeField] private float _enemySpeed = 2.50f;
    private Player _player;

    [SerializeField] private SpawnManager _spawnManager;

    // create handle to animator component
    private Animator _enemyAnimator;
    private AudioSource _explosionAudioSource;
    [SerializeField] private GameObject _explosionPreFab;
    private GameObject _explosion;
    [SerializeField] private GameObject _homingMissilePrefab;
    private GameObject _enemyWeapon;
    private float _fireRate = 3.0f;
    private float _canFire = -1;
    [SerializeField] private float _missileOffset = 1.0f;

    private bool _playerDestroyed;

    private void Start()
    {
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
        _enemyAnimator = GetComponent<Animator>();
        if (_enemyAnimator == null) Debug.LogError("Error: Enemy Animator Audio Source not found");
    }

    void Update()
    {
        CalculateMovement();
        if (Time.time > _canFire)
        {
            FireMissile();
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

        if (other.tag == "PlayerWeapon")
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
        _explosion.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        Destroy(gameObject);
    }

    void FireMissile()
    {
        _fireRate = Random.Range(3f, 7f);
        _canFire = Time.time + _fireRate;

        _enemyWeapon = Instantiate(_homingMissilePrefab, transform.position + new Vector3(0, _missileOffset, 0), Quaternion.identity); 
        _enemyWeapon.tag = "EnemyMissile";
        /* _enemyWeapon.transform.parent = this.transform;
        Laser[] lasers = _enemyWeapon.GetComponentsInChildren<Laser>();
        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
        */
    }

    public void SetPlayerDestroyed()
    {
        _playerDestroyed = true;
    }
}
