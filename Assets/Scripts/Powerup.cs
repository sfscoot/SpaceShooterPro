using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _powerupSpeed = 3.0f;
    [SerializeField]
    private AudioClip _powerupClip;
    [SerializeField]
    private AudioClip _laserReloadClip;
    [SerializeField]
    AudioSource _powerupAudioSource;
    [SerializeField]
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
        _laserReloadAudioSource = GameObject.Find("Laser_Reload_AudioSource").GetComponent <AudioSource>();
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
                 _powerupAudioSource.clip = _powerupClip;
                _powerupAudioSource.Play();
                switch (_powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        AudioSource.PlayClipAtPoint(_powerupClip, transform.position, 0.5f); // plays the clip before the powerup is destroyed
                        break;
                    case 1:
                        player.SpeedPowerupActive();
                        AudioSource.PlayClipAtPoint(_powerupClip, transform.position, 0.5f); // plays the clip before the powerup is destroyed
                        break;
                    case 2:
                        player.ShieldPowerupActive();
                        AudioSource.PlayClipAtPoint(_powerupClip, transform.position, 0.5f); // plays the clip before the powerup is destroyed
                        break;
                    case 3:
                        player.AmmoReload();
                        AudioSource.PlayClipAtPoint(_laserReloadClip, transform.position, 0.5f); // plays the clip before the powerup is destroyed
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
