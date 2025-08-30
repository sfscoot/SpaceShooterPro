using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private float _enemySpeed = 2.50f;
    private Player _player;

    // create handle to animator component
    private Animator _bossAnimator;
    private AudioSource _explosionAudioSource;
    [SerializeField] private BossSlider _bossSlider;
    [SerializeField] private int _bossDamage = 0;
    [SerializeField] private int _maxBossDamage = 100;
    [SerializeField] private GameObject _explosionPreFab;
    [SerializeField] private GameObject _dreadnaughtFront;
    [SerializeField] private GameObject _dreadnaughtRear;

    private DreadnaughtFront _df;
    private DreadnaughtRear _dr;
    private bool _bossInPosition = false;
    private bool _dreadnaughtFrontGunsDead = false;
    private bool _dreadnaughtRearGunsDead = false;
    private SpawnManager _spawnManager;

    // reset player lives
    // final attack
    // boss explosion
    // tell them it's the boss round
    // if they die, restart just the boss round
    // turn on powerup spawning for just ammo and lives
    private void Start()
    {
        transform.position = new Vector3(17.5f, 5.5f, 0f);

        _explosionAudioSource = GetComponent<AudioSource>();
        if (_explosionAudioSource == null)
        {
            Debug.LogError("Error: Explosion Audio Source not found");
        }
        _bossAnimator = GetComponent<Animator>();
        if (_bossAnimator == null) Debug.LogError("Error: Enemy Animator Audio Source not found");
        _bossAnimator.SetTrigger("BossEnters");

        _df = _dreadnaughtFront.GetComponent<DreadnaughtFront>();
        if (_df == null) Debug.LogError("dreadnaught front not found");

        _dr = _dreadnaughtRear.GetComponent<DreadnaughtRear>();
        if (_dr == null) Debug.LogError("dreadnaught rear not found");

        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null) Debug.LogError("Player object not found");

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null) Debug.LogError("spawn manager not found");

        StartCoroutine("SweepAndShoot"); // first Boss attack launches automatically

        _bossSlider = GameObject.Find("BossSlider").GetComponent<BossSlider>();
        if (_bossSlider == null) Debug.LogError("boss slider not found");
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

    public void ActivateBoss()
    {
        gameObject.SetActive(true);
    }

    public void BossSeparates()
    {
        _bossAnimator.SetTrigger("BossSeparates");
    }

    public void BossRejoins()
    {
        _bossAnimator.SetTrigger("BossRejoins");
    }

    public void BossInPosition()
    {
        _bossInPosition = true;
    }

    IEnumerator SweepAndShoot()
    {
        while (_bossInPosition == false)
        {
            yield return null;
        }
        _df.SweepAttack();
        _dr.SweepAttack();

        while (_dreadnaughtFrontGunsDead == false && _dreadnaughtRearGunsDead == false)
        {
            yield return null;
        }
        
    }

    public void DreadnaughtGunsDead(string _frontOrRear)
    {
        if (_frontOrRear == "Front")
        {
            _dreadnaughtFrontGunsDead = true;
        }
        if (_frontOrRear == "Rear")
        {
            _dreadnaughtRearGunsDead = true;
        }

        if (_dreadnaughtRearGunsDead && _dreadnaughtFrontGunsDead)
        {
            _spawnManager.BossWaveComplete();
        }
    }

    public void BossDamage(int damage)
    {
        _bossDamage += damage;
        _bossSlider.UpdateDamageSlider(_bossDamage, _maxBossDamage);
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
        
    }

}
