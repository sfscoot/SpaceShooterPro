using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreadnaughtGrenade : MonoBehaviour
{
    [SerializeField] private float _screenYBoundary = -3.65f;
    [SerializeField] private float _screenXBoundary = 10.5f;
    [SerializeField] private float _grenadeSpeed;
    [SerializeField] private float _grenadeTimerMin;
    [SerializeField] private float _grenadeTimerMax;
    private float _grenadeTimer;
    private Collider2D _collider;
    [SerializeField] private GameObject _explosionPreFab;
    private GameObject _explosion;
    private AudioSource _explosionAudioSource;

    private void Start()
    {
        if (_explosionPreFab == null) Debug.LogError("Dreadnaught Grenade - explosion prefab not assigned");
        _collider = GetComponent<Collider2D>();
        _collider.enabled = true;
        _explosionAudioSource = GetComponent<AudioSource>();
        if (_explosionAudioSource == null) Debug.LogError("Dreadnaught Grenade - explosion audio source not assigned");
        StartCoroutine(ExplosionDelay());
    }
    void Update()
    {
        MoveDown();
    }

    void MoveDown()
    {
        transform.Translate(Vector3.down * _grenadeSpeed * Time.deltaTime);

        if (transform.position.y < _screenYBoundary || transform.position.x > _screenXBoundary)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator ExplosionDelay()
    {
        _grenadeTimer = Random.Range(_grenadeTimerMin, _grenadeTimerMax);
        yield return new WaitForSeconds(_grenadeTimer);
        _collider.enabled = true;
        _explosionAudioSource.Play();
        _explosion = Instantiate(_explosionPreFab, transform.position, Quaternion.identity);
        _explosion.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        Destroy(this.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
                Destroy(gameObject);
            }
        }
    }
}
