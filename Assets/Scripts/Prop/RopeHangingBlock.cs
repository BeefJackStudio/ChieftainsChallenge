using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RopeHangingBlock : MonoBehaviour {
    public GameObject rope;
    public GameObject block;
    public int ropeLength;

    List<GameObject> ropePeaces;

    // Use this for initialization
    void Start() {
        float offset = 0.3f;

        ropePeaces = new List<GameObject>();
        GameObject obj = GameObject.Instantiate(rope);
        GameObject lastObj = obj;
        GetComponent<HingeJoint2D>().connectedBody = obj.GetComponent<Rigidbody2D>();
        ropePeaces.Add(obj);
        obj.transform.parent = transform;
        obj.transform.localPosition = Vector3.zero;

        for (int index = 0; index < ropeLength; index++) {
            obj = GameObject.Instantiate(rope);
            lastObj.GetComponent<HingeJoint2D>().connectedBody = obj.GetComponent<Rigidbody2D>();
            ropePeaces.Add(obj);
            lastObj = obj;
            offset += 0.3f;
            obj.transform.parent = transform;
            obj.transform.localPosition = new Vector3(0, offset, 0);
        }

        obj = GameObject.Instantiate(block, new Vector3(0, offset, 0), Quaternion.identity) as GameObject;
        lastObj.GetComponent<HingeJoint2D>().connectedBody = obj.GetComponent<Rigidbody2D>();
        ropePeaces.Add(obj);
        obj.transform.parent = transform;
    }

    // Update is called once per frame
    void Update() {

    }
}
