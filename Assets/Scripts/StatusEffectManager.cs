using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _energizedEffect;
    private bool _isEnergized;
    public void StartEnergizedEffect(float duration)
    {
        _isEnergized = true;
        _energizedEffect.SetActive(true);
        _energizedEffect.transform.Find("Radial_Progress_Bar").GetComponent<RadialProgressBar>().ActivateCountdown(duration);
        StartCoroutine(EndEnergizedEffect(duration));

    }

    IEnumerator EndEnergizedEffect(float duration)
    {
        yield return new WaitForSeconds(duration);
        _isEnergized = false;
        _energizedEffect.SetActive(false);
    } 
}
