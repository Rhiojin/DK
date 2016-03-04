using UnityEngine;
using System.Collections;

public class FaceCam : MonoBehaviour {

    public GameObject m_Camera;
 	void Start(){
		m_Camera = GameObject.Find("LookUp");	
	}
    void Update(){
		transform.LookAt(m_Camera.transform);
	}
	
}