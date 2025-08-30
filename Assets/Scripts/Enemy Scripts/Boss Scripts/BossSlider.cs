using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;


public class BossSlider : MonoBehaviour
{
    [SerializeField] private Slider _bossSlider;
    [Header("Stats")]
    [SerializeField] private int _currentDamage = 0;
    [SerializeField] private int _maxDamage = 100;


    // Update is called once per frame
    void Update()
    {
    }
    public void UpdateDamageSlider(int current, int max)
    {
        float Damage =  (float)current / max;
        _bossSlider.value = Damage;
    }
}
