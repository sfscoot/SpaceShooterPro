using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreadnaughtMissileLauncher : MonoBehaviour
{
    [SerializeField] private float _zRotationIncrement = .025f;
    [SerializeField] private float _rotationSpeed = 1;
    [SerializeField] private float _fireRate = 1.25f;
    [SerializeField] private GameObject _missilePrefab;
    [SerializeField] private float _rotationInterval;
    private AudioSource _explosionAudioSource;
    [SerializeField] private GameObject _explosionPreFab;
    private float _canFire = -1f;
    private GameObject _explosion;

    private GameObject _enemyWeapon;

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        _explosionAudioSource = GetComponent<AudioSource>();
        if (_explosionAudioSource == null)
        {
            Debug.LogError("Error: Explosion Audio Source not found");
        }
    }

    public void RotateMissileLauncher(float zRotationFactor, float zRotation)
    {
        transform.Rotate(new Vector3(0, 0, zRotationFactor * zRotation), Space.Self);
        FireLaser();
    }

    public void NewRotateMissileLauncher(float eulerZ)
    {
        //transform.eulerAngles = new Vector3(0,0, eulerZ);
        transform.eulerAngles = new Vector3(0, 0, eulerZ);
        FireLaser();
    }
    public void ResetMissileLauncherRotation(float zRotation)
    {
        transform.rotation = Quaternion.Euler(0f, 0f, zRotation);
    }

    private void FireLaser()
    {
        if (Time.time > _canFire)
        {
            _canFire = Time.time + _fireRate;
            _enemyWeapon = Instantiate(_missilePrefab, transform.position, Quaternion.identity); // spawn outside the collider
            _enemyWeapon.transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z);
            _enemyWeapon.transform.parent = transform.parent.Find("Laser_Blasts");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "PlayerLaser")
        {
            // _explosionAudioSource.Play();
            Destroy(other.gameObject);
            _explosion = Instantiate(_explosionPreFab, transform.position, Quaternion.identity);
            _explosion.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            Destroy(this.gameObject, .25f);
        }
    }
}
