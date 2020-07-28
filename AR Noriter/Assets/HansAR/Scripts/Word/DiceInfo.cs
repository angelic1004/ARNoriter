using UnityEngine;
using System.Collections;

public class DiceInfo : MonoBehaviour {

    public MeshRenderer meshRenderer;

    public Material[] alphabetMaterial;

    public char alphabet;

    void OnCollisionEnter(Collision col)
    {
        if (col.collider == WordGameManager.instance.objCollider)
        {
            this.GetComponent<Rigidbody>().AddForce(new Vector3(0, 100, -300));
        }
        else if (col.collider == WordGameManager.instance.wall.transform.FindChild(col.collider.name))
        {
            this.GetComponent<Rigidbody>().AddForce(new Vector3(0, 100, -300));
        }
    }
}