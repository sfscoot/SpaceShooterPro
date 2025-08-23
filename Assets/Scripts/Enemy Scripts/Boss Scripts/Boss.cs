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
    [SerializeField] private GameObject _explosionPreFab;
    [SerializeField] private GameObject _dreadnaughtFront;
    [SerializeField] private GameObject _dreadnaughtRear;
    private DreadnaughtFront _df;
    private DreadnaughtRear _dr;
    private bool _bossInPosition = false;
    private bool _dreadnaughtFrontGunsDead = false;
    private bool _dreadnaughtRearGunsDead = false;

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
        StartCoroutine("SweepAndShoot");
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
        Debug.Log("the boss is ready to kick your ass");
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
        Debug.Log("all guns destroyed");
        // refill lives
    }

    public void DreadnaughtGunsDead(string _frontOrRear)
    {
        if (_frontOrRear == "Front")
        {
            _dreadnaughtFrontGunsDead = true;
            Debug.Log($"front guns dead {_dreadnaughtFrontGunsDead}");
        }
        if (_frontOrRear == "Rear")
        {
            _dreadnaughtRearGunsDead = true;
            Debug.Log($"rear guns dead {_dreadnaughtRearGunsDead}");
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
        
    }

    void PlayEnemyDeathSequence()
    {
        _bossAnimator.SetTrigger("OnEnemyDeath");
        // Instantiate(_explosionPreFab, transform.position, Quaternion.identity);
        _enemySpeed /= 1.25f;
        Destroy(GetComponent<Collider2D>());
        Destroy(GetComponent<Rigidbody2D>());
        Destroy(gameObject, 2.8f);
    }




}
