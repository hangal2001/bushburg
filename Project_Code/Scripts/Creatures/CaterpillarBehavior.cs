using UnityEngine;
using System.Collections;

public class CaterpillarBehavior : MonoBehaviour 
{
	static int BASEROLL = 150;

	public GameObject caterpillarPrefab;

	Vector3 destination;
	Renderer render;
	CapsuleCollider collide;

	float discoveryTimer;

	public bool dragging;
	Vector3 oldLoc;

	GameObject currentProspect;

	CitizenManager_Script citizens;

	ForageGhostBehavior forageGhost;

	// Use this for initialization
	void Awake () 
	{
		render = this.gameObject.GetComponent<Renderer>();
		collide = this.gameObject.GetComponent<CapsuleCollider>();

		citizens = GameObject.Find ("CitizenManager").GetComponent<CitizenManager_Script>();
		dragging = false;

		forageGhost = GameObject.Find ("Ghost").GetComponent<ForageGhostBehavior>();

		Deactivate();
		//discoveryTimer = 100;
		ChangeDestination();
	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdateMetrics();
	}

	void UpdateMetrics()
	{

		if (Input.GetKeyDown("x"))
		{
			Activate ();
		}

		if (!dragging)
		{
			Vector3 direction = destination - transform.position;


			if (direction.magnitude < .1f)
			{
				ChangeDestination();
			}

			transform.position += direction.normalized*Time.deltaTime;

			if (discoveryTimer > 0)
			{
				discoveryTimer -= Time.deltaTime*Utilities.TIMESCALE;

				if (discoveryTimer <= 0)
				{
					Deactivate();
				}

			}
			else
			{
				foreach( GameObject cit in citizens.citizens)
				{
					Vector3 distance = cit.transform.position - transform.position;
					
					if (distance.magnitude < 4f)
					{
						TestPerception (cit.GetComponent<CitizenBehavior>().currentAttributes[Utilities.Attributes.Perception]);
					}
				}

				if (forageGhost.citizen != null)
				{
					Vector3 distance = forageGhost.transform.position - transform.position;

					if (distance.magnitude < 12f)
					{
						TestPerception (forageGhost.citizen.currentAttributes[Utilities.Attributes.Perception]*5);
					}
				}
			}
		}
	}

	void ChangeDestination()
	{
		float xloc = Random.Range (-30, 30);
		float zloc;

		if (xloc > -2)
		{
			zloc = Random.Range (-16, 18);
		}
		else
		{
			zloc = Random.Range (-16, 4);
		}

		destination = new Vector3(xloc, -.2f, zloc);
	}

	public void Drop()
	{
		dragging = false;

		if (currentProspect != null)
		{
			CitizenBehavior citizen = currentProspect.GetComponent<CitizenBehavior>();
			citizen.FeedCaterpillar ();

			GameObject.Find ("CreatureManager").GetComponent <CreatureManager_Script>().DestroyCaterpillar(gameObject);
			//GameObject.Instantiate (caterpillarPrefab, new Vector3(0f, -.2f, 0f), Quaternion.identity);


			//Destroy (this.gameObject);
		}
		else
		{
			transform.position = oldLoc;
		}
	}

	void OnTriggerEnter(Collider collision_in)
	{
		if ((dragging) && collision_in.tag == "Citizen")
		{
			currentProspect = collision_in.gameObject;
		}
	}

	//this will remove all targets, so overlapped collisions are a problem
	//OnTriggerStay function included to prevent that problem
	void OnTriggerExit(Collider collision_in)
	{
		if ((dragging) && collision_in.tag == "Citizen")
		{
			currentProspect = null;
		}
	}

	//included so overlapped collisions still keep a target
	void OnTriggerStay(Collider collision_in)
	{
		if ((dragging) && collision_in.tag == "Citizen")
		{
			currentProspect = collision_in.gameObject;
		}
	}

	public void TestPerception(float perception_in)
	{
		int rollmax = ((int)(BASEROLL/Time.deltaTime)/Mathf.CeilToInt (perception_in+.01f));

		int roll = Random.Range (0, rollmax);

		//Utilities.FloatText (transform.position, roll.ToString() + " " + rollmax.ToString(), Utilities.TextTypes.Dbug);

		if (roll == rollmax-1)
		{
			Activate ();
		}
	}
	
	//makes the caterpillar visible
	public void Activate()
	{
		render.enabled = true;
		collide.enabled = true;

		discoveryTimer = 10f;
	}

	//hides the caterpillar
	public void Deactivate()
	{
		render.enabled = false;
		collide.enabled = false;

		discoveryTimer = 0;
	}

	//keeps track of old position in case the drop zone is invalid
	public void Drag()
	{
		oldLoc = transform.position;
		dragging = true;
	}
}
