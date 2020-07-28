using UnityEngine;
using System.Collections;

public class SideWallCollider : MonoBehaviour {
    public GameObject AquariumManagerObject;
    public GameObject FishingUIObject;
    public GameObject ColliderObject;    
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == ColliderObject.name)
        {
            if (gameObject.name.Equals("LeftWall") || gameObject.name.Equals("RightWall"))
            {
                FishingUIObject.SendMessage("OnMessageDirectionChange", gameObject, SendMessageOptions.DontRequireReceiver);
            }
            else if (gameObject.name.Equals("HitPoint"))
            {
                AquariumManagerObject.SendMessage("OnMessageStayFishingArea", true, SendMessageOptions.DontRequireReceiver);
            }
            else
            {

            }
        }     
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.name == ColliderObject.name)
        {
            if (gameObject.name.Equals("HitPoint"))
            {
                AquariumManagerObject.SendMessage("OnMessageStayFishingArea", false, SendMessageOptions.DontRequireReceiver);
            }
        }
    }    
}
