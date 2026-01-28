using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class EvasiveEnemy : MonoBehaviour
{
    private bool _canMove = true;

    [Header("Enemy Variables")]
    [SerializeField] private GameObject _explosionPreFab;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private float _enemySpeed = 2.50f;
    [SerializeField] private float _evadingCooldown = 1.5f;
    [SerializeField] private int _enemyPointsValue;

    private bool _evading = false;
    private string _evadeDirection = "left";
    private Player _player;

    private SpawnManager _spawnManager;
    private GameObject _explosion;

    private float _fireRate = 3.0f;
    private float _canFire = -1;

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
        if (other.tag == "Player")
        {
            _spawnManager.GetComponent<SpawnManager>().WaveEnemyDefeated();
            Player player = other.transform.GetComponent<Player>();

            if (_player != null)
            {
                _player.Damage();
                _player.AddToScore(_enemyPointsValue);
            }

            PlayEnemyDeathSequence();
        }

        if (other.tag == "PlayerWeapon")
        {
            gameObject.GetComponent<Collider2D>().enabled = false; // stops the tripleshot laser getting counted twice 
            _spawnManager.GetComponent<SpawnManager>().WaveEnemyDefeated();
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddToScore(_enemyPointsValue);
            }

            PlayEnemyDeathSequence();
        }
    }

    void PlayEnemyDeathSequence()
    {
        _explosion = Instantiate(_explosionPreFab, transform.position, Quaternion.identity);
        _explosion.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
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

}
