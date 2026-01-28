using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    private bool _targetAcquired = false;
    private bool _targetReleased = false;
    [SerializeField] private float _trackingCooldown = 3.0f;
    private GameObject _player;
    [SerializeField] float _trackingTriggerDistance = 4.0f;
    [SerializeField] private float _missileSpeed = 2.50f;
    private AudioSource _explosionAudioSource;
    [SerializeField] private GameObject _explosionPreFab;
    private GameObject _explosion;

    private void Start()
    {
        _player = GameObject.Find("Player");
        if (_player == null)
        {
            Debug.LogError("Player not assigned " + this.tag + "  " + this.gameObject.name);
        }
        _explosionAudioSource = GetComponent<AudioSource>();
        if (_explosionAudioSource == null)
        {
            Debug.LogError("Error: Explosion Audio Source not found");
        }
    }
    void Update()
    {
        CheckAttackPlayer();
        CalculateMovement();
    }

    void CalculateMovement()
    {
        if (_targetAcquired == false || _targetReleased == true)
        {
            transform.Translate(Vector3.down * _missileSpeed * Time.deltaTime);
        }
        else if (_targetAcquired == true && _targetReleased == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _missileSpeed * Time.deltaTime);
            transform.rotation = LocalLookAt2D(_player.transform.position, transform.position);
        }

        if (transform.position.y < -5.0)
        {
            Destroy(gameObject);
        }
    }

    public Quaternion LocalLookAt2D(Vector2 target, Vector2 center)
    {
        Vector3 diff = target - center; // gets a vector - distance and x/y direction 
        diff.Normalize(); // on the curve of a circle with a radius of 1
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0f, 0f, rot_z + 90); // rot_z + 90
    }


    IEnumerator MissileCooldown()
    {
        yield return new WaitForSeconds(_trackingCooldown);
        _targetReleased = true;
        _targetAcquired = false;
        transform.eulerAngles = new Vector3(0f, 0f, 0f);
    }

    void CheckAttackPlayer()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (distance <= _trackingTriggerDistance)
        {
            _targetAcquired = true;
            _targetReleased = false;
            StartCoroutine(MissileCooldown());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _explosion = Instantiate(_explosionPreFab, transform.position, Quaternion.identity);
            _explosion.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
                Destroy(gameObject);
            }
        }

        if (other.tag == "PlayerWeapon")
        {
            _explosion = Instantiate(_explosionPreFab, transform.position, Quaternion.identity);
            _explosion.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
