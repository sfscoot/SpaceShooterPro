﻿using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class EvasiveEnemy : MonoBehaviour
{
    // tmp vars
    private bool _canMove = true;

    [Header("Screen Boundaries")]
    [SerializeField] private float _rightBound = 11f;
    [SerializeField] private float _leftBound = -11f;
    [SerializeField] private float _topBound = 5f;
    [SerializeField] private float _bottomBound = -10.5f;

    [Header("Enemy Variables")]
    [SerializeField] private float _enemySpeed = 2.50f;
    [SerializeField] private float _evadingCooldown = 1.5f;
    private bool _evading = false;
    private string _evadeDirection = "left";
    private Player _player;

    private SpawnManager _spawnManager;

    // create handle to animator component
    private Animator _enemyAnimator; // clean this up? 
    private AudioSource _explosionAudioSource;
    [SerializeField] private GameObject _explosionPreFab;
    [SerializeField] float _trackingTriggerDistance = 3.0f;

    [SerializeField] private GameObject _laserPrefab;
    private float _fireRate = 3.0f;
    private float _canFire = -1;

    private bool _playerDestroyed;
    private Quaternion _targetRotation;
    private float _rotationSpeed = .05f;
    private Transform _tmpTransform;

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
    }

    void Update()
    {

        CalculateMovement();

        if (Time.time > _canFire)
        {
            FireLaser();
        }
    }

    public void StopMoving()
    {
        _canMove = false;
    }
    void CalculateMovement()
    {
        if (!_evading) 
        {
            transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
        } else
        {
            if (_evadeDirection == "right")
            {
                transform.Translate(Vector3.right * _enemySpeed * Time.deltaTime, Space.World);
            } else
            {
                transform.Translate(Vector3.left * _enemySpeed * Time.deltaTime, Space.World);
            }
        }


        if (transform.position.y <= -5.44f) // if moving off the bottom of screen, respawn at random x pos at top 
        {
            float randomX = Random.Range(-9.5f, 9.5f);
            transform.position = new Vector3(randomX, 7.5f, 0);
        }
    }

    public void TakeEvasiveAction(string direction)
    {
        if (_evading == false) 
        {
            _evading = true;
            _evadeDirection = direction;
            StartCoroutine(EvadingCooldown());
        }
    }

    IEnumerator EvadingCooldown()
    {
        if (_evadeDirection == "right")
        {
                transform.Rotate(0, 0, 90);
        }
        else
        {
                transform.Rotate(0, 0, -90);
        }
            
        yield return new WaitForSeconds(_evadingCooldown);

        if (_evadeDirection == "right")
        {
            transform.Rotate(0, 0, -90);
        }
        else
        {
            transform.Rotate(0, 0, 90);
        }

        _evading = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("evasive enemy just hit by " + other.tag);
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
        // _enemyAnimator.SetTrigger("OnEnemyDeath");
        Instantiate(_explosionPreFab, transform.position, Quaternion.identity);
        // _enemySpeed /= 1.25f;
        // Destroy(GetComponent<Collider2D>());
        // Destroy(GetComponent<Rigidbody2D>());
        // _canFire = Time.time + 2.8f; // make it so enemy can't fire after they've been destroyed.
        Destroy(gameObject, .2f);
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
        _playerDestroyed = true;

    }
}
