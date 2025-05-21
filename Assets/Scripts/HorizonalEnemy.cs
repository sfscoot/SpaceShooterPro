using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizonalEnemy : MonoBehaviour
{
    [SerializeField]
    private float _enemySpeed = 2.50f;

    private int _direction = 1;

    [Header("Screen Boundaries")]
    [SerializeField] private float _rightBound = 11f;
    [SerializeField] private float _leftBound = -11f;
    [SerializeField] private float _topBound = 11f;
    [SerializeField] private float _bottomBound = -10.5f;
    [SerializeField] private float _topBoundOffset = 5;
    [SerializeField] private GameObject _explosionPreFab;

    private float _randomY;

    // create handle to animator component
    private Animator _enemyAnimator;
    private AudioSource _explosionAudioSource;

    [SerializeField]
    private Player _player;

    [SerializeField]
    private GameObject _laserPrefab; // leaving this in for later implementation of this enemy destroying powerups.
    private float _fireRate = 3.0f;
    private float _canFire = -1;

    private void Start()
    {
        _explosionAudioSource = GetComponent<AudioSource>();

    }

    void Update()
    {
        CalculateMovement();
        if (Time.time > _canFire)
        {
            Debug.Log("firing laser");
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        Debug.Log("calc movement");
        // transform.Translate(Vector3.right * (2 * Time.deltaTime));
        transform.Translate(Vector3.right * (_enemySpeed * Time.deltaTime) * _direction);
        Debug.Log("just transformed");
        if (transform.position.x <= _leftBound)
        {
            Debug.Log("past the left bound " + transform.position.x);
            _randomY = Random.Range((_topBound - _topBoundOffset), 0.0f);
            transform.position = new Vector3(_leftBound, _randomY, 0);
            _direction = 1;
        }
        else if (transform.position.x >= _rightBound)
        {
            Debug.Log("past the right bound " + transform.position.x);
            _randomY = Random.Range((_topBound - _topBoundOffset), 0.0f);
            transform.position = new Vector3(_rightBound, _randomY, 0);
            _direction = -1;
        }
        Debug.Log("leaving calculating movement");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("just collided with: " +  other);
        // _explosionAudioSource.Play();
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }
            PlayEnemyDeathSequence();
        }

        if (other.tag == "Laser")
        {
            // _explosionAudioSource.Play();
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddToScore(10);
            }
            PlayEnemyDeathSequence();
        }

        if (other.tag == "Missile")
        {
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
            // _explosionAudioSource.Play();
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
        /* Debug.Log("Playing enemy death sequence");
        _enemyAnimator.SetTrigger("OnEnemyDeath");
        Debug.Log("should have started the animator");
        _enemySpeed /= 1.25f;
        Destroy(GetComponent<Collider2D>());
        Destroy(GetComponent<Rigidbody2D>());
        _canFire = Time.time + 2.8f; // make it so enemy can't fire after they've been destroyed.
        */
        Destroy(gameObject);

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
        // _playerDestroyed = true;
    }
}
