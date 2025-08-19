using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MechAttack : MonoBehaviour
{
    private Mech[] _mechArmy;
    private float[] xTargets = { -9f, -6f, -3f, 0f, 3f, 6f, 9f };
    private float _tmpYPos;
    private float _verticalTarget = 3f;
    private bool _mechAppearOn = false;
    private bool _mechsInPosition = false;
    [SerializeField] private float _attack1Interval;
    [SerializeField] private float _attack1Speed;
    [SerializeField] private float _verticalDrop;

    [SerializeField] private float _vertSpeed = 1f;
    private Vector3 _transformVertPos;


    void Start()
    {
        _mechArmy = gameObject.GetComponentsInChildren<Mech>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            BringOnTheMechs();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(MechAttackLToR());
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            _mechsInPosition = false;
            StartCoroutine(MechAttackReset());
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine(MechAttackOdds());
        }
    }

    private void BringOnTheMechs()
    {
        for (int i = 0; i < _mechArmy.Length; i++) {
            // _mechArmy.SetActive(true);
            _mechArmy[i].MechAppear(xTargets[i], _verticalDrop);
        }
    }
    IEnumerator MechAttackLToR() // left to right
    {
        for (int i = 0; i < _mechArmy.Length; i++)
        {
            // _mechArmy.SetActive(true);
            _mechArmy[i].MechAttack1(_attack1Speed);
            yield return new WaitForSeconds(_attack1Interval);
        }
    }

    IEnumerator MechAttackOdds() // left to right
    {
        for (int i = 0; i < _mechArmy.Length; i++)
        {
            // _mechArmy.SetActive(true);
            if ((i + 1) % 2 != 0)
            { 
                _mechArmy[i].MechAttack1(_attack1Speed);
            }
        }
        yield return new WaitForSeconds(_attack1Interval);
        for (int i = 0; i < _mechArmy.Length; i++)
        {
            // _mechArmy.SetActive(true);
            if ((i + 1) % 2 == 0)
            {
                _mechArmy[i].MechAttack1(_attack1Speed);
            }
        }
    }

    IEnumerator MechAttackReset()
    {
        _verticalTarget = transform.position.y - _verticalDrop;
        while (_mechsInPosition == false)
        {

            if (transform.position.y >= _verticalTarget)
            {
                _tmpYPos = transform.position.y - _verticalDrop;
                _transformVertPos = new Vector3(transform.position.x, _tmpYPos, 0);
                transform.position = Vector3.Lerp(transform.position, _transformVertPos, Time.deltaTime * _vertSpeed);
                yield return null;
            }
            else
            {
                _mechsInPosition = true;
            }
        }
    }
}
