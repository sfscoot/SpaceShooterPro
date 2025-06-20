using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

    [SerializeField]
    private float _laserSpeed = 8.0f;
    private bool _isEnemyLaser = false;
    string _laserDirection = "down";
    void Update()
    {
        if (_isEnemyLaser)
        {
            if (_laserDirection == "down")
            {
                MoveDown();
            } else if (_laserDirection == "left")
            {
                MoveLeft();
            } else if (_laserDirection == "right")
            {
                MoveRight();
            }

        }
        else
        {
            MoveUp();
        }
    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * _laserSpeed * Time.deltaTime);

        if (transform.position.y > 8)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(gameObject);
        }
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

    void MoveLeft()
    {
        transform.Rotate(0, 0, 90, Space.Self);
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

    void MoveRight()
    {
        transform.Rotate(0, 0, 90, Space.Self);
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

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    public void AssignLaserDirection(string direction)
    {
        _laserDirection = direction;
        Debug.Log("assigning laser directino to " +  _laserDirection);
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
