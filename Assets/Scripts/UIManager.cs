﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditorInternal;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _scoreText;

    [SerializeField]
    private TMP_Text _ammoCountTxt;
    private bool _lowAmmoWarning = false;
    private bool _lowAmmoFlicker = false;
    private int _ammoCount;

    [SerializeField]
    private TMP_Text _gameOverText;

    [SerializeField]
    private TMP_Text _gameRestartText;

    [SerializeField]
    private TMP_Text _thrusterPowerLevelText;
    [SerializeField]
    private Image _thrusterLevelImg;
    private float _powerLevelPct;

    [SerializeField]
    private Image _livesImage;

    [SerializeField]
    private Sprite[] _liveSprites;

    [SerializeField]
    private GameObject _player;

    private GameManager _gameManager;


    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _gameRestartText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("ERROR! GameManager not found");
        }
    }

    private void Update()
    {
        if (_lowAmmoWarning == true && _lowAmmoFlicker == false)
        {
            StartCoroutine("LowAmmoFlicker");
            _lowAmmoFlicker = true;
        }

        if (_lowAmmoWarning == false && _lowAmmoFlicker == true)
        {
            StopCoroutine("LowAmmoFlicker");
            ResetAmmoDisplay();
        }
    }
    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score " + playerScore;
    }

    public void UpdateAmmoCount(int ammoCount)
    {
        _ammoCountTxt.text = "Ammo " + ammoCount;
        _ammoCount = ammoCount;
    }

    public void SetLowAmmoWarning(bool lowAmmoWarning)
    {
        _lowAmmoWarning = lowAmmoWarning;
    }

    IEnumerator LowAmmoFlicker()
    {
        while (_lowAmmoWarning == true) 
        {
            if (_ammoCount > 0)
            {
                _ammoCountTxt.text = "Ammo " + _ammoCount;
            }
            
            yield return new WaitForSeconds(0.25f);
            if (_ammoCount > 0)
            {
                _ammoCountTxt.text = "low ammo!";
            } else
            {
                _ammoCountTxt.text = "out of ammo!";
                _ammoCountTxt.color = Color.red;
            }
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void ResetAmmoDisplay()
    {
        _ammoCountTxt.color = Color.white;
    }

    public void UpdateThrusterPowerLevel(float thrusterPowerLevel, bool thrusterActive, float thrusterMaxLevel)
    {
        _powerLevelPct = thrusterPowerLevel / thrusterMaxLevel * 100;
        if (thrusterPowerLevel == thrusterMaxLevel)
        {
            _thrusterPowerLevelText.text = "Thruster Ready";
            return;       
        }
        else if (thrusterActive)
        {
            _thrusterPowerLevelText.text = "Power  " + _powerLevelPct.ToString("f0") + "%";
        }
        else 
        {
            if (_powerLevelPct < 100)
            {
                _thrusterPowerLevelText.text = "Charging  " + _powerLevelPct.ToString("f0") + "%";
            }
            else
            {
                _thrusterPowerLevelText.text = "Thruster Ready";
            }
        }
        // change thruster text color
        if (_powerLevelPct > 0 && _powerLevelPct <= 33)
        {
            _thrusterPowerLevelText.color = Color.red;
            _thrusterLevelImg.color = Color.red;
        } else if (_powerLevelPct > 33 && _powerLevelPct <= 66)
        {
            _thrusterPowerLevelText.color = Color.yellow;
            _thrusterLevelImg.color = Color.yellow;
        } else
        {
            _thrusterPowerLevelText.color = Color.green;
            _thrusterLevelImg.color = Color.green;
        } 
    }

    public void UpdateLivesImage(int currentLives)
    {
        if (currentLives > 0)
        {
            _livesImage.sprite = _liveSprites[currentLives];
        }
        
        if (currentLives == 0)
        {
            GameOverSequence();
        }
    }

    void GameOverSequence()
    {
        _gameOverText.gameObject.SetActive(true);
        _gameRestartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
        _gameManager.GameOver();
    }

    IEnumerator GameOverFlickerRoutine()
    { 
        while (true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
