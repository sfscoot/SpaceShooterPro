using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;


public class BossSlider : MonoBehaviour
{
    [SerializeField] private Slider _bossSlider;
    [SerializeField] private int _currentDamage = 0;
    [SerializeField] private int _maxDamage = 100;

    public void UpdateDamageSlider(int current, int max)
    {
        float Damage =  (float)current / max;
        _bossSlider.value = Damage;
    }
}
