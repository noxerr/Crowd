using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour {

    public Action<Collider> OnCollEnter;
    private Collider lastCollider;
    private Animator myAnimator;

    // Use this for initialization
    void Start () {
        myAnimator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        myAnimator.SetFloat("VSpeed", Input.GetAxis("Vertical"));
        //myAnimator.SetFloat("HSpeed", Input.GetAxis("Horizontal"));
        if (Input.GetAxis("Horizontal") != 0 && Input.GetAxis("Vertical") != 0)
        {

            if (Input.GetAxis("Horizontal") > 0) transform.Rotate(Vector3.up * Time.deltaTime * 100f);
            else transform.Rotate(Vector3.up * Time.deltaTime * -100f);
        }
        
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
