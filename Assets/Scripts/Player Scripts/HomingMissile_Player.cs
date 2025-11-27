using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile_Player : MonoBehaviour
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

    private GameObject _targetEnemy;

    private void Start()
    {
        _explosionAudioSource = GetComponent<AudioSource>();
        if (_explosionAudioSource == null)
        {
            Debug.LogError("Error: Explosion Audio Source not found");
        }
    }
    void Update()
    {
        CalculateMovement();
    }

    void CalculateMovement()
    {
        if (_targetEnemy != null) 
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetEnemy.transform.position, _missileSpeed * Time.deltaTime);
            transform.rotation = LocalLookAt2D(transform.position, _targetEnemy.transform.position);
        } else
        {
            GameObject _explosion = Instantiate(_explosionPreFab, transform.position, Quaternion.identity);
            _explosion.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            Destroy(this);
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
        Debug.Log("in missile cooldown");
        yield return new WaitForSeconds(_trackingCooldown);
        _targetReleased = true;
        _targetAcquired = false;
        transform.eulerAngles = new Vector3(0f, 0f, 0f);
        Debug.Log("target is released - ");
        _explosion = Instantiate(_explosionPreFab, transform.position, Quaternion.identity);
        _explosion.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        Destroy(this);
    }

    public void MissileTarget(GameObject targetEnemy)
    {
        _targetEnemy = targetEnemy;
        Debug.Log($"Target enemy is {targetEnemy.name}");
        StartCoroutine(MissileCooldown());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        /*
        if (other.tag == "Player")
        {
            GameObject _explosion = Instantiate(_explosionPreFab, transform.position, Quaternion.identity);
            _explosion.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            // Instantiate(_explosionPreFab, transform.position, Quaternion.identity);
            _explosionAudioSource.Play();

            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
                Destroy(gameObject);
            }
        }

        if (other.tag == "PlayerLaser")
        {
            GameObject _explosion = Instantiate(_explosionPreFab, transform.position, Quaternion.identity);
            _explosion.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            // Instantiate(_explosionPreFab, transform.position, Quaternion.identity);
            _explosionAudioSource.Play();
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
         */
    }
}
