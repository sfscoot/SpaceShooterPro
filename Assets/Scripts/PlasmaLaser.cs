using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaLaser : MonoBehaviour
{
    [SerializeField] private float _laserSpeed = 8.0f;
    [SerializeField] private float _horizonalLaserSpeed = 5.0f;
    private bool _isEnemyLaser = false;

    string _laserDirection = "down";
    void Update()
    {
                MoveDown();
    }

    void MoveDown()
    {
        transform.Translate(Vector3.down * _laserSpeed * Time.deltaTime);

        if (transform.position.y < -5.0)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isEnemyLaser == true && other.tag == "Player")
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
