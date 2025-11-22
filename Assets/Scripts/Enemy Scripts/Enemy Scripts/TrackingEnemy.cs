using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.LightmapEditorSettings;
using static UnityEngine.GraphicsBuffer;

public class TrackingEnemy : MonoBehaviour
{

    [Header("Screen Boundaries")]
    // [SerializeField] private float _rightBound = 11f;
    // [SerializeField] private float _leftBound = -11f;
    // [SerializeField] private float _topBound = 5f;
    // [SerializeField] private float _bottomBound = -10.5f;

    [Header("Enemy Variables")]
    [SerializeField] private float _enemySpeed = 2.50f;
    [SerializeField] private float _trackingCooldown = 4;
    private Player _player;

    private SpawnManager _spawnManager;

    // create handle to animator component
    private Animator _enemyAnimator; // clean this up? 
    private AudioSource _explosionAudioSource;
    [SerializeField] private GameObject _explosionPreFab;
    private GameObject _explosion;
    private bool _targetAcquired = false;
    private bool _targetReleased = false;
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
        _enemyAnimator = GetComponent<Animator>();
       // if (_enemyAnimator == null) Debug.LogError("Error: Enemy Animator Audio Source not found");
    }

    void Update()
    {
        CheckAttackPlayer();
        CalculateMovement();
        if (Time.time > _canFire)
        {
            FireLaser();
        }
    }

    void CheckAttackPlayer()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (distance <= _trackingTriggerDistance) 
        { 
            _targetAcquired = true;
            StartCoroutine(TrackingCooldown());
        }
    }

    IEnumerator TrackingCooldown()
    {
        yield return new WaitForSeconds(_trackingCooldown);
        _targetAcquired = false;
        _targetReleased = true;
        transform.eulerAngles = new Vector3(0f, 0f, 0f);
    }

    void CalculateMovement()
    {
        if (_targetAcquired == false || _targetReleased == true)
        {
            transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
        }
        else if (_targetAcquired == true && _targetReleased == false) 
        {
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _enemySpeed * Time.deltaTime);
            transform.rotation = LocalLookAt2D(_player.transform.position, transform.position);
        }

        if (transform.position.y <= -5.44f) // if moving off the bottom of screen, respawn at random x pos at top 
        {
            float randomX = Random.Range(-9.5f, 9.5f);
            transform.position = new Vector3(randomX, 7.5f, 0);
        }
    }

    public Quaternion LocalLookAt2D(Vector2 target, Vector2 center)
    {
        Vector3 diff = target - center;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0f, 0f, rot_z + 90);
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

    public void SetPlayerDestroyed()
    {
        _playerDestroyed = true;

    }
}
