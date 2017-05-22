using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour {
    public static int numFloors = 8;

    [System.Serializable]
    public class FloorContent
    {
        public List<GameObject> joints = new List<GameObject>();
        public GameObject FloorMindPoints, floorParent;
        public void AddJoint(GameObject go)
        {
            joints.Add(go);
        }
        public void SetFloorMindPoints(GameObject parentPoints)
        {
            FloorMindPoints = parentPoints;
        }

        public void SetFloorParent(GameObject floorparent)
        {
            this.floorParent = floorparent;
        }
    }
    

    public GameObject startPoint, finishPoint;
    public GameObject mainAvatar;
    public FloorContent[] floorContent;
    public bool alarm = false;
    [Tooltip("1 means no danger, 5 means ship is sunken")]
    public int levelOfDanger = 1;

    [HideInInspector]
    public int playerFloor = 4;

    private MindMap userMap;
    private int stairsLayer, mind;
    private float lastYStairs;
    private List<GameObject> userSteps;


    // Use this for initialization
    void Start () {
        floorContent = new FloorContent[numFloors];
        for (int i = 0; i < numFloors; i++) floorContent[i] = new FloorContent();
        mainAvatar.transform.SetPositionAndRotation(startPoint.transform.position, startPoint.transform.rotation);
        stairsLayer = LayerMask.NameToLayer("stairs");
        mind = LayerMask.NameToLayer("Mind");
        lastYStairs = startPoint.transform.position.y;
        Invoke("SetAlarm", 3f );
        mainAvatar.GetComponent<PlayerLogic>().OnCollEnter += OnPlayerCollided;
        userSteps = new List<GameObject>();
    }

    public void AddJoint(GameObject joint, int floor) 
    {

        floorContent[floor].AddJoint(joint);
    }

    public void SetFloorMindPoints(GameObject mindsParent, int floor)
    {
        floorContent[floor].SetFloorMindPoints(mindsParent);
    }

    public void SetFloorParent(GameObject floorParent, int floor)
    {
        floorContent[floor].SetFloorParent(floorParent);
        if (floor < playerFloor - 1 || floor > playerFloor + 1) floorParent.SetActive(false);
        else floorParent.SetActive(true);
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
        List<GameObject> joints = floorContent[playerFloor].joints;
        foreach (GameObject go in joints)
        {
            //go.transform.GetChild(0).RotateAround(transform.position, Vector3.up, 90);
            go.transform.GetChild(0).Rotate(0, 90, 0);
        }
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
        else if (collidedGO.layer == mind)
        {
            userSteps.Add(collidedGO);
        }
    }

    private void ChangeFloor()
    {
        Debug.Log("Floor number: " + playerFloor);
        if (floorContent[playerFloor - 1].floorParent != null) floorContent[playerFloor - 1].floorParent.SetActive(true);
        if (floorContent[playerFloor - 2].floorParent != null) floorContent[playerFloor - 2].floorParent.SetActive(false);
        if (floorContent[playerFloor + 1].floorParent != null) floorContent[playerFloor + 1].floorParent.SetActive(true);
        if (floorContent[playerFloor + 2].floorParent != null) floorContent[playerFloor + 2].floorParent.SetActive(false);
        List<GameObject> joints = floorContent[playerFloor].joints;
        foreach (GameObject go in joints)
        {
            go.transform.GetChild(0).Rotate(0, 90, 0);
        }
    }

}
