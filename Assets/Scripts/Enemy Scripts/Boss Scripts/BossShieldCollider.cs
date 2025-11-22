using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShieldCollider : MonoBehaviour
{
    [SerializeField] private GameObject _explosionPreFab;
    private GameObject _explosion;
    private Vector3 _explosionPosition;

    private void Start()
    {
        if (_explosionPreFab == null) Debug.LogError("explosion prefab missing");
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        _explosionPosition = other.transform.position;


        if (other.tag == "PlayerWeapon")
        {
            other.gameObject.SetActive(false);
            _explosion = Instantiate(_explosionPreFab, other.transform.position, Quaternion.identity);
            _explosion.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

            // explosionAudioSource.Play();
        }
    }
}
