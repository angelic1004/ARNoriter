using UnityEngine;
using System.Collections;

public class DriveEduCol : MonoBehaviour {

    private bool addMessage;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "wall")
        {
            DriveEduManager.instance.CollisionEnter(col);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        DriveEduManager.instance.EventCheck(col);
    }
}
