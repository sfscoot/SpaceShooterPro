using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechAttack : MonoBehaviour
{
    private Mech[] _mechArmy;
    private float[] xTargets = { -9f, -6f, -3f, 0f, 3f, 6f, 9f };

    void Start()
    {
        _mechArmy = transform.GetComponentsInChildren<Mech>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MechsAppear()
    {
        StartCoroutine(BringOnTheMechs());
    }

    IEnumerator BringOnTheMechs()
    {
        foreach (var mech in _mechArmy)
        {

            mech.gameObject.SetActive(true);
            mech.MechsAppear();
        }
    }
}
