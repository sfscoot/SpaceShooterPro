using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PowerUpClass  
{
    public string name;
    public GameObject powerupPrefab;
    public PowerUpType powerUpType;
    public int waveAvailable;
    public float frequency;
}
