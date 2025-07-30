using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaLaser : MonoBehaviour
{
    [SerializeField] private float _laserSpeed = 8.0f;
    private bool _isEnemyLaser = true;
    [SerializeField] private float _screenYBoundary = -3.65f;
    [SerializeField] private float _screenXBoundary = 10.5f;
    void Update()
    {
         MoveDown();
    }

    void MoveDown()
    {
        transform.Translate(Vector3.down * _laserSpeed * Time.deltaTime);

        if (transform.position.y < _screenYBoundary || transform.position.x > _screenXBoundary)
        {
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
