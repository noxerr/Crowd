using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour {

    public Action<Collider> OnCollEnter;
    private Collider lastCollider;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (lastCollider != other)
        {
            lastCollider = other;
            if (OnCollEnter != null) OnCollEnter(other);
        }
    }
}
