using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UIElements;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    // private float _fillSpeed = 0.5f;

    void Start()
    {
        transform.GetComponent<Slider>().value = 1;
    }
    void Update()
    {
        // IncrementProgress(_fillSpeed * Time.deltaTime);        
    }
    public void IncrementProgress(float _progress)
    {
        transform.GetComponent<Slider>().value -= _progress;
        if (transform.GetComponent<Slider>().value == 0)
        {
            gameObject.SetActive(false);
        }
    }
    public void ActivateProgressBar()
    {
        gameObject.SetActive(true);
    }

    public void DeactivateProgressBar() 
    {
        gameObject.SetActive(false);
    }

}
