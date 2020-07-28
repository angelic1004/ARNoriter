using UnityEngine;
using System.Collections;

public class DisableMapProp : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "kartgame_item" || col.gameObject.tag == "kartgame_fuel" 
            || col.gameObject.tag == "kartgame_obstacle" || col.gameObject.tag == "kartgame_booster")
        {
            col.gameObject.SetActive(false);
            col.transform.parent = KartPlayerManager.getInstance.itemParent;
            col.transform.localPosition = Vector3.zero;
        }
    }
}
