using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Levels : MonoBehaviour
{
    public static Dictionary<int, LevelProfile> all = new Dictionary<int, LevelProfile>();
    public LevelProfile profile;


    private void Awake()
    {
        
    }
}

[Serializable]
public class LevelProfile
{
    public static LevelProfile main; // cur lvl
    public const int maxSize = 12; // max playing field size

    public int levelID = 0; // LevelId;
    public int level = 0; // Level number;

    // size field
    public int width = 9;
    public int height = 9;
    public int colorCount = 6; // count of chip color
    public int targetColorCount = 30; // count of target color in Color mode
    //public int targetSomeElseDropsCount = 0;
    public int firstStarScore = 100; // get one star
    public int secondStarScore = 200; // get two stars
    public int thridStarScore = 300; // get three stars
    //public float stonePortion = 0.0f;
}

