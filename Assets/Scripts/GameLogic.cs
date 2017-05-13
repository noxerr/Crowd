using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour {

    [System.Serializable]
    public class pairGOFloor
    {
        public GameObject go;
        public int floor;
        public pairGOFloor(GameObject go, int floor)
        {
            this.go = go;
            this.floor = floor;
        }
    }

    public GameObject startPoint, finishPoint;
    public GameObject mainAvatar;
    public List<pairGOFloor> doorJoints = new List<pairGOFloor>();
    public bool alarm = false;
    [Tooltip("1 means no danger, 5 means ship is sunken")]
    public int levelOfDanger = 1;

    [HideInInspector]
    public int playerFloor = 4;

    private MindMap userMap;
    private int stairsLayer;
    private float lastYStairs;

    // Use this for initialization
    void Start () {
        mainAvatar.transform.SetPositionAndRotation(startPoint.transform.position, startPoint.transform.rotation);
        stairsLayer = LayerMask.NameToLayer("stairs");
        lastYStairs = startPoint.transform.position.y;
        Invoke("SetAlarm", 3f );
        mainAvatar.GetComponent<PlayerLogic>().OnCollEnter += OnPlayerCollided;

    }

    public void AddJoint(GameObject joint, int floor)
    {
        doorJoints.Add(new pairGOFloor(joint, floor));
    }
	
	// Update is called once per frame
	void Update () {
		if (!alarm)
        {

        }
        else
        {

        }
	}


    private void SetAlarm()
    {
        alarm = true;
    }


    private void OnPlayerCollided(Collider collider)
    {
        GameObject collidedGO = collider.gameObject;
        if (collidedGO.layer == stairsLayer)
        {
            if (lastYStairs + 1 < collidedGO.transform.position.y) { playerFloor += 1; ChangeFloor(); }
            else if (lastYStairs - 1 > collidedGO.transform.position.y) { playerFloor -= 1; ChangeFloor(); }
            lastYStairs = collidedGO.transform.position.y;
        }
    }

    private void ChangeFloor()
    {
        Debug.Log("Floor number: " + playerFloor);
    }

}
