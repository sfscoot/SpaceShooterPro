using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _powerupSpeed = 3.0f;

    AudioSource _powerupAudioSource;
    AudioSource _laserReloadAudioSource;

    // IDs for powerups
    // 0 = Triple Shot
    // 1 = Speed
    // 2 = Shields
    // 3 = ammo
    [SerializeField]
    private int _powerupID;

    private void Start()
    {
        _powerupAudioSource = GameObject.Find("Powerup_AudioSource").GetComponent<AudioSource>();
        if ( _powerupAudioSource == null )
        {
            Debug.LogError("Powerup: Powerup Audio Source not found");
        }
        _laserReloadAudioSource = GameObject.Find("Laser_Reload_AudioSource").GetComponent <AudioSource>();

        if ( _laserReloadAudioSource == null )
        {
            Debug.LogError("Powerup: laser reload audio source missing");
        }
    }
    void Update()
    {
        transform.Translate(Vector3.down * _powerupSpeed * Time.deltaTime);
        if (transform.position.y < -5.44f)
        {
            Destroy(gameObject);
        }
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
                    default:
                        Debug.LogError("unknown powerup encountered" + _powerupID);
                        break;
                }
            }
            Destroy(gameObject);
        }
    }
}
