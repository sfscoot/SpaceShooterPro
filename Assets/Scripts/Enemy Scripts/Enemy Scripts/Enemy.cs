using System.Collections;
using UnityEngine;
using static UnityEditor.LightmapEditorSettings;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _enemySpeed = 2.50f;
    private Player _player;
    private SpawnManager _spawnManager;

    // create handle to animator component
    private Animator _enemyAnimator; // clean this up? 
    private AudioSource _explosionAudioSource;
    [SerializeField] private GameObject _explosionPreFab;

    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _homingMissilePrefab;
    private GameObject _enemyWeapon;
    [SerializeField]  private float _fireRate = 3.0f;
    [SerializeField] private float _canFire = -1;
    [SerializeField] private float _laserOffset;

    [SerializeField] private float _raycastRange = 5.0f;

    private bool _enemyDestroyed = false;
    private bool _raycastActive = true;


    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null) Debug.LogError("Player not assigned " + this.tag + "  " + this.gameObject.name);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();


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

        _canFire = Time.time + Random.Range(0.25f, 1.0f); // add 1 second delay before enemy can fire, otherwise they fire as soon as they're spawned
    }

    void Update()
    {
        CalculateMovement();
        if (Time.time > _canFire  && transform.position.x <= 7)
        {
            FireLaser();
        }

        CheckForPowerupsInSights();

        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, _raycastRange);
        Debug.DrawRay(transform.position, Vector2.down * _raycastRange, Color.red);

        // If it detects something...
        if (hit && hit.collider.gameObject.tag == "Powerup" && _raycastActive == true)
        {
            FireLaser();
        }
    }


    void CalculateMovement()
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
        if (transform.position.y <= -5.44f && _enemyDestroyed == false) // if moving off the bottom of screen, respawn at random x pos at top 
        {
            float randomX = Random.Range(-9.5f, 9.5f);
            transform.position = new Vector3(randomX, 7.5f, 0);
        }
    }

    private void CheckForPowerupsInSights()
    { 
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, _raycastRange);
        Debug.DrawRay(transform.position, Vector2.down* _raycastRange, Color.red);

        // If it detects something...
        if (hit && hit.collider.gameObject.tag == "Powerup" && _raycastActive == true)
        {
            FireLaser();
        }
    }
private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _spawnManager.GetComponent<SpawnManager>().WaveEnemyDefeated();
            Debug.Log("dead enemy");
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
            Debug.Log("dead enemy");
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
        _enemyDestroyed = true;
        _enemyAnimator.SetTrigger("OnEnemyDeath");
        // Instantiate(_explosionPreFab, transform.position, Quaternion.identity);
        _enemySpeed /= 1.25f;
        Destroy(GetComponent<Collider2D>());
        Destroy(GetComponent<Rigidbody2D>());
        _raycastActive = false;
        _canFire = Time.time + 10.0f; // make it so enemy can't fire after they've been destroyed.
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
        } else
        {
            _enemyWeapon = Instantiate(_laserPrefab, transform.position + new Vector3(0, _laserOffset, 0), Quaternion.identity); // spawn outside the collider
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
        Debug.Log("enemy - set player destroyed is called");
        // _playerDestroyed = true;
    }
}
