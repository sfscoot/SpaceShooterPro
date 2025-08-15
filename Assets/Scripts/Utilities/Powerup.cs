using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private float _powerupSpeed = 3.0f;
    [SerializeField] private GameObject _explosionPreFab;

    AudioSource _powerupAudioSource;
    AudioSource _laserReloadAudioSource;
    private GameObject _player;
    private bool _moveTowardPlayer = false;

    [SerializeField]  private float _pickupCollectSpeed = 50f;

    // IDs for powerups
    // 0 = Triple Shot
    // 1 = Speed
    // 2 = Shields
    // 3 = ammo
    [SerializeField] private int _powerupID;

    private void Start()
    {
        _powerupAudioSource = GameObject.Find("Powerup_AudioSource").GetComponent<AudioSource>();
        _player = GameObject.Find("Player");
        if ( _powerupAudioSource == null )
        {
            Debug.LogError("Powerup: Powerup Audio Source not found");
        }
        _laserReloadAudioSource = GameObject.Find("Laser_Reload_AudioSource").GetComponent <AudioSource>();

        if ( _laserReloadAudioSource == null )
        {
            Debug.LogError("Powerup: laser reload audio source missing");
        }
        if (_player == null ) { Debug.LogError("Player not found " + this.name); }

    }
    void Update()
    {
        if (_moveTowardPlayer) 
        { 
            MoveTowardPlayer();
        } else
        {
            MoveDown();
        }
    }

    void MoveDown()
    {
        transform.Translate(Vector3.down * _powerupSpeed * Time.deltaTime);
        if (transform.position.y < -5.44f)
        {
            Destroy(gameObject);
        }
    }
    public void MoveTowardPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _pickupCollectSpeed * Time.deltaTime);
        transform.rotation = LocalLookAt2D(_player.transform.position, transform.position);
    }

    public void SetMoveTowardPlayer()
    {
        _moveTowardPlayer = true;
    }

    public Quaternion LocalLookAt2D(Vector2 target, Vector2 center)
    {
        Vector3 diff = target - center; // gets a vector - distance and x/y direction 
        diff.Normalize(); // on the curve of a circle with a radius of 1
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0f, 0f, rot_z + 90); // rot_z + 90
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null) 
            {
                // AudioSource.PlayClipAtPoint(_powerupClip, transform.position, 0.5f); // plays the clip before the powerup is destroyed
                switch (_powerupID)
                {
                    case 0:
                        _powerupAudioSource.Play();
                        player.TripleShotActive();
                        break;
                    case 1:
                        _powerupAudioSource.Play();
                        player.SpeedPowerupActive();
                        break;
                    case 2:
                        _powerupAudioSource.Play();
                        player.ShieldPowerupActive();
                        break;
                    case 3:
                        _laserReloadAudioSource.Play();
                        player.AmmoReload();
                        break;
                    case 4:
                        _laserReloadAudioSource.Play();
                        player.LivesPowerup();
                        break;
                    case 5:
                        _powerupAudioSource.Play();
                        player.MissilePowerupActive();
                        break;
                    case 6:
                        _powerupAudioSource.Play();
                        player.MineLauncherActive();
                        break;
                    case 7:
                        _powerupAudioSource.Play();
                        player.LeftRightSwapActive();
                        break;
                    case 8:
                        _powerupAudioSource.Play();
                        player.HomingMissileActive();
                        break;
                    default:
                        Debug.LogError("unknown powerup encountered" + _powerupID);
                        break;
                }
            }
            Destroy(gameObject);
        }
        if (other.tag == "EnemyLaser" || other.tag == "Laser")
        {
            _explosionPreFab.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            Instantiate(_explosionPreFab, transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
