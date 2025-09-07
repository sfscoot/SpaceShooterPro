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
    [SerializeField] private float _verticalDrop;

    [SerializeField] private float _vertSpeed = 1f;
    private Vector3 _transformVertPos;
    private bool _inPosition;
    private int _outsideMechIdx;
    private SpawnManager _spawnManager;


    private void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null) Debug.LogError("spawn manager not found");
    }
    void Update()
    {

    }

    public void BringOnTheMechs()  // old - not using any more
    {
        _mechArmy = gameObject.GetComponentsInChildren<Mech>();
        for (int i = 0; i < _mechArmy.Length; i++)
        {
            _mechArmy[i].MechLineUp(xTargets[i], _verticalDrop);
        }
    }

    public void MechsPositionsReset()
    {
        transform.position = Vector3.zero;
    }
    public void MechsDropToPosition(float vTarget, bool reset)
    {
        StartCoroutine(MechsDropToPositionCoroutine(vTarget, reset));
    }

    IEnumerator MechsDropToPositionCoroutine(float vTarget, bool reset)
    {
        _inPosition = false;
        _mechArmy = gameObject.GetComponentsInChildren<Mech>();
        transform.position = _mechArmy[3].transform.position;
        if (reset) vTarget = 0f;

        while (_inPosition == false)
        {

            if (transform.position.y >= vTarget)
            {
                _tmpYPos = transform.position.y - vTarget;
                if (reset) _tmpYPos = 0;
                _transformVertPos = new Vector3(transform.position.x, _tmpYPos, 0);
                transform.position = Vector3.Lerp(transform.position, _transformVertPos, Time.deltaTime * _vertSpeed);
            }
            yield return null;
            if (transform.position.y <= vTarget) _inPosition = true;
        }
    }

    public void MechsSpreadToPosition()  // old - not using any more
    {
        _mechArmy = gameObject.GetComponentsInChildren<Mech>();
        for (int i = 0; i < _mechArmy.Length; i++)
        {
            _mechArmy[i].MechSpreadToPosition(xTargets[i]);
        }
    }
    public void ResetTheMechs()
    {
        _mechArmy = gameObject.GetComponentsInChildren<Mech>();
        for (int i = 0; i < _mechArmy.Length; i++)
        {
            _mechArmy[i].MechReset();
        }
    }

    public void StartMechAttackLToR(float attackSpeed)
    {
        StartCoroutine(MechAttackLToR(attackSpeed));
    }
    private IEnumerator MechAttackLToR(float attackSpeed) // left to right
    {
        for (int i = 0; i < _mechArmy.Length; i++)
        {
            _mechArmy[i].MechAttack(attackSpeed);
            yield return new WaitForSeconds(_attack1Interval);
        }
        _spawnManager.BossWaveComplete();
    }

    public void StartMechAttackOutsideIn(float attackSpeed)
    {
        StartCoroutine(MechAttackOutsideIn(attackSpeed));
    }
    private IEnumerator MechAttackOutsideIn(float attackSpeed) // outside in
    {
        _outsideMechIdx = _mechArmy.Length - 1;
        for (int i = 0; i <= (_mechArmy.Length / 2); i++)
        {
            _mechArmy[i].MechAttack(attackSpeed);
             if (i !=  Mathf.Round((_mechArmy.Length - 1) / 2)) _mechArmy[_outsideMechIdx].MechAttack(attackSpeed);
            _outsideMechIdx--;
            yield return new WaitForSeconds(_attack1Interval);
        }
        _spawnManager.BossWaveComplete();
    }

    public void StartMechBounceAttack(float attackSpeed)
    {
        StartCoroutine(MechBounceAttack(attackSpeed));
    }

    private IEnumerator MechBounceAttack(float attackSpeed)
    {
        for (int i = 0; i < _mechArmy.Length; i++)
        {
            _mechArmy[i].MechBounceAttack(attackSpeed);
            yield return new WaitForSeconds(_attack1Interval);
        }

    }

    public IEnumerator MechAttackBounce(float attackSpeed) // outside in
    {
        for (int i = 0; i < _mechArmy.Length; i++)
        {
            _mechArmy[i].MechBounceAttack(attackSpeed);
            yield return new WaitForSeconds(_attack1Interval);
        }
    }

    IEnumerator MechAttackOdds(float attackSpeed) // left to right
    {
        for (int i = 0; i < _mechArmy.Length; i++)
        {
            // _mechArmy.SetActive(true);
            if ((i + 1) % 2 != 0)
            { 
                _mechArmy[i].MechAttack(attackSpeed);
            }
        }
        yield return new WaitForSeconds(_attack1Interval);
        for (int i = 0; i < _mechArmy.Length; i++)
        {
            // _mechArmy.SetActive(true);
            if ((i + 1) % 2 == 0)
            {
                _mechArmy[i].MechAttack(attackSpeed);
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
