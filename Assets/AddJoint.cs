using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddJoint : MonoBehaviour {
    public GameLogic gl;
    public int floor;
	// Use this for initialization
	void Start () {
        GameObject go = GameObject.FindGameObjectWithTag("GameLogic");
        transform.SetParent(go.transform);
        if (gl == null) gl = go.GetComponent<GameLogic>();
        gl.AddJoint(gameObject, floor);
        //gl.doorJoints.Add(gameObject);
        Destroy(this);
	}
	
}
