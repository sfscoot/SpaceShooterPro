using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechAttack : MonoBehaviour
{
    private Mech[] _mechArmy;
    private float[] xTargets = { -9f, -6f, -3f, 0f, 3f, 6f, 9f };
    private bool _mechAppearOn = false;

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
    }

    private void BringOnTheMechs()
    {
        for (int i = 0; i < _mechArmy.Length; i++) {
            // _mechArmy.SetActive(true);
            _mechArmy[i].MechAppear(xTargets[i], 3f);
        }
    }
}
