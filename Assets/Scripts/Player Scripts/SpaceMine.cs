using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceMine : MonoBehaviour
{
    [SerializeField]
    private float _mineDeploySpeed = 8;
    [SerializeField]
    private float _mineDeployDistance;
    private float _mineCurrentDistance;
    // Start is called before the first frame update
    void Start()
    {
        _mineDeployDistance = Random.Range(3.0f, 4.3f);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < _mineDeployDistance)    

        // if (_mineCurrentDistance < _mineDeployDistance)
        {
            transform.Translate(Vector3.up * _mineDeploySpeed * Time.deltaTime);
        }

        if (transform.position.x > 10.64 || transform.position.x < - 10.64) Destroy(gameObject);

        _mineCurrentDistance++;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "PlayerLaser" || other.tag == "EnemyLaser")
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
