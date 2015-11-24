using UnityEngine;
using System.Collections;

public class CollectionBehavior : MonoBehaviour {

	bool isFlying;
	Vector3 target;

	// Use this for initialization
	void Awake () 
	{
		isFlying = false;
		target = new Vector3(0,0,0);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (isFlying)
		{
			if (Vector3.Distance (transform.position, target) < 1f)
			{
				Destroy (this.gameObject);
			}
			else
			{
				transform.position = Vector3.Lerp (transform.position, target, 2.3f*Time.deltaTime);
			}
		}
	}

	public void FlyToTarget(Vector3 target_in)
	{
		isFlying = true;
		target = target_in;
	}


}
