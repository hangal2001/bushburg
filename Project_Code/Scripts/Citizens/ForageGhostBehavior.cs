using UnityEngine;
using System.Collections;

public class ForageGhostBehavior : MonoBehaviour 
{
	static float MOVEMENTSCALE = .7f;
	static Vector3 INITIALLOC = new Vector3(4.5f, 0, -9);

	Vector3 endpoint1, endpoint2;
	Vector3 target, direction;

	public CitizenBehavior citizen{get; private set;}

	public bool eradicating {get; private set;}

	// Use this for initialization
	void Awake () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdateMetrics();

		if (eradicating)
		{

		}
		else
		{
			Search();
		}
	}

	void UpdateMetrics()
	{

	}

	void Search()
	{
		Vector3 movement = direction*Time.deltaTime*Utilities.TIMESCALE;
		float distance1 = Vector3.Distance (transform.position, endpoint1);
		float distance2 = Vector3.Distance (transform.position, endpoint2);

		if (movement.magnitude > distance1 && target == endpoint1)
		{
			Pivot (endpoint1);
		}
		else if (movement.magnitude > distance2 && target == endpoint2)
		{
			Pivot (endpoint2);
		}
		else
		{
			transform.position += direction*Time.deltaTime*Utilities.TIMESCALE*MOVEMENTSCALE;
		}
	}

	void Pivot(Vector3 pivotLoc_in)
	{

		if (pivotLoc_in == endpoint1)
		{
			GameController_Script gameController = Utilities.GetGameController ();
			GetEndpoint2(gameController.level);

			target = endpoint2;
			direction = (endpoint2 - endpoint1).normalized;
			transform.position = endpoint1;


		}
		else
		{
			target = endpoint1;
			direction = (endpoint1 - endpoint2).normalized;
			transform.position = endpoint2;
		}


	}

	Vector3 GetEndpoint2(int level_in)
	{

		endpoint2 = new Vector3(4.5f, 0, 0);

		switch (level_in)
		{
			case 1:
				endpoint2 += new Vector3(0,0,8); 
				break;

			case 2:
				endpoint2 += new Vector3(0,0,3);
				break;

			case 3:
				endpoint2 += new Vector3(0,0,-2);
				break;

			default:

				endpoint2 += new Vector3(0,0,-8);
				break;

		}

		return endpoint2;
	}

	public void Initialize(Vector3 startLoc_in, CitizenBehavior citizen_in)
	{
		citizen = citizen_in;

		gameObject.GetComponent<Renderer>().enabled = true;

		eradicating = false;
		endpoint1 = startLoc_in;
		transform.position = INITIALLOC;
		endpoint2 = transform.position;

		Pivot (endpoint2);

	}

	public void Deactivate()
	{
		gameObject.GetComponent<Renderer>().enabled = false;
		citizen = null;
	}
	
}
