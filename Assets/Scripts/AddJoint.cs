﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddJoint : MonoBehaviour {
    private GameLogic gl;
    public int floor;
    private bool alreadySet = false;

    void Start()
    {
        GameObject go = GameObject.FindGameObjectWithTag("GameLogic");
        transform.SetParent(go.transform);
        if (gl == null) gl = go.GetComponent<GameLogic>();

    }

    private void LateUpdate()
    {
        if (gl.floorContent.Length > 0 && !alreadySet)
        {
            gl.AddJoint(gameObject, floor);
            alreadySet = true; 
            Destroy(this);
        }
    }

	
}
