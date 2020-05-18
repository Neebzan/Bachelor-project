﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;

    public EmulatedPlayer emulatedPlayer;
    public EmulatedPlayerUI EmulatedPlayerUI;

    public int LastPlayerUpdateTick = 0;


    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
    }
}
