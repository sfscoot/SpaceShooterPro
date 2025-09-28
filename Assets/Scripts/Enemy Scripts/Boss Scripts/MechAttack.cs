using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MechAttack : MonoBehaviour
{
    private Mech[] _mechArmy;
    private float[] xTargets = { -9f, -6f, -3f, 0f, 3f, 6f, 9f };
    private float _tmpYPos;
    // private float _verticalTarget = 3f;
    // private bool _mechAppearOn = false;
    // private bool _mechsInPosition = false;
    private bool _mechBounceAttackActive = false;
    [SerializeField] private float _attack1Interval;
    [SerializeField] private float _verticalDrop;

    [SerializeField] private float _vertSpeed = 1f;
    [SerializeField] private float _mechDefaultY;
    private Vector3 _transformVertPos;
    private bool _inPosition;
    private int _outsideMechIdx;
    private SpawnManager _spawnManager;
    private int _mechsFinishedAttacking = 0;
    private int _mechsFinishedSpreading = 0;


    private void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null) Debug.LogError("spawn manager not found");
    }
    void Update()
    {
        if (_mechBounceAttackActive == true)
        {
            _mechArmy = gameObject.GetComponentsInChildren<Mech>();
            if (_mechArmy.Length == 0)
            {
                _mechBounceAttackActive = false;
                _spawnManager.BossWaveComplete();
            }   
        }
    }
    public void MechsPositionsReset()
    {
        // transform.position = Vector3.zero;
        transform.position = new Vector3(0, _mechDefaultY, 0);
        _mechArmy = gameObject.GetComponentsInChildren<Mech>();
        for (int i = 0; i < _mechArmy.Length; i++)
        {
            _mechArmy[i].transform.localPosition = new Vector3(0, 0, 0);
        }
    }
    public void MechsDropToPosition(float vTarget, bool reset)
    {
        StartCoroutine(MechsDropToPositionCoroutine(vTarget, reset));
    }

    public IEnumerator MechsDropToPositionCoroutine(float vTarget, bool reset)
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

    public void MechsSpreadToPosition()
    {
        StartCoroutine(MechsSpreadToPositionCoroutine());
    }

    public IEnumerator MechsSpreadToPositionCoroutine()
    {
        _mechArmy = gameObject.GetComponentsInChildren<Mech>();
        for (int i = 0; i < _mechArmy.Length; i++)
        {
            _mechArmy[i].MechMoveToPosition(xTargets[i]);
        }
        while (_mechsFinishedSpreading < _mechArmy.Length)
        {
            yield return null;
        }
    }

    public void ResetMechsFinishedSpreading()
    {
        _mechsFinishedAttacking = 0;
    }
    public void CountMechsFinishedSpreading()
    {
        _mechsFinishedSpreading++;
    }

    public void StartMechAttackLToR(float attackSpeed)
    {
        ResetMechsFinishedAttacking();
        StartCoroutine(MechAttackLToR(attackSpeed));
    }
    private IEnumerator MechAttackLToR(float attackSpeed) // left to right
    {
        while (_mechsFinishedAttacking < _mechArmy.Length) 
        {
            for (int i = 0; i < _mechArmy.Length; i++)
            {
                _mechArmy[i].MechAttack(attackSpeed);
                yield return new WaitForSeconds(_attack1Interval);
            }
        }
        _spawnManager.BossWaveComplete();
    }
    public void ResetMechsFinishedAttacking()
    {
        _mechsFinishedAttacking = 0;
    }
    public void CountMechsFinishedAttacking()
    {
        _mechsFinishedAttacking++;
    }
    public void StartMechAttackOutsideIn(float attackSpeed)
    {
        ResetMechsFinishedAttacking();
        StartCoroutine(MechAttackOutsideIn(attackSpeed));
    }
    private IEnumerator MechAttackOutsideIn(float attackSpeed) // outside in
    {
        _outsideMechIdx = _mechArmy.Length - 1;
        while (_mechsFinishedAttacking < _mechArmy.Length)
        {
            for (int i = 0; i <= (_mechArmy.Length / 2); i++)
            {
                _mechArmy[i].MechAttack(attackSpeed);
                if (i != Mathf.Round((_mechArmy.Length - 1) / 2)) _mechArmy[_outsideMechIdx].MechAttack(attackSpeed);
                _outsideMechIdx--;
                yield return new WaitForSeconds(_attack1Interval);
            }
        }
        _spawnManager.BossWaveComplete();
    }

    public IEnumerator StartMechBounceAttackCoroutine(float attackSpeed)
    {
        _mechBounceAttackActive = true;
        for (int i = 0; i < _mechArmy.Length; i++)
        {
            _mechArmy[i].MechBounceAttack(attackSpeed);
            yield return new WaitForSeconds(_attack1Interval);
        }
    }
}
