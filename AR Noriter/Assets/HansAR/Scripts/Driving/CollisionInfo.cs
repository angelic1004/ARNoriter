using UnityEngine;
using System.Collections;

public class CollisionInfo : MonoBehaviour {

    private TweenPosition tweenPosition;

    void Start()
    {

        tweenPosition = GetComponent<TweenPosition>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!tweenPosition.enabled)
        {
            Destroy(this.gameObject);
        }
    }
}
