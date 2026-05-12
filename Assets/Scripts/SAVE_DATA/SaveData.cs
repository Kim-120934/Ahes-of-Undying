using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public string sceneName;
    public float respawnX;
    public float respawnY;
    public int coins;
    public int currentLives;
    public int currentHits;
    public int currentSoul;
    public bool hasProjectile;
    public bool hasDoubleJump;
    public string playerName;
    public bool hasDash;
    public bool dasherDefeated;
    public string[] destroyedWalls = new string[0];
    public bool bossDefeated;
}
