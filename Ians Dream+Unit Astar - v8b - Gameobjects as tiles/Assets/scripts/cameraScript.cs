using UnityEngine;
using System.Collections;

public class cameraScript : MonoBehaviour {

	public Transform target;
	private Vector3 targetPos;

	private Vector3 offSet;
	private Vector3 theTouch;

	private float xBounds;
	private float yBounds;

	private float panSpeed = 15f;

	void Start () 
	{
		targetPos = transform.position;

		xBounds = Screen.width*0.01f;
		yBounds = Screen.height*0.01f;

		//print(Screen.width);
		//print(xBounds*90);

	}
	
	// Update is called once per frame
	void Update () 
	{
		CameraControltwo();
	}

	void CameraControlone()
	{
		if(Input.GetMouseButtonDown(2))
		{
			offSet = transform.position - Input.mousePosition;
		}
		
		if(Input.GetMouseButton(2))
		{
			targetPos = Input.mousePosition + offSet;
			
			transform.position = Vector3.Lerp(transform.position, targetPos, 0.1f*Time.deltaTime);
		}
		
		if(Input.touchCount > 1)
		{
			if(Input.GetTouch(0).phase == TouchPhase.Began)
			{
				offSet = transform.position - Input.mousePosition;
			}
			if(Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary)
			{
				targetPos = Input.mousePosition + offSet;
			}

		}
		
		
		
		transform.position = Vector3.Lerp(transform.position, targetPos, 0.1f*Time.deltaTime);
		
		if(transform.position.x < 3)
		{
			targetPos.x = 8;
			transform.position = targetPos;
		}
		if(transform.position.y < 2)
		{
			targetPos.y = 4;
			transform.position = targetPos;
		}
		if(transform.position.x > 40)
		{
			targetPos.x = 31;
			transform.position = targetPos;
		}
		if(transform.position.y > 38)
		{
			targetPos.y = 35;
			transform.position = targetPos;
		}
	}

	void CameraControltwo()
	{
		if(Input.touchCount > 0)
		{
			if(Input.GetTouch(0).phase == TouchPhase.Moved)
			{
				theTouch = Camera.main.ScreenToWorldPoint( Input.GetTouch(0).position);
				targetPos = theTouch;
				targetPos.z = -10;
				transform.position = Vector3.Lerp(transform.position, targetPos, 2f*Time.deltaTime);
			}

//			theTouch = Input.GetTouch(0).position;
//
//			if(theTouch.x < xBounds*2)
//			{
//				transform.position -= -Vector3.right * panSpeed*Time.deltaTime;
//			}
//
//			if(theTouch.x > xBounds*90)
//			{
//				transform.position += Vector3.right * panSpeed*Time.deltaTime;
//			}
//
//			if(theTouch.y < yBounds*2)
//			{
//				transform.position -= -Vector3.up * panSpeed*Time.deltaTime;
//			}
//
//			if(theTouch.y > yBounds*90)
//			{
//				transform.position -= Vector3.up * panSpeed*Time.deltaTime;
//			}

//			if(theTouch.x < transform.position.x - 1)
//			{
//				transform.position -= Vector3.right * panSpeed*Time.deltaTime;
//			}
//			
//			if(theTouch.x >  transform.position.x + 1)
//			{
//				transform.position += Vector3.right * panSpeed*Time.deltaTime;
//			}
//			
//			if(theTouch.y <  transform.position.y - 1)
//			{
//				transform.position -= Vector3.up * panSpeed*Time.deltaTime;
//			}
//			
//			if(theTouch.y > transform.position.y + 1)
//			{
//				transform.position += Vector3.up * panSpeed*Time.deltaTime;
//			}
		}

		else if(Input.GetMouseButton(2))
		{
			theTouch = Input.mousePosition;

			if(theTouch.x < xBounds*2)
			{
				transform.position -= Vector3.right * panSpeed*Time.deltaTime;
			}
			
			if(theTouch.x > xBounds*90)
			{
				transform.position += Vector3.right * panSpeed*Time.deltaTime;
			}
			
			if(theTouch.y < yBounds*2)
			{
				transform.position -= Vector3.up * panSpeed*Time.deltaTime;
			}
			
			if(theTouch.y > yBounds*90)
			{
				transform.position += Vector3.up * panSpeed*Time.deltaTime;
			}
		}

		if(transform.position.x < 4.5f)
		{
			targetPos = transform.position;
			targetPos.x = 4.5f;
			transform.position = targetPos;
		}
		if(transform.position.y < 2)
		{
			targetPos = transform.position;
			targetPos.y = 2;
			transform.position = targetPos;
		}
		if(transform.position.x > 35)
		{
			targetPos = transform.position;
			targetPos.x = 35;
			transform.position = targetPos;
		}
		if(transform.position.y > 35)
		{
			targetPos = transform.position;
			targetPos.y = 35;
			transform.position = targetPos;
		}
	}
	
}
