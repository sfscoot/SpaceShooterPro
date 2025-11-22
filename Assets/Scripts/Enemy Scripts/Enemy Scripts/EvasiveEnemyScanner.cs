using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class EvasiveEnemyScanner : MonoBehaviour
{
    private Enemy _parent;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" || other.tag == "PlayerWeapon")

        {
            if (this.name == "Forward_Left_Sensor")
            {
                transform.parent.GetComponent<EvasiveEnemy>().TakeEvasiveAction("right");
            }
            else if (this.name == "Forward_Right_Sensor")
            {
                transform.parent.GetComponent<EvasiveEnemy>().TakeEvasiveAction("left");
            }
        }
    }
}
