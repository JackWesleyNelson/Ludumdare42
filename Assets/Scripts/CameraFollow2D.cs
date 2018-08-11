using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow2D : MonoBehaviour {
    [SerializeField]
    Transform target;
    
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
	}
}
