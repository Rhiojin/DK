using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.Rotate(new Vector3(0,Random.Range(0,360),0));
		Destroy(this);
	}
}
