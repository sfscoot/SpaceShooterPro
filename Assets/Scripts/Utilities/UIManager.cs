using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditorInternal;
using Unity.Collections.LowLevel.Unsafe;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _ammoCountTxt;
    [SerializeField] private TMP_Text _waveEnemyDeathCount;
    [SerializeField] private TMP_Text _homingMissileActiveTxt;
    [SerializeField] private TMP_Text _powerupCollectActiveTxt;
    [SerializeField] private TMP_Text _gameBroadcastMsg;
    private bool _broadcastMsgOn = false;

    private bool _lowAmmoWarning = false;
    private bool _lowAmmoFlicker = false;
    private int _ammoCount;

    [SerializeField] private TMP_Text _gameOverText;
    [SerializeField] private TMP_Text _gameRestartText;
    [SerializeField] private TMP_Text _waveText;

    [SerializeField] private TMP_Text _thrusterPowerLevelText;
    [SerializeField] private Image _thrusterLevelImg;
    private float _powerLevelPct;

    [SerializeField] private Image _livesImage;
    [SerializeField] private Sprite[] _liveSprites;
    [SerializeField] private GameObject _player;

    [SerializeField] private AudioClip _weaponsOfflineClip;
    [SerializeField] private AudioClip _weaponsOnlineClip;
    [SerializeField] private AudioSource _audioSource;

    private GameManager _gameManager;
    private bool _displayHomingMissileActiveMessage = false;
    private bool _displayPowerupCollectActiveMessage = false;


    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _gameRestartText.gameObject.SetActive(false);
        _waveText.gameObject.SetActive(false);
        _gameBroadcastMsg.gameObject.SetActive(false);

        if (_waveText == null) Debug.LogError("level text not assigned");

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)  Debug.LogError("ERROR! GameManager not found");

        _homingMissileActiveTxt.gameObject.SetActive(false);
        _powerupCollectActiveTxt.gameObject.SetActive(false);
        if (_weaponsOfflineClip == null) Debug.LogError("no audio clip found for weapons offline");
        if (_weaponsOnlineClip == null) Debug.LogError("no audio clip found for weapons online");
        _audioSource = _gameBroadcastMsg.gameObject.GetComponent<AudioSource>();
        if (_audioSource == null) Debug.Log("warning message audio source not found");
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

    public void TurnOffScore()
    {
        _scoreText.gameObject.SetActive(false);
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
        _livesImage.sprite = _liveSprites[currentLives];
        
        if (currentLives == 0)
        {
            GameOverSequence();
        }
    }

    public void UpdateKills(int currentKills, int killsNeeded)
    {
        _waveEnemyDeathCount.text = "Kills: " + currentKills + "|" + killsNeeded;
    }

    public void TurnOffEnemyDeathCount()
    {
        _waveEnemyDeathCount.gameObject.SetActive(false);
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

    public void DisplayWaveOn (int level)
    {
        _waveText.gameObject.SetActive(true);
        _waveText.text = "Wave " + level;
    }

    public void GameBroadcastMessage(string msg, int warningAudioID)
    {
        _gameBroadcastMsg.text = msg;
        _gameBroadcastMsg.fontSize = 24;
        _audioSource = _gameBroadcastMsg.gameObject.GetComponent<AudioSource>();
        if (warningAudioID == 1)
        {
            _audioSource.clip = _weaponsOfflineClip;
            _gameBroadcastMsg.color = Color.red;
        }
        if (warningAudioID == 2)
        {
            _audioSource.clip = _weaponsOnlineClip;
            _gameBroadcastMsg.color = Color.green;
        }
        _gameBroadcastMsg.gameObject.SetActive(true);

        _broadcastMsgOn = true;
        StartCoroutine(FlashBroadcastMessage(msg));
    }

    IEnumerator FlashBroadcastMessage(string msg)
    {
        while (_broadcastMsgOn == true) 
        {
            _gameBroadcastMsg.text = msg;
            yield return new WaitForSeconds(0.5f);
            _gameBroadcastMsg.text = " ";
            yield return new WaitForSeconds(0.5f);
        }
    }
    public void GameBroadcastMessageOff()
    {
        _gameBroadcastMsg.gameObject.SetActive(false);
        _broadcastMsgOn = false;
    }
    public void DisplayWaveOff()
    {
        _waveText.gameObject.SetActive(false);
    }

    public void DisplayBossWaveOn(int bossWave)
    {
        _waveText.gameObject.SetActive(true);
        switch (bossWave)
        {
            case 1:
                _waveText.text = "Boss Level Lives at Full Ammo +50";
                break;
            case 2:
                _waveText.text = "Nicely Done, Wave 2 Coming";
                break;
            case 3:
                _waveText.text = "Wave 3 Coming, Good Luck!";
                break;
            case 4:
                _waveText.text = "Wave 4 It's a Hard One";
                break;
            case 5:
                _waveText.text = "Wave 5 This is It - Good Luck Space Cowboy";
                break;
            case 6:
                _waveText.text = "Congratulations You Saved the Galaxy";
                break;
            default:
                Debug.LogWarning("bad switch case in boss UImanager code");
                break;
        }
         
    }

    public void HomingMissileActive()
    {
        _displayHomingMissileActiveMessage = true;
        StartCoroutine(HomingMissileMsgActive());
    }

    public void HomingMissileInactive()
    {
        // _homingMissileActiveTxt.gameObject.SetActive(false);
        _displayHomingMissileActiveMessage = false;
    }

    IEnumerator HomingMissileMsgActive()
    {
        while (_displayHomingMissileActiveMessage == true)
        {

            yield return new WaitForSeconds(0.25f);
            _homingMissileActiveTxt.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.25f);
            _homingMissileActiveTxt.gameObject.SetActive(false);
        }
    }

    public void PowerupCollectActive()
    {
        _displayPowerupCollectActiveMessage = true;
        _powerupCollectActiveTxt.gameObject.SetActive(true);
        // StartCoroutine(PowerupCollectMsgActive());
    }

    public void PowerupCollectInactive()
    {
        // _homingMissileActiveTxt.gameObject.SetActive(false);
        _powerupCollectActiveTxt.gameObject.SetActive(false);
        _displayPowerupCollectActiveMessage = false;
    }

    IEnumerator PowerupCollectMsgActive()
    {
        while (_displayHomingMissileActiveMessage == true)
        {

            yield return new WaitForSeconds(0.25f);
            _powerupCollectActiveTxt.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.25f);
            _powerupCollectActiveTxt.gameObject.SetActive(false);
        }
    }
}
