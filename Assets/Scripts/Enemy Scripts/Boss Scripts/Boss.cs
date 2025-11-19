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
    [Header("Boss Variables")]
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

    [Header("Shield Variables")]
    [SerializeField]  private GameObject _bossShield;
    [SerializeField] int _bossShieldMaxLevel = 6;
    private int _bossShieldLevel = 0;
    private bool _bossShieldActive = false;

    private Vector3 _bossShieldMax = new Vector3(5.0f, 1.0f, 0.0f);
    private Vector3 _bossShield6 = new Vector3(5.0f, 0.9f, 0.0f);
    private Vector3 _bossShield5 = new Vector3(5.0f, 0.8f, 0.0f);
    private Vector3 _bossShield4 = new Vector3(5.0f, 0.7f, 0.0f);
    private Vector3 _bossShield3 = new Vector3(5.0f, 0.6f, 0.0f);
    private Vector3 _bossShield2 = new Vector3(5.0f, 0.5f, 0.0f);
    private Vector3 _bossShield1 = new Vector3(5.0f, 0.4f, 0.0f);


    // reset player lives
    // final attack
    // boss explosion

    // if they die, restart just the boss round
    // turn on powerup spawning for just ammo and lives
    private void Start()
    {

        transform.position = new Vector3(17.5f, 5.5f, 0f);

        _explosionAudioSource = GetComponent<AudioSource>();
        if (_explosionAudioSource == null) Debug.LogError("Error: Explosion Audio Source not found");

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

        _bossSlider = GameObject.Find("BossSlider").GetComponent<BossSlider>();
        if (_bossSlider == null) Debug.LogError("boss slider not found");
    }

    public void ActivateBoss()
    {
        gameObject.SetActive(true);
    }

    public void ActivateBossShield()
    {
        _bossShieldActive = true;
        _bossShieldLevel = _bossShieldMaxLevel;
    }

    public void DamageBossShield()
    {

        _bossShieldLevel -= 1;
        Debug.Log($"boss shield has been damaged {_bossShieldLevel}");
        switch (_bossShieldLevel)
        {
            case 5:
                _bossShield.transform.localScale = _bossShield5;
                break;
            case 4:
                _bossShield.transform.localScale = _bossShield4;
                break;
            case 3:
                _bossShield.transform.localScale = _bossShield3;
                break;
            case 2:
                _bossShield.transform.localScale = _bossShield2;
                break;
            case 1:
                _bossShield.transform.localScale = _bossShield1;
                break;
            case 0:
                _bossShield.gameObject.SetActive(false);
                break;
            default:
                Debug.LogError("unknown shield level encountered" + _bossShieldLevel);
                break;
        }
}
    public void BossSeparates()
    {
        _bossAnimator.SetTrigger("BossSeparates");
    }

    
    public void BossEnters()
    {
        _bossAnimator.SetTrigger("BossEnters");
    }
    public void BossRejoins()
    {
        _bossAnimator.SetTrigger("BossRejoins");
    }

    public void BossLeaves()
    {
        _bossAnimator.SetTrigger("BossLeaves");
    }
    public void BossInPosition()
    {
        _bossInPosition = true;
    }

    public void StartSweepAndShoot()
    {
        _dreadnaughtFrontGunsDead = false;
        _dreadnaughtRearGunsDead = false;
        StartCoroutine("SweepAndShoot");
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
        Debug.Log($"damage to boss is: {_bossDamage}");
    }

    /*
    void CalculateMovement()
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
        if (transform.position.y <= -5.44f) // if moving off the bottom of screen, respawn at random x pos at top 
        {
            float randomX = Random.Range(-9.5f, 9.5f);
            transform.position = new Vector3(randomX, 7.5f, 0);
        }
    }
    */

}
